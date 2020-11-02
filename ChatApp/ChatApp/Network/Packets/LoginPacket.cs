
namespace ChatApp.Network.Packets
{
    public class LoginPacket : IJSONPacket
    {
        public PacketHeader Header => PacketHeader.Login;
    }
}
