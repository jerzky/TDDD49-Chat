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
using System.Windows.Media;

namespace ChatApp.ViewModels
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        private Client _client;
        private string _inputMessage;
        private IAsyncCommand _sendClickedCommand;

        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Message> Messages { get; private set; } = new ObservableCollection<Message>();
        public IAsyncCommand SendClickedCommand { get => _sendClickedCommand ??= new AsyncCommand(SendClicked); } 
        public string InputMessage 
        { 
            get => _inputMessage; 
            set 
            {
                _inputMessage = value;
                OnPropertyChanged();
            } 
        }

        public ChatViewModel(Client client)
        {
            _client = client;
            client.MessageReceived += (sender, message) => Messages.Add(message);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task SendClicked()
        {
            Messages.Add(new Message(_inputMessage, Brushes.Green, _client.MyUsername));
            await _client.SendMessage(_inputMessage);
            InputMessage = "";

        }
    }
}
