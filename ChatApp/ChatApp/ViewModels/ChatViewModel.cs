using ChatApp.Annotations;
using ChatApp.Models;
using ChatApp.Network;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace ChatApp.ViewModels
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        private readonly Client _client;
        private readonly ConversationHistory _history;
        private string _inputMessage;
        private IAsyncCommand _sendClickedCommand;
        private Conversation _conversation = new Conversation();


        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Message> Messages { get; private set; } = new ObservableCollection<Message>();
        public IAsyncCommand SendClickedCommand => _sendClickedCommand ??= new AsyncCommand(SendClicked);

        public string InputMessage 
        { 
            get => _inputMessage; 
            set 
            {
                _inputMessage = value;
                OnPropertyChanged();
            } 
        }

        public ChatViewModel(Client client, ConversationHistory history)
        {
            _client = client;
            _history = history;
            client.MessageReceived += MessageCreate;
            client.ClientDisconnected += ClientDisconnected;
            _conversation.Date = DateTime.Now;
            
          

        }

        private void ClientDisconnected(object? sender, string e)
        {
            _conversation.Date = DateTime.Now;
            _conversation.Username = _client.OtherUsername;
            if (_conversation.Messages.Count > 0)
                _history.Conversations.Add(_conversation);

            _conversation = new Conversation();
        }

        private void MessageCreate(object? sender, Message e)
        {
            Application.Current.Dispatcher.BeginInvoke(() => { Messages.Add(e); });
            if (e.IsSystemMessage)
                return;
            _conversation.Messages.Add(e);
           
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task SendClicked()
        {
            if (string.IsNullOrEmpty(_inputMessage))
                return;

            MessageCreate(this, new Message(_inputMessage, Brushes.Green, _client.MyUsername));
             await _client.SendMessage(_inputMessage);
            InputMessage = "";

        }
    }
}
