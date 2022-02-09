using ChatForm;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfAppClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ClientApp : Window
    {
        ChatClient Client;
        // Strings for butten UI
        private const String START_SERVER_TEXT = "Starten";
        private const String CLOSE_SERVER_TEXT = "Sluiten";

        public ClientApp()
        {
            InitializeComponent();
            BtnStartServer.Content = START_SERVER_TEXT;
        }

        /// <summary>
        /// <c>Event_Handler</c> BtnStartServer
        /// </summary>
        private async void BtnStartServer_Click(object sender, RoutedEventArgs e)
        {
            await NewMethod();
        }

        private async Task NewMethod()
        {
            // If server is started close server
            if (IsServerStarterd()) { CloseConnection(); return; }
            // Validate fields
            if (!ChatValidator.FieldsAreValid(InputServerIP.Text, InputBufferSize.Text, InputPortNumber.Text)) { UpdateErrorDisplay("Foute gegevens, controleer probeer opnieuw"); return; }
            int PortNr = int.Parse(InputPortNumber.Text);
            int BufferSize = int.Parse(InputBufferSize.Text);
            AddToChatList("Connectie maken...");
            Client = new ChatClient(PortNr,
                BufferSize,
                InputServerIP.Text,
                (message) => AddToChatList(message),
                () => UpdateBtnServerStart());
            await Client.Connect();
            if (IsServerStarterd()) AddToChatList("Druk op 'Sluiten' of verzend 'bye' om connectie te sluiten");
            UpdateBtnServerStart();
        }

        private bool IsServerStarterd()
        {
            if (Client == null) return false;
            return Client.IsServerStarted();
        }

        private void CloseConnection()
        {
            Client.CloseConnection();
        }

        /// <summary>
        /// <c>Event_Handler</c> BtnSendMessage Click
        /// </summary>
        private void BtnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            string message = InputMessage.Text;
            if (string.IsNullOrWhiteSpace(message)) { UpdateErrorDisplay("Vul een bericht in."); return; }
            if (!IsServerStarterd()) { UpdateErrorDisplay("Niet verbonden!"); return; }

            // Update UI
            UpdateErrorDisplay();
            try
            {
                Client.SendMessage(message);
                if (message != "bye")
                {
                    AddToChatList(message);
                }
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
            InputMessage.Focus();
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
                // Scroll to item
                ChatList.ScrollIntoView(item);
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
                if (BtnStartServer.Content.ToString() == START_SERVER_TEXT)
                {
                    BtnStartServer.Content = CLOSE_SERVER_TEXT;
                }
                else
                {
                    BtnStartServer.Content = START_SERVER_TEXT;
                }
            });
        }

        /// <summary>
        /// Event handler for InputServerIP_KeyUp, checks if valid input else update error display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputServerIP_KeyUp(object sender, KeyEventArgs e)
        {
            if (ChatValidator.IsValidIP(InputServerIP.Text))
            {
                UpdateErrorDisplay();
            }
            else
            {
                UpdateErrorDisplay("Ongeldig IP adres.");
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
