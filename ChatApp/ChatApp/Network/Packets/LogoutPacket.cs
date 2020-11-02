
namespace ChatApp.Network.Packets
{
    public class LogoutPacket : IJSONPacket
    {
        public PacketHeader Header => PacketHeader.Logout;
    }
}
