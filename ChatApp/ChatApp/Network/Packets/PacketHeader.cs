namespace ChatApp.Network.Packets
{
    public enum PacketHeader : byte
    {
        SendRequest = 0,
        RequestRejected = 1,
        RequestAccepted = 2,
        Message = 3
    }
}
