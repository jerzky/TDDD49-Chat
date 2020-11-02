using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChatApp.Dialogs
{
    /// <summary>
    /// Interaction logic for ConnectionDialog.xaml
    /// </summary>
    public partial class RequestReceivedDialog : Window
    {
        public bool RequestAccepted { get; set; }
        public RequestReceivedDialog(string infoText)
        {
            InitializeComponent();
            InfoText.Text = $"User: {infoText} wants to chat!\nDo you wish to accept?";
        }

        private void Button_Accept_Click(object sender, RoutedEventArgs e)
        {
            RequestAccepted = true;
            Close();
        }

        private void Button_Reject_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
