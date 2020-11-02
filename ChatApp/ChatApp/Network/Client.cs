using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using ChatApp.Network.Packets;

namespace ChatApp.Network
{
    public class Client
    {
        
        private readonly PacketHandler _packetHandler = new PacketHandler();
        private TcpClient _tcpClient = new TcpClient();
        private TcpListener _tcpListener;
        private NetworkStream _networkStream;
        private StreamWriter _writer;
        private string _username;

        public EventHandler<SendRequestPacket> SendRequestRecieved;
        public bool Enabled { get; set; } = true;
        public Client()
        {

            _packetHandler.SendRequestRecieived += (sender, packet) =>
            {
                _username = packet.Username;
                SendRequestRecieved?.Invoke(this, packet);
            };
            _packetHandler.RequestAcceptedRecieived += (sender, packet) =>
            {
              //  MessageBox.Show("We have been accepted!");
                _username = packet.Username;
            };
        }



        public async Task Connect(string ip, int port, string username)
        {
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
                    while (await _networkStream.ReadAsync(dataReceived, 0, dataReceived.Length) != 0)
                    {
                        //MessageBox.Show(Encoding.UTF8.GetString(dataReceived));
                        _packetHandler.Parse(Encoding.UTF8.GetString(dataReceived));
                    }
                }
            }
            catch (IOException e)
            {
                MessageBox.Show($"Lost connection to: {_username}.");
            }
            finally
            {
                _tcpClient.Close();
                _tcpClient.Dispose();
            }
        }

        private async Task SendPacket(IJSONPacket packet)
        {
             await _networkStream.WriteAsync(Encoding.UTF8.GetBytes(_packetHandler.ToJson(packet)));
            await _networkStream.FlushAsync();
          //   await _writer.WriteAsync(_packetHandler.ToJson(packet)); 
           //  await _writer.FlushAsync();
        }




        public async Task SendRequestPacket(string username)
        {
            var requestPacket = new SendRequestPacket()
            {
                Username = username
            };
            await SendPacket(requestPacket);
        }
        public async Task SendRequestAcceptPacket(string username)
        {
            var packet = new RequestAcceptedPacket()
            {
                Username = username
            };
            await SendPacket(packet);
        }
    }
}
