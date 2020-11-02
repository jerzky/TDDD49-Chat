using ChatApp.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.ViewModels
{
    public class MainViewModel
    {
        private readonly Client _client = new Client();
        public ChatViewModel ChatViewModel { get; private set; }
        public ConnectionViewModel ConnectionViewModel { get; private set; }

        public MainViewModel()
        {
            ConnectionViewModel = new ConnectionViewModel(_client);
            ChatViewModel = new ChatViewModel(_client);
        }
    }
}
