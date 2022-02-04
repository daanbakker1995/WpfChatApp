﻿#pragma checksum "..\..\..\ClientApp.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "1A85B97E87964E718CD4163DF72EC888FA6206EB"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace WpfAppClient {
    
    
    /// <summary>
    /// ClientApp
    /// </summary>
    public partial class ClientApp : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 18 "..\..\..\ClientApp.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox ChatList;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\..\ClientApp.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox InputMessage;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\ClientApp.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnSendMessage;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\ClientApp.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock ErrorTextBlock;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\ClientApp.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox InputServerIP;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\..\ClientApp.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox InputPortNumber;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\ClientApp.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox InputBufferSize;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\..\ClientApp.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnStartServer;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.1.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/WpfAppClient;component/clientapp.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\ClientApp.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.1.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.ChatList = ((System.Windows.Controls.ListBox)(target));
            return;
            case 2:
            this.InputMessage = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.BtnSendMessage = ((System.Windows.Controls.Button)(target));
            
            #line 28 "..\..\..\ClientApp.xaml"
            this.BtnSendMessage.Click += new System.Windows.RoutedEventHandler(this.BtnSendMessage_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.ErrorTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.InputServerIP = ((System.Windows.Controls.TextBox)(target));
            
            #line 42 "..\..\..\ClientApp.xaml"
            this.InputServerIP.KeyUp += new System.Windows.Input.KeyEventHandler(this.InputServerIP_KeyUp);
            
            #line default
            #line hidden
            return;
            case 6:
            this.InputPortNumber = ((System.Windows.Controls.TextBox)(target));
            
            #line 47 "..\..\..\ClientApp.xaml"
            this.InputPortNumber.KeyUp += new System.Windows.Input.KeyEventHandler(this.InputPortNumber_KeyUp);
            
            #line default
            #line hidden
            return;
            case 7:
            this.InputBufferSize = ((System.Windows.Controls.TextBox)(target));
            
            #line 52 "..\..\..\ClientApp.xaml"
            this.InputBufferSize.KeyUp += new System.Windows.Input.KeyEventHandler(this.InputBufferSize_KeyUp);
            
            #line default
            #line hidden
            return;
            case 8:
            this.BtnStartServer = ((System.Windows.Controls.Button)(target));
            
            #line 55 "..\..\..\ClientApp.xaml"
            this.BtnStartServer.Click += new System.Windows.RoutedEventHandler(this.BtnStartServer_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

