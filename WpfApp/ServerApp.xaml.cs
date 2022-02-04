﻿using ChatForm;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfAppServer;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ServerApp : Window
    {
        private ChatServer Server;
        private const string START_SERVER_TEXT = "Starten";
        private const string CLOSE_SERVER_TEXT = "Sluiten";

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

        private void StartServer()
        {
            if (IsServerStarted()) CloseConnection();
            if (!FieldsAreValid())
            {
                UpdateErrorDisplay("Foute gegevens, controleer probeer opnieuw");
                return;
            }
            UpdateErrorDisplay();
            AddToChatList("Server Starten...");
            Server = new ChatServer(
                int.Parse(InputPortNumber.Text),
                int.Parse(InputBufferSize.Text),
                InputServerIP.Text,
                (message) => AddToChatList(message),
                () => UpdateBtnServerStart());


            try
            {
                Server.StartListening();
                AddToChatList("Druk op 'Sluiten' of verzend 'bye' om connectie te sluiten");
                UpdateBtnServerStart();
            }
            catch (Exception exception)
            {
                AddToChatList(exception.Message);
                CloseConnection();
                return;
            }
        }

        private bool FieldsAreValid()
        {
            return ChatValidator.IsValidIP(InputServerIP.Text) &&
                ChatValidator.IsValidPortNumber(InputPortNumber.Text) &&
                ChatValidator.IsValidBufferSize(InputBufferSize.Text);
        }

        /// <summary>
        /// <c>Event_Handler</c> BtnSendMessage Click
        /// </summary>
        private void BtnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            if (InputMessage.Text == "") UpdateErrorDisplay();
            if (!IsServerStarted()) UpdateErrorDisplay("Start eerst de server!");

            string message = InputMessage.Text;
            UpdateErrorDisplay();
            AddToChatList(message);
            Server.BroadCast(message, null);

            if (message == "bye")
            {
                CloseConnection();
            }

            // Empty message field
            InputMessage.Clear();
            _ = InputMessage.Focus();
        }

        private bool IsServerStarted()
        {
            if (Server == null) return false;
            return Server.IsStarted();
        }

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
                BtnStartServer.Content = (BtnStartServer.Content.ToString() == START_SERVER_TEXT) ? CLOSE_SERVER_TEXT : START_SERVER_TEXT;
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