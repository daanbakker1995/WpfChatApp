using ChatForm;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfAppServer;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ServerApp : Window
    {
        private ChatServer Server;
        protected const string START_SERVER_TEXT = "Starten";
        protected const string CLOSE_SERVER_TEXT = "Sluiten";

        public ServerApp()
        {
            InitializeComponent();
            BtnStartServer.Content = START_SERVER_TEXT;
        }

        /// <summary>
        /// <c>Event_Handler</c> BtnStartServer
        /// </summary>
        private void BtnStartServer_Click(object sender, RoutedEventArgs e)
        {
            StartServer();
        }

        public void BtnClearChatList_Click(object sender, RoutedEventArgs e)
        {
            ChatList.Items.Clear();
        }

        /// <summary>
        /// The main meth
        /// </summary>
        private async void StartServer()
        {
            if (IsServerStarted()) { CloseConnection(); return; }
            string ip = InputServerIP.Text;
            string buffer = InputBufferSize.Text;
            string port = InputPortNumber.Text;

            if (!ChatValidator.FieldsAreValid(ip, port, buffer)) { UpdateErrorDisplay("Foute gegevens, controleer probeer opnieuw"); return; }
            UpdateErrorDisplay();

            AddToChatList($"Server Starten...");
            Server = new ChatServer(
                int.Parse(port),
                int.Parse(buffer),
                ip,
                (message) => AddToChatList(message),
                () => UpdateBtnServerStart());

            try
            {
                Server.Start();
                UpdateBtnServerStart();
                AddToChatList("Luisteren naar chatclients...");
                await Server.AcceptClientsAsync();
            }
            catch (SocketException exception)
            {
                AddToChatList("Verbindings fout: " + exception.Message);
                CloseConnection();
                return;
            }
            catch (Exception exception)
            {
                AddToChatList(exception.Message);
                CloseConnection();
                return;
            }
        }

        /// <summary>
        /// Event_Handler BtnSendMessage Click
        /// </summary>
        private void BtnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            string message = InputMessage.Text;
            if (string.IsNullOrWhiteSpace(message)) { UpdateErrorDisplay("Vul een bericht in."); return; }
            if (!IsServerStarted()) { UpdateErrorDisplay("Start eerst de server!"); return; }

            UpdateErrorDisplay();
            try
            {
                Server.BroadCast(message, null);
                AddToChatList(message);
            }
            catch (ArgumentException ex)
            {
                UpdateErrorDisplay(ex.Message);
            }
            catch (Exception ex)
            {
                UpdateErrorDisplay(ex.Message);
            }

            if (message == "bye")
            {
                CloseConnection();
            }

            // Empty message field
            InputMessage.Clear();
            _ = InputMessage.Focus();
        }

        /// <summary>
        /// Checks if there is a server and if its started.
        /// </summary>
        /// <returns></returns>
        private bool IsServerStarted()
        {
            if (Server == null) return false;
            return Server.IsStarted();
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        private void CloseConnection()
        {
            AddToChatList("Verbinding sluiten...");
            Server.CloseServer();
        }

        /// <summary>
        /// Adds a message to the chatlist
        /// </summary>
        private void AddToChatList(string message)
        {
            Dispatcher.Invoke(() =>
            {
                ListBoxItem item = new() { Content = message };
                ChatList.Items.Add(item);
                ChatList.ScrollIntoView(item); // Scroll to item
            });
        }

        /// <summary>
        /// Update interface with Error, by default empty
        /// </summary>
        /// <param name="errorMessage"></param>
        private void UpdateErrorDisplay(string errorMessage = "")
        {
            ErrorTextBlock.Text = errorMessage;
        }

        /// <summary>
        /// Update the text of BtnStartServer
        /// </summary>
        private void UpdateBtnServerStart()
        {
            Dispatcher.Invoke(() =>
            {
                BtnStartServer.Content = IsServerStarted() ? CLOSE_SERVER_TEXT : START_SERVER_TEXT;
            });
        }

        /// <summary>
        /// Event handler for InputServerIP_KeyUp, checks if valid input else update error display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputServerIP_KeyUp(object sender, KeyEventArgs e)
        {
            if (!ChatValidator.IsValidIP(InputServerIP.Text))
            {
                UpdateErrorDisplay("Ongeldig IP adres.");
            }
            else
            {
                UpdateErrorDisplay();
            }
        }

        /// <summary>
        /// Event handler for InportPortNumber_KeyUp, checks if input is valid else updates error display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputPortNumber_KeyUp(object sender, KeyEventArgs e)
        {
            if (ChatValidator.IsValidPortNumber(InputPortNumber.Text))
            {
                UpdateErrorDisplay();
            }
            else
            {
                UpdateErrorDisplay("Ongeldig poort nummer.");
            }
        }

        /// <summary>
        /// Event handler for InputBufferSize_KeyUp, checks if bufferSize is valid else updates error display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputBufferSize_KeyUp(object sender, KeyEventArgs e)
        {
            if (ChatValidator.IsValidBufferSize(InputBufferSize.Text))
            {
                UpdateErrorDisplay();
            }
            else
            {
                UpdateErrorDisplay("Ongeldig poort nummer.");
            }
        }
    }
}