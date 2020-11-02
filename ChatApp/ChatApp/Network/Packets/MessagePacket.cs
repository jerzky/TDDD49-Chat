
namespace ChatApp.Network.Packets
{
    public class MessagePacket : IJSONPacket
    {
        public PacketHeader Header => PacketHeader.Message;
        public string Message { get; set; }

    }
}
