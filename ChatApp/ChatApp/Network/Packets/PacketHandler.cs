using System;
using System.Collections.Generic;
using System.Windows;
using Newtonsoft.Json;

namespace ChatApp.Network.Packets
{
    public class PacketHandler
    {

        public EventHandler<SendRequestPacket> SendRequestRecieived;
        public EventHandler<RequestRejectedPacket> RequestRejectedRecieived;
        public EventHandler<RequestAcceptedPacket> RequestAcceptedRecieived;
        public EventHandler<MessagePacket> MessageRecieived;




        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public void Parse(string jsondata)
        {
            var packet = JsonConvert.DeserializeObject<IJSONPacket>(jsondata, _settings);
            if (packet == null)
            {
                MessageBox.Show(jsondata);
               return;
            }
            var header = packet.Header;

            switch (header)
            {
                case PacketHeader.SendRequest:
                    SendRequestRecieived?.Invoke(this, packet as SendRequestPacket);
                    break;
                case PacketHeader.RequestRejected:
                    RequestRejectedRecieived?.Invoke(this, packet as RequestRejectedPacket);
                    break;
                case PacketHeader.RequestAccepted:
                    RequestAcceptedRecieived?.Invoke(this, packet as RequestAcceptedPacket);
                    break;
                case PacketHeader.Message:
                    MessageRecieived?.Invoke(this, packet as MessagePacket);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public string ToJson(object obj)
        {
            return obj is IJSONPacket
                ? JsonConvert.SerializeObject(obj, Formatting.Indented, _settings)
                : throw new ArgumentException("Must be a IJSON packet.");
        }



    }
}
