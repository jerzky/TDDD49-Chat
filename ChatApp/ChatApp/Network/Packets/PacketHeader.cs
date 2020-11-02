namespace ChatApp.Network.Packets
{
    public enum PacketHeader : byte
    {
        Login = 0,
        Logout = 1,
        Message = 2
    }
}
