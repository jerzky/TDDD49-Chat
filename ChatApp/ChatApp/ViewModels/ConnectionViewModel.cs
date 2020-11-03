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
        private readonly MainViewModel _mainViewModel;
        private IAsyncCommand _startListenCommand;
        private IAsyncCommand _connectCommand;
        private string _ip = "127.0.0.1";
        private int _port = 4321;

        private string _username;
        

        private bool _isNotConnected = true;
        private AsyncCommand _disconnectCommand;


        public bool IsNotConnected
        {
            get => _isNotConnected;
            private set
            {
                if (value.Equals(_isNotConnected))
                    return;

                _isNotConnected = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsConnected));
            }
        }

        public bool IsConnected => !_isNotConnected;

        public IAsyncCommand StartListenCommand
        {
            get { return _startListenCommand ??= new AsyncCommand(StartListen); }
        }
        public IAsyncCommand ConnectCommand
        {
            get { return _connectCommand ??= new AsyncCommand(Connect); }
        }
        public IAsyncCommand DisconnectCommand
        {
            get { return _disconnectCommand ??= new AsyncCommand(Disconnect); }
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

        public ConnectionViewModel(Client client, MainViewModel mainViewModel)
        {
            _client = client;
            _mainViewModel = mainViewModel;
            _client.SendRequestRecieved += OnSendRequestRecieved;
            _client.ClientDisconnected += (sender, s) => IsNotConnected = true;
            _client.RequestAcceptedRecieived += (sender, packet) => _mainViewModel.TabIndex = 1;
        }

        private void OnSendRequestRecieved(object? sender, SendRequestPacket e)
        {

            // MessageBox.Show($"User: {e.Username} wants to connect!");
            //Accept or reject?
            var rrd = new RequestReceivedDialog(e.Username)
            {
                Owner = Application.Current.MainWindow
            };
            rrd.ShowDialog();
            if (rrd.RequestAccepted)
            {
                _client.MessageReceived?.Invoke(this, new Message($"{_client.OtherUsername} joined the chat."));
                _mainViewModel.TabIndex = 1;
            }

            Task.Run(() => rrd.RequestAccepted ? _client.SendRequestAcceptedPacket(Username) : _client.SendRequestRejectedPacket());


        }


        private async Task StartListen()
        {
            IsNotConnected = false;
            await _client.StartListener(_port);
        }

        private async Task Connect()
        {
            IsNotConnected = false;
            await _client.Connect(_ip, _port, Username);
           
        }

        private async Task Disconnect()
        {
            IsNotConnected = true;
            _client.MessageReceived?.Invoke(this, new Message($"Disconnected."));
            await Task.Run( () => _client.Disconnect());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
