using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ChatApp.Network.Packets
{
    public class PacketHandler
    {

        public EventHandler<LoginPacket> LoginRecieived;
        public EventHandler<LogoutPacket> LogoutRecieived;
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
               Console.WriteLine("error");
               return;
            }
            var header = packet.Header;

            switch (header)
            {
                case PacketHeader.Login:
                    OnLoginRecieived(packet as LoginPacket);
                    break;
                case PacketHeader.Logout:
                    OnLogoutRecieived(packet as LogoutPacket);
                    break;
                case PacketHeader.Message:
                    OnMessageRecieived(packet as MessagePacket);
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

        private void OnLoginRecieived(LoginPacket packet)
        {
            LoginRecieived?.Invoke(this, packet);
        }

        private void OnLogoutRecieived(LogoutPacket packet)
        {
            LogoutRecieived?.Invoke(this, packet);
        }
        private void OnMessageRecieived(MessagePacket packet)
        {
            MessageRecieived?.Invoke(this, packet);
        }

    }
}
