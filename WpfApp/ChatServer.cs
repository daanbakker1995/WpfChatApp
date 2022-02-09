using ChatForm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppServer
{
    class ChatServer : ChatApp
    {
        // TcpListener for listening for clients
        private TcpListener TcpListener = null;
        // List of TcpCLients
        private List<TcpClient> TcpClients = new();
        // boolean holding the Server Status
        private bool ServerStarted { get; set; } = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="PortNr"></param>
        /// <param name="BuffferSize"></param>
        /// <param name="IPAdress"></param>
        /// <param name="AddMessageToChat"></param>
        /// <param name="ToggleStartButton"></param>
        public ChatServer(int PortNumber, int BufferSize, string Ipaddres, Action<string> AddMessageToChatAction, Action ToggleStartButton) : base(PortNumber, BufferSize, Ipaddres, AddMessageToChatAction, ToggleStartButton)
        {
        }

        public void StartListening()
        {
            try
            {
                ServerStarted = true;
                TcpListener = new TcpListener(IPAddress.Parse(Ipaddres), PortNumber);
                TcpListener.Start();
                AddMessageToChatAction("Luisteren naar chatclients...");
                TcpListener.BeginAcceptTcpClient(AcceptClients, TcpListener); // TODO: Make task
            }
            catch (Exception e)
            {
                ServerStarted = false;
                throw new Exception($"Server Error: {e.Message}");
            }
        }

        private async void AcceptClients(IAsyncResult result)
        {
            if (!ServerStarted) return; // Return if server is not started
            using TcpClient tcpClient = TcpListener.EndAcceptTcpClient(result); // Use the returned TCP client from the 
            try
            {
                TcpClients.Add(tcpClient);
                TcpListener.BeginAcceptTcpClient(AcceptClients, TcpListener);
                await Task.Run(() => ReceiveData(tcpClient));
            }
            catch (Exception e)
            {
                // Add exception to chat if server started and an exception caught
                if (ServerStarted) AddMessageToChatAction("AcceptClients error:" + e.Message);
            }

        }

        private void ReceiveData(TcpClient tcpClient)
        {
            StringBuilder stringBuilder = new();
            try
            {
                using NetworkStream networkStream = tcpClient.GetStream();
                // Send feedback to server UI
                AddMessageToChatAction("Nieuwe chat deelnemer!");
                while (networkStream != null && networkStream.CanRead)
                {
                    // Receive data from stream
                    byte[] byteArray = new byte[BufferSize];
                    int readByteSize = networkStream.Read(byteArray, 0, BufferSize);
                    string message = Encoding.ASCII.GetString(byteArray, 0, readByteSize);

                    // Make one message from received bytes
                    stringBuilder = stringBuilder.Append(message);

                    //end of Message
                    if (!message.EndsWith(ENDOFTRANSITIONCHARACTER)) continue;
                    // Make message readable
                    string clientMessage = stringBuilder.ToString();
                    clientMessage = clientMessage.Remove(clientMessage.Length - ENDOFTRANSITIONCHARACTER.Length);
                    if (clientMessage == "bye") break;
                    // Display message in chat
                    AddMessageToChatAction(clientMessage);
                    // Send message to other connected clients
                    BroadCast(clientMessage, tcpClient);
                    stringBuilder.Clear();
                }
                // Loop is broken so it can't read, remove client.
                if (networkStream.CanRead) RemoveClient(tcpClient);
                return;
            }
            catch (ObjectDisposedException e)
            {
                AddMessageToChatAction($"Object is verwijderd: {e.Message}");
                RemoveClient(tcpClient);
            }
            catch (ArgumentNullException e)
            {
                AddMessageToChatAction($"Geen waarde voor verwerken bericht(en): {e.Message}");
                RemoveClient(tcpClient);
            }
            catch (ArgumentOutOfRangeException e)
            {
                AddMessageToChatAction($"Fout bij het verwerken van bericht(en): {e.Message}");
                RemoveClient(tcpClient);
            }
            catch (Exception)
            {
                AddMessageToChatAction($"Fout bij ontvangen bericht(en)");
                RemoveClient(tcpClient);

            }
        }

        internal bool IsStarted()
        {
            return ServerStarted;
        }

        /// <summary>
        /// Send a message to all connected clients
        /// </summary>
        /// <param name="clientMessage"></param>
        /// <param name="tcpClient"></param>
        public void BroadCast(string clientMessage, TcpClient tcpClient)
        {
            // Send message to all connected clients that are not the tcpClient
            foreach (TcpClient client in TcpClients.Where(client => client != tcpClient))
            {
                SendMessage(clientMessage, client);
            }
        }

        /// <summary>
        /// End connection with a specific client
        /// </summary>
        /// <param name="client"></param>
        private void RemoveClient(TcpClient client)
        {
            AddMessageToChatAction("Een client heeft chat verlaten");
            // Close client and remove from List
            client.Close();
            TcpClients.Remove(client);
        }

        /// <summary>
        /// Stop TcpListener, reset client list and server status
        /// </summary>
        public void CloseServer()
        {
            // Send bye to clients and stop the listener
            ServerStarted = false;
            BroadCast("bye", null);
            TcpListener.Stop();
            // Reset list
            TcpClients.Clear();
            // Update UI
            ToggleStartButtonAction();
            AddMessageToChatAction("Verbinding gesloten..");
        }
    }
}
