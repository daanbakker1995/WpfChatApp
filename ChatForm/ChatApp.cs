using System;
using System.Net.Sockets;
using System.Text;

namespace ChatForm
{
    public class ChatApp
    {
        // Settings for server
        protected int PortNumber { get; set; }
        protected int BufferSize { get; set; }
        protected string Ipaddres { get; set; }
        protected readonly string ENDOFTRANSITIONCHARACTER = "@-_@";

        // Delegates/Actions to update UI
        protected Action<string> AddMessageToChat;
        protected Action ToggleStartButton;

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
                AddMessageToChat("Fout bij versturen bericht");
            }
        }
    }
}
