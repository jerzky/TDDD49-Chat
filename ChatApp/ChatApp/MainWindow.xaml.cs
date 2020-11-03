using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChatApp.Models;
using ChatApp.Network;
using ChatApp.ViewModels;

namespace ChatApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var _client = new Client();
            var _history = ConversationHistory.Load("history.json");
            ConnectionViewModel connectionViewModel = new ConnectionViewModel(_client); ;
            var chatViewModel = new ChatViewModel(_client, _history);
            var historyViewModel = new HistoryViewModel(_history);
            DataContext = new MainViewModel(connectionViewModel, chatViewModel, historyViewModel, _client);
        }

        private void MainWindow_OnClosed(object? sender, EventArgs e)
        {
            var mainviewmodel = DataContext as MainViewModel;
            mainviewmodel?.Close();
        }
    }
}
