using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using ChatApp.Network.Packets;
using ChatApp.Models;
using System.Windows.Media;

namespace ChatApp.Network
{
    public class Client
    {
        
        private readonly PacketHandler _packetHandler = new PacketHandler();
        private TcpClient _tcpClient = new TcpClient();
        private TcpListener _tcpListener;
        private NetworkStream _networkStream;
        private StreamWriter _writer;
        private string _otherUsername;
        public string MyUsername { get; set; }
        public string OtherUsername { get => _otherUsername; }

        public EventHandler<SendRequestPacket> SendRequestRecieved;
        public EventHandler<Message> MessageReceived;
        public bool Enabled { get; set; } = true;
        public Client()
        {

            _packetHandler.SendRequestRecieived += (sender, packet) =>
            {
                _otherUsername = packet.Username;
                SendRequestRecieved?.Invoke(this, packet);
            };
            _packetHandler.RequestAcceptedRecieived += (sender, packet) =>
            {
                MessageReceived?.Invoke(this, new Message($"{packet.Username} joined the chat."));
                _otherUsername = packet.Username;
            };
            _packetHandler.RequestRejectedRecieived += (sender, packet) =>
            {
                MessageBox.Show("User rejected your chat request.");
            };

            _packetHandler.MessageRecieived += (sender, packet) =>
            {
                MessageReceived?.Invoke(this, new Message(packet.Message, Brushes.Red, _otherUsername));
            };
        }



        public async Task Connect(string ip, int port, string username)
        {
            MessageReceived?.Invoke(this, new Message($"Connecting to {ip}:{port}"));
            var connectTask = _tcpClient.ConnectAsync(ip, port);
            var timeoutTask = Task.Delay(millisecondsDelay: 2000);
            if (await Task.WhenAny(connectTask, timeoutTask) == timeoutTask)
            {
                MessageBox.Show("User is not avaliable at the moment.");
                return;
                //  throw new TimeoutException();
            }
            _writer = new StreamWriter(_tcpClient.GetStream());
            _networkStream = _tcpClient.GetStream();
            await SendRequestPacket(username);
            await ClientListen();
        }
      


        public async Task StartListener(int port)
        {
            MessageReceived?.Invoke(this, new Message($"Listening to {port}"));
            _tcpListener = TcpListener.Create(port);
            _tcpListener.Start();
            _tcpClient = await _tcpListener.AcceptTcpClientAsync();
            _writer = new StreamWriter(_tcpClient.GetStream());
            _networkStream = _tcpClient.GetStream();
            await ClientListen();
        }



        private async Task ClientListen()
        {
            try
            {
                while (Enabled)
                {
                    var dataReceived = new byte[1024];
                    var length = await _networkStream.ReadAsync(dataReceived, 0, dataReceived.Length);
                    if (length <= 0) 
                        continue;

                    var segment = new ArraySegment<byte>(dataReceived, 0, length);
                    _packetHandler.Parse(Encoding.UTF8.GetString(segment));
                }
            }
            catch (IOException e)
            {
                MessageReceived?.Invoke(this, new Message($"User: {_otherUsername} has left the chat."));
            }
            finally
            {
                _tcpClient.Close();
                _tcpClient.Dispose();
            }
        }

        private async Task SendPacket(IJSONPacket packet)
        {
            if(_tcpClient.Connected)
                await _networkStream.WriteAsync(Encoding.UTF8.GetBytes(_packetHandler.ToJson(packet)));
        }

        public async Task SendMessage(string text)
        {
            await SendPacket(new MessagePacket() { Message = text });
        }

        public async Task SendRequestPacket(string username)
        {
            var requestPacket = new SendRequestPacket()
            {
                Username = username
            };
            await SendPacket(requestPacket);
        }
        public async Task SendRequestAcceptedPacket(string username)
        {
            var packet = new RequestAcceptedPacket()
            {
                Username = username
            };
            await SendPacket(packet);
        }

        public async Task SendRequestRejectedPacket()
        {
            var packet = new RequestRejectedPacket();
            await SendPacket(packet);
        }
    }
}
