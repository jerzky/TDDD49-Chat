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
        private readonly Client _client;
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


        public MainViewModel(ConnectionViewModel connectionViewModel, ChatViewModel chatViewModel, HistoryViewModel historyViewModel, Client client)
        {
            ConnectionViewModel = connectionViewModel;
            ChatViewModel = chatViewModel;
            HistoryViewModel = historyViewModel;
            _client = client;
            ConnectionViewModel.EditTabIndex = (value) => TabIndex = value;
        }

        public void Close()
        {
            _client.Disconnect();
            HistoryViewModel.Save();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
