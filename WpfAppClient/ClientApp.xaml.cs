﻿using ChatForm;
using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfAppClient;

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
        private void BtnStartServer_Click(object sender, RoutedEventArgs e)
        {
            if (IsServerStarterd()) CloseConnection();
            if (!FieldsAreValid()) { UpdateErrorDisplay("Foute gegevens, controleer probeer opnieuw"); return; }
            int PortNr = int.Parse(InputPortNumber.Text);
            int BufferSize = int.Parse(InputBufferSize.Text);
            AddToChatList("Connectie maken...");
            AddToChatList("Druk op 'Sluiten' of verzend 'bye' om connectie te sluiten");
            Client = new ChatClient(PortNr,
                BufferSize,
                InputServerIP.Text,
                (message) => AddToChatList(message),
                () => UpdateBtnServerStart());
            Client.Connect();
            UpdateBtnServerStart();
        }

        private bool FieldsAreValid()
        {
            return ChatValidator.IsValidIP(InputServerIP.Text) &&
                ChatValidator.IsValidPortNumber(InputPortNumber.Text) &&
                ChatValidator.IsValidBufferSize(InputBufferSize.Text);
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
            if (InputMessage.Text != "")
            {
                if (IsServerStarterd())
                {
                    string message = InputMessage.Text;
                    // Update UI
                    UpdateErrorDisplay();
                    AddToChatList(InputMessage.Text);
                    // Send message tot server
                    Client.SendMessage(InputMessage.Text);
                    if (message == "bye")
                    {
                        CloseConnection();
                    }
                    // Empty message field
                    InputMessage.Clear();
                    InputMessage.Focus();
                }
                else
                {
                    UpdateErrorDisplay("Start eerst de server!");
                }
            }
            else
            {
                UpdateErrorDisplay();
            }
        }

        /// <summary>
        /// Adds a message to the chatlist
        /// </summary>
        private void AddToChatList(string message)
        {
            Dispatcher.Invoke(() =>
            {
                ListBoxItem item = new ListBoxItem { Content = message };
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