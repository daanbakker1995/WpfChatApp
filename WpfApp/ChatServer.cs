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
        private List<TcpClient> TcpClients;

        // boolean holding the Server Status
        private bool ServerStarted { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="PortNr"></param>
        /// <param name="BuffferSize"></param>
        /// <param name="IPAdress"></param>
        /// <param name="AddMessageToChat"></param>
        /// <param name="ToggleStartButton"></param>
        public ChatServer(int PortNr, int BuffferSize, string IPAdress, Action<string> AddMessageToChat, Action ToggleStartButton)
        {
            PortNumber = PortNr;
            BufferSize = BuffferSize;
            Ipaddres = IPAdress;
            this.AddMessageToChat = AddMessageToChat;
            this.ToggleStartButton = ToggleStartButton;

            ServerStarted = false;
            TcpClients = new List<TcpClient>();
        }

        public void StartListening()
        {
            try
            {
                ServerStarted = true;
                TcpListener = new TcpListener(IPAddress.Parse(Ipaddres), PortNumber);
                TcpListener.Start();
                AddMessageToChat("Luisteren naar chatclients...");
                TcpListener.BeginAcceptTcpClient(AcceptClients, TcpListener); // Make task
            }
            catch (Exception e)
            {
                throw new Exception($"Server Error: {e.Message}");
            }
        }

        private async void AcceptClients(IAsyncResult result)
        {
            if (!ServerStarted) return;
            using TcpClient tcpClient = TcpListener.EndAcceptTcpClient(result); // Use the returned TCP client from the TcpListener

            try
            {
                // Check if server is not closed
                if (!ServerStarted) return;
                // Add Client to list of clients
                TcpClients.Add(tcpClient);
                // Listen for new client to connect
                TcpListener.BeginAcceptTcpClient(AcceptClients, TcpListener);
                // Receive data on net Taks
                await Task.Run(() => ReceiveData(tcpClient));
            }
            catch (Exception e)
            {
                // Add exception to chat if server started and an exception caught
                if (ServerStarted) AddMessageToChat("AcceptClients error:" + e.Message);
            }

        }

        private void ReceiveData(TcpClient tcpClient)
        {
            // Using streamwriter from the TCP Client
            using NetworkStream networkStream = tcpClient.GetStream();
            StringBuilder stringBuilder = new();
            try
            {
                // Send feedback to server UI
                AddMessageToChat("Nieuwe chat deelnemer!");
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
                    AddMessageToChat(clientMessage);
                    // Send message to other connected clients
                    BroadCast(clientMessage, tcpClient);
                    stringBuilder.Clear();
                }
                if (!networkStream.CanRead)
                {
                    RemoveClient(tcpClient);
                }
            }
            catch (Exception)
            {
                if (networkStream.CanRead)
                {
                    // Send error to chat and remove client
                    AddMessageToChat($"Fout bij ontvangen bericht(en)");
                    RemoveClient(tcpClient);
                }
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
            AddMessageToChat("Client heeft chat verlaten");
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
            ToggleStartButton();
            AddMessageToChat("Verbinding gesloten..");
        }
    }
}
