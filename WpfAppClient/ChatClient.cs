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
        {   // Variables
            PortNumber = PortNr;
            BufferSize = BuffferSize;
            Ipaddres = IPAdress;
            // Delegates
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
                using (Client = new TcpClient(Ipaddres, PortNumber))
                {
                    // send feedback to chat
                    AddMessageToChat("Verbonden!");
                    ConnectedToServer = true;
                    // Make Client receivve data from server
                    await Task.Run(() => ReceiveData());
                }
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
            // Using streamwriter from the TCP Server
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
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(message);

                    //end of Message
                    if (message.EndsWith(ENDOFTRANSITIONCHARACTER))
                    {
                        // Make message readable
                        string clientMessage = stringBuilder.ToString();
                        clientMessage = clientMessage.Remove(clientMessage.Length - ENDOFTRANSITIONCHARACTER.Length);
                        if (clientMessage == "bye")
                            break;
                        // Display message in chat
                        AddMessageToChat(clientMessage);
                        // Empty stringBuilder for new message
                        stringBuilder = new StringBuilder();
                    }
                }
                if (ConnectedToServer)
                {
                    CloseConnection();
                }
            }
            catch (Exception)
            {
                if (ConnectedToServer)
                {
                    // Send error to chat and close connection
                    AddMessageToChat("Onverwachte server fout");
                    CloseConnection();
                }
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
            // inform server and close connection
            ConnectedToServer = false;
            Client.Close();
            Client.Dispose();
            // Update UI
            ToggleStartButton();
            AddMessageToChat("Connectie gesloten");
        }
    }

}

