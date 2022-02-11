using ChatForm;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppClient
{
    class ChatClient : ChatApp
    {
        // TcpListener for listening for clients
        private TcpClient Client { get; set; }

        // boolean holding the connection status
        private bool ConnectedToServer { get; set; } = false;

        /// <summary>
        /// Contstructor
        /// </summary>
        /// <param name="PortNr"></param>
        /// <param name="BuffferSize"></param>
        /// <param name="IPAdress"></param>
        /// <param name="AddMessageToChat"></param>
        /// <param name="ToggleStartButton"></param>
        public ChatClient(int PortNumber, int BufferSize, string Ipaddres, Action<string> AddMessageToChatAction, Action ToggleStartButton) : base(PortNumber, BufferSize, Ipaddres, AddMessageToChatAction, ToggleStartButton)
        {
        }

        /// <summary>
        /// Method to connect to the server
        /// </summary>
        public async Task ConnectAsync()
        {
            try
            {
                Client = new TcpClient();
                await Client.ConnectAsync(Ipaddres, PortNumber);
                // send feedback to chat
                AddMessageToChatAction("Verbonden!");
                ConnectedToServer = true;
                // Make Client receivve data from server
                await Task.Run(() => ReceiveData());
            }
            catch (Exception)
            {
                AddMessageToChatAction($"Fout bij maken connectie.");
                ConnectedToServer = false;
                UpdateStartButton();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private async void ReceiveData()
        {

            StringBuilder stringBuilder = new();
            try
            {
                using NetworkStream networkStream = Client.GetStream();
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
                    stringBuilder.Clear();
                }
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
            if (ConnectedToServer) CloseConnection();
        }

        internal bool IsServerStarted()
        {
            return ConnectedToServer;
        }

        public void SendMessage(string message)
        {
            SendMessage(message, Client);
        }

        /// <summary>
        /// Closed connection and reset settings
        /// </summary>
        public void CloseConnection()
        {
            // Inform server and close connection
            ConnectedToServer = false;
            if (Client.Connected) SendMessage("bye");
            Client.Close();
            // Update UI
            UpdateStartButton();
            AddMessageToChatAction("Connectie gesloten");
        }
    }

}

