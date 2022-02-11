using ChatForm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using System.Threading.Tasks;

namespace WpfAppServer
{
    class ChatServer : ChatApp
    {
        // TcpListener for listening for clients
        private TcpListener _tcpListener = null;
        // List of TcpCLients
        private List<TcpClient> _tcpClients = new();
        // boolean holding the Server Status
        private bool _serverStarted { get; set; } = false;

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

        public void Start()
        {
            try
            {
                _tcpListener = new TcpListener(IPAddress.Parse(Ipaddres), PortNumber);
                _tcpListener.Start();
                _serverStarted = true;
            }catch (SocketException)
            {
                throw new Exception("Fout bij starten server.");
            }
            catch (Exception e)
            {
                _serverStarted = false;
                throw new Exception($"Server Error: {e.GetType()}");
            }
        }

        public async Task AcceptClientsAsync()
        {
            while (_serverStarted)
            {
                try
                {
                    TcpClient client = await _tcpListener.AcceptTcpClientAsync();
                    _tcpClients.Add(client);
                    _ = Task.Run(() => ReceiveDataAsync(client));
                }
                catch (Exception e)
                {
                    if (!_serverStarted) return;
                    _serverStarted = false;
                    CloseServer();
                    throw new Exception($"Server Error: {e.Message}");
                }
            }
        }

        private async Task ReceiveDataAsync(TcpClient tcpClient)
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
                    int readByteSize = await networkStream.ReadAsync(byteArray.AsMemory(0, BufferSize));
                    string message = Encoding.ASCII.GetString(byteArray, 0, readByteSize);

                    // Make one message from received bytes
                    stringBuilder = stringBuilder.Append(message);

                    //end of Message
                    if (networkStream.DataAvailable) continue;

                    // Make message readable
                    string clientMessage = stringBuilder.ToString();
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
            catch (ObjectDisposedException)
            {
                AddMessageToChatAction($"Client is niet bereikbaar");
            }
            catch (IOException)
            {
                AddMessageToChatAction($"Client is niet bereikbaar");
            }
            catch (ArgumentNullException e)
            {
                AddMessageToChatAction($"Geen waarde voor verwerken bericht(en): {e.Message}");
            }
            catch (ArgumentOutOfRangeException e)
            {
                AddMessageToChatAction($"Fout bij het verwerken van bericht(en): {e.Message}");
            }
            catch (Exception e)
            {
                AddMessageToChatAction($"Onverwachte fout ontvangen bericht(en){e.Message}");
            }
            RemoveClient(tcpClient);
        }

        internal bool IsStarted()
        {
            return _serverStarted;
        }

        /// <summary>
        /// Send a message to all connected clients
        /// </summary>
        /// <param name="clientMessage"></param>
        /// <param name="tcpClient"></param>
        public void BroadCast(string clientMessage, TcpClient tcpClient)
        {
            // Send message to all connected clients that are not the tcpClient
            foreach (TcpClient client in _tcpClients.Where(client => client != tcpClient))
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
            AddMessageToChatAction("Verbinding met een client is verbroken.");
            // Close client and remove from List
            client.Close();
            _tcpClients.Remove(client);
        }

        /// <summary>
        /// Stop TcpListener, reset client list and server status
        /// </summary>
        public void CloseServer()
        {
            // Send bye to clients and stop the listener
            _serverStarted = false;
            BroadCast("bye", null);
            if (_tcpListener != null) _tcpListener.Stop();
            _tcpListener = null;
            // Reset list
            _tcpClients.Clear();
            // Update UI
            UpdateStartButton();
            AddMessageToChatAction("Verbinding gesloten..");
        }
    }
}
