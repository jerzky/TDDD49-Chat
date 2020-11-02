
namespace ChatApp.Network.Packets
{
    public class RequestAcceptedPacket : IJSONPacket
    {
        public PacketHeader Header => PacketHeader.RequestAccepted;
        public string Username { get; set; }
    }
}
