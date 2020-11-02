
namespace ChatApp.Network.Packets
{
    public class RequestRejectedPacket : IJSONPacket
    {
        public PacketHeader Header => PacketHeader.RequestRejected;
    }
}
