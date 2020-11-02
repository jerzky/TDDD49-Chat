using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using ChatApp.Network.Packets;

namespace ChatApp.Network
{
    public class Client
    {
        
        private readonly PacketHandler _packetHandler = new PacketHandler();
        private TcpClient _tcpClient = new TcpClient();
        private TcpListener _tcpListener;
        private StreamReader _reader;
        private StreamWriter _writer;
       


        public bool Enabled { get; set; } = true;
        public Client()
        {

        }

        private async Task Connect(string ip, int port)
        {
            var connectTask = _tcpClient.ConnectAsync(ip, port);
            var timeoutTask = Task.Delay(millisecondsDelay: 100);
            if (await Task.WhenAny(connectTask, timeoutTask) == timeoutTask)
            {
                throw new TimeoutException();
            }
            await ClientListen();
        }


        private async Task StartListener(int port)
        {
            _tcpListener = TcpListener.Create(port);
            _tcpClient = await _tcpListener.AcceptTcpClientAsync();
            await ClientListen();
        }



        private async Task ClientListen()
        {
            _writer = new StreamWriter(_tcpClient.GetStream());
            _reader = new StreamReader(_tcpClient.GetStream());
            try
            {
                while (Enabled)
                {
                    var recv = await _reader.ReadLineAsync();
                    _packetHandler.Parse(recv);
                }
            }
            catch (SystemException e)
            {
                throw e;
            }
            finally
            {
                await _writer.DisposeAsync();
                _reader.Dispose();
                _tcpClient.Close();
                _tcpClient.Dispose();
            }
        }


    }
}
