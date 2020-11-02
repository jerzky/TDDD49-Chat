using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ChatApp.Annotations;
using ChatApp.Dialogs;
using ChatApp.Models;
using ChatApp.Network;
using ChatApp.Network.Packets;
using static System.Int32;

namespace ChatApp.ViewModels
{
    public class ConnectionViewModel : INotifyPropertyChanged
    {
        private readonly Client _client;
        private IAsyncCommand _startListenCommand;
        private IAsyncCommand _connectCommand;
        private string _ip = "127.0.0.1";
        private int _port = 4321;

        private string _username;
        

        private bool _isBusy = true;


        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (value.Equals(_isBusy))
                    return;

                _isBusy = value;
                OnPropertyChanged();
            }
        }
        public IAsyncCommand StartListenCommand
        {
            get { return _startListenCommand ??= new AsyncCommand(StartListen, () => IsBusy); }
        }
        public IAsyncCommand ConnectCommand
        {
            get { return _connectCommand ??= new AsyncCommand(Connect, () => IsBusy); }
        }
        public string IP
        {
            get => _ip;
            set
            {
                if (value.Equals(_ip))
                    return;
                _ip = value;
                OnPropertyChanged();
            }
        }
        public string Port
        {
            get => _port.ToString();
            set
            {
                if (!TryParse(value, out var v))
                    return; 
                if (v.Equals(_port))
                    return;

                _port = v;
                OnPropertyChanged();
            }
        }
        public string Username
        {
            get => _username;
            set
            {
                if (value.Equals(_username))
                    return;

                _username = value;
                _client.MyUsername = value;
                OnPropertyChanged();
            }
        }

        public ConnectionViewModel(Client client)
        {
            _client = client;
            _client.SendRequestRecieved += OnSendRequestRecieved;
        }

        private void OnSendRequestRecieved(object? sender, SendRequestPacket e)
        {

            // MessageBox.Show($"User: {e.Username} wants to connect!");
            //Accept or reject?
            var rrd = new RequestReceivedDialog(e.Username);
            rrd.ShowDialog();
            if(rrd.RequestAccepted) _client.MessageReceived?.Invoke(this, new Message($"{_client.OtherUsername} joined the chat."));
            Task.Run(() => rrd.RequestAccepted ? _client.SendRequestAcceptedPacket(Username) : _client.SendRequestRejectedPacket());
        }


        private async Task StartListen()
        {
            IsBusy = true;
            await _client.StartListener(_port);
        }

        private async Task Connect()
        {
            IsBusy = false;
            await _client.Connect(_ip, _port, Username);
            IsBusy = true;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
