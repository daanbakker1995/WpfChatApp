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
                AddMessageToChatAction("Fout bij maken connectie.");
                ConnectedToServer = false;
                UpdateStartButton();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private async void ReceiveData()
        {
            using NetworkStream networkStream = Client.GetStream();
            try
            {
                while (networkStream.CanRead)
                {
                    // Receive data from stream
                    byte[] byteArray = new byte[BufferSize];
                    int resultSize = await networkStream.ReadAsync(byteArray.AsMemory(0, BufferSize));
                    string message = Encoding.ASCII.GetString(byteArray, 0, resultSize);

                    // Make one message from received bytes
                    StringBuilder stringBuilder = new();
                    stringBuilder.Append(message);

                    // if end of message is not end of transmition character continue to receive data.
                    if (!message.EndsWith(ENDOFTRANSITIONCHARACTER)) continue;

                    // Make message readable
                    string clientMessage = stringBuilder.ToString();
                    clientMessage = clientMessage.Remove(clientMessage.Length - ENDOFTRANSITIONCHARACTER.Length);
                    // If received message is bye the server is closed so the client so break connection.
                    if (clientMessage == "bye")
                        break;
                    // Display message in chat
                    AddMessageToChatAction(clientMessage);
                    // Empty stringBuilder for new message
                    stringBuilder = new StringBuilder();
                }
            }
            catch (IOException e)
            {
                if (ConnectedToServer)
                {
                    // Send error to chat and close connection
                    AddMessageToChatAction($"Communicatie fout: {e.Message}");
                }
            }
            catch (Exception e)
            {
                // Send error to chat and close connection
                AddMessageToChatAction($"Onverwachte server fout: {e.GetType().Name}");
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

