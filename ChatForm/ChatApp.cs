using System;
using System.Net.Sockets;
using System.Text;

namespace ChatForm
{
    public abstract class ChatApp
    {
        // Settings for server
        protected int PortNumber { get; set; }
        protected int BufferSize { get; set; }
        protected string Ipaddres { get; set; }
        protected readonly string ENDOFTRANSITIONCHARACTER = "@-_@";

        // Delegates/Actions to update UI
        protected Action<string> AddMessageToChatAction;
        protected Action ToggleStartButtonAction;

        protected ChatApp(int PortNumber, int BufferSize, string Ipaddres, Action<string> AddMessageToChatAction, Action ToggleStartButtonAction)
        {
            this.PortNumber = PortNumber;
            this.BufferSize = BufferSize;
            this.Ipaddres = Ipaddres;
            this.AddMessageToChatAction = AddMessageToChatAction;
            this.ToggleStartButtonAction = ToggleStartButtonAction;
        }

        /// <summary>
        /// Sends message to connected server
        /// </summary>
        /// <param name="message"></param>
        protected void SendMessage(String message, TcpClient client)
        {
            try
            {
                // get Networkstream
                var stream = client.GetStream();
                // Make message ready to send
                message += ENDOFTRANSITIONCHARACTER;
                var buffer = Encoding.ASCII.GetBytes(message);
                // Write message
                stream.Write(buffer, 0, buffer.Length);
            }
            catch (Exception)
            {
                AddMessageToChatAction("Fout bij versturen bericht");
            }
        }
    }
}
