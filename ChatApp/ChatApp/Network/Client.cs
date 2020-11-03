using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ChatApp.Network.Packets;
using ChatApp.Models;
using System.Windows.Media;
using System.Diagnostics;

namespace ChatApp.Network
{
    public class Client
    {
        
        private readonly PacketHandler _packetHandler = new PacketHandler();
        private TcpClient _tcpClient = new TcpClient();
        private TcpListener _tcpListener;

        private string _otherUsername;
        public string MyUsername { get; set; }
        public string OtherUsername => _otherUsername;

        public EventHandler<SendRequestPacket> SendRequestRecieved;
        public EventHandler<RequestAcceptedPacket> RequestAcceptedRecieived;
        public EventHandler<BuzzPacket> BuzzReceived;
        public EventHandler<Message> MessageReceived;
        public EventHandler<string> ClientDisconnected;

        public bool Enabled { get; set; } = true;

        public bool IsConnected => _tcpClient.Connected;


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
                RequestAcceptedRecieived?.Invoke(sender, packet);
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

            _packetHandler.BuzzReceived += (sender, packet) =>
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer("nudge.wav");
                player.Play();
            };
        }



        public async Task Connect(string ip, int port, string username)
        {
            _tcpClient = new TcpClient();
            MessageReceived?.Invoke(this, new Message($"Connecting to {ip}:{port}"));
            var connectTask = _tcpClient.ConnectAsync(ip, port);
            var timeoutTask = Task.Delay(millisecondsDelay: 200);
            if (await Task.WhenAny(connectTask, timeoutTask) == timeoutTask)
            {
                MessageBox.Show("User is not avaliable at the moment.");
                await Task.Delay(1000);
                ClientDisconnected?.Invoke(this, "");
                return;
            }


            if (!_tcpClient.Connected)
            {
                Disconnect();
                return;
            }

            await SendRequestPacket(username);
            await ClientListen();
        }


        public async Task StartListener(int port)
        {
            MessageReceived?.Invoke(this, new Message($"Listening to {port}"));
            _tcpListener = TcpListener.Create(port);
            _tcpListener.Start();
            try
            {
                _tcpClient = await _tcpListener.AcceptTcpClientAsync();
            }
            catch (ObjectDisposedException e)
            {
                Disconnect();
                return;
            }
            await ClientListen();
        }


        private async Task ClientListen()
        {
            Enabled = true;
            try
            {
                while (Enabled)
                {


                    var dataReceived = new byte[1024];
                    var length = await _tcpClient.Client.ReceiveAsync(dataReceived,  SocketFlags.None);

                    if (length == 0)
                    {
                        Disconnect();
                        break;
                    }

                    var segment = new ArraySegment<byte>(dataReceived, 0, length);
                    _packetHandler.Parse(Encoding.UTF8.GetString(segment));
                }
            }
            catch (IOException e)
            {
                
                Disconnect();
            }
            catch (SocketException e)
            {
                Disconnect();
            }
        }


        public void Disconnect()
        {
            MessageReceived?.Invoke(this, new Message($"User: {_otherUsername} has left the chat."));
            Enabled = false;
            if (_tcpClient.Connected)
                _tcpClient.Close();
            _tcpListener?.Stop();
            ClientDisconnected?.Invoke(this, "");
        }

        private async Task SendPacket(IJSONPacket packet)
        {
            if(_tcpClient.Connected)
                await _tcpClient.GetStream().WriteAsync(Encoding.UTF8.GetBytes(_packetHandler.ToJson(packet)));
        }

        public async Task SendMessage(string text)
        {
            await SendPacket(new MessagePacket() { Message = text });
        }

        public async Task SendBuzz()
        {
            await SendPacket(new BuzzPacket());
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
