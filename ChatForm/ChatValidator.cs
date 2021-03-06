using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace ChatForm
{
    public static class ChatValidator
    {
        public static bool FieldsAreValid(string IpAdress, string Portnumber, string Buffersize)
        {
            return IsValidIP(IpAdress) && IsValidPortNumber(Portnumber) && IsValidBufferSize(Buffersize);
        }

        /// <summary>
        /// Checks if given string can be converted into an int and is its a valid number based on <code>IsValidPortNumber</code>
        /// </summary>
        /// <param name="portNr"></param>
        /// <returns></returns>
        public static bool IsValidPortNumber(string portNr)
        {
            return (portNr != "") && int.TryParse(portNr, out int portnummer) && portnummer > 0;
        }

        /// <summary>
        /// Check if param is valid ip4
        /// </summary>
        /// <param name="IPAdress"></param>
        /// <returns></returns>
        public static bool IsValidIP(string IPAdress)
        {
            const int allowedSplitValues = 4;
            string[] splitValues = IPAdress.Split('.');
            if (splitValues.Length != allowedSplitValues || string.IsNullOrWhiteSpace(IPAdress)) return false; // Check if split value is 4.
            return splitValues.All(x => byte.TryParse(x, out _)) && IPAddress.TryParse(IPAdress, out _); // Returns true if all split values can be parsed to bytes
        }

        public static bool IsValidBufferSize(string BufferSize)
        {
            if (string.IsNullOrEmpty(BufferSize)) return false;

            Regex RegMatch = new(@"^[0-9]*$"); // Regular expression for numbers
            if (!RegMatch.IsMatch(BufferSize)) return false;

            if (!int.TryParse(BufferSize, out int bufferSizeInt) || bufferSizeInt <= 0)
            {
                return false;
            }
            return true;
        }
    }
}
