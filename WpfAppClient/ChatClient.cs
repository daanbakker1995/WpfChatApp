using ChatForm;
using System;
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
        private bool ConnectedToServer { get; set; }

        /// <summary>
        /// Contstructor
        /// </summary>
        /// <param name="PortNr"></param>
        /// <param name="BuffferSize"></param>
        /// <param name="IPAdress"></param>
        /// <param name="AddMessageToChat"></param>
        /// <param name="ToggleStartButton"></param>
        public ChatClient(int PortNr, int BuffferSize, String IPAdress, Action<String> AddMessageToChat, Action ToggleStartButton)
        {   // Initialize Variables
            PortNumber = PortNr;
            BufferSize = BuffferSize;
            Ipaddres = IPAdress;
            // Initialize Delegates
            this.AddMessageToChat = AddMessageToChat;
            this.ToggleStartButton = ToggleStartButton;
            // Server Setting
            ConnectedToServer = false;
        }

        /// <summary>
        /// Method to connect to the server
        /// </summary>
        public async void Connect()
        {
            try
            {
                Client = new TcpClient(Ipaddres, PortNumber);
                // send feedback to chat
                AddMessageToChat("Verbonden!");
                ConnectedToServer = true;
                // Make Client receivve data from server
                await Task.Run(() => ReceiveData());

            }
            catch (Exception)
            {
                AddMessageToChat("Fout bij maken connectie.");
                ConnectedToServer = false;
                ToggleStartButton();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ReceiveData()
        {
            using NetworkStream networkStream = Client.GetStream();
            try
            {
                while (true)
                {
                    // Receive data from stream
                    byte[] byteArray = new byte[BufferSize];
                    int resultSize = networkStream.Read(byteArray, 0, BufferSize);
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
                    AddMessageToChat(clientMessage);
                    // Empty stringBuilder for new message
                    stringBuilder = new StringBuilder();
                }
                if (ConnectedToServer) CloseConnection();
            }
            catch (Exception)
            {
                if (!ConnectedToServer) return;
                // Send error to chat and close connection
                AddMessageToChat("Onverwachte server fout");
                CloseConnection();
            }
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
            Client.Close();
            Client.Dispose();
            // Update UI
            ToggleStartButton();
            AddMessageToChat("Connectie gesloten");
        }
    }

}

