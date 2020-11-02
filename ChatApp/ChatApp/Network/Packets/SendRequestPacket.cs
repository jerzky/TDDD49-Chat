
namespace ChatApp.Network.Packets
{
    public class SendRequestPacket : IJSONPacket
    {
        public PacketHeader Header => PacketHeader.SendRequest;
        public string Username { get; set; }
    }
}
