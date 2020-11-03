using ChatApp.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using ChatApp.Annotations;
using ChatApp.Models;

namespace ChatApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly Client _client = new Client();
        public ChatViewModel ChatViewModel { get; private set; }
        public ConnectionViewModel ConnectionViewModel { get; private set; }
        public HistoryViewModel HistoryViewModel { get; private set; }


        private readonly ConversationHistory _history;
        private int _tabIndex = 0;

        public int TabIndex
        {
            get => _tabIndex;
            set
            {
                if (value.Equals(_tabIndex))
                    return;

                _tabIndex = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            _history = ConversationHistory.Load("history.json");

            ConnectionViewModel = new ConnectionViewModel(_client, this);
            ChatViewModel = new ChatViewModel(_client, _history);
            HistoryViewModel = new HistoryViewModel(_history);
        }

        public void Close()
        {
            _client.Disconnect();
            ConversationHistory.Save(_history, "history.json");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
