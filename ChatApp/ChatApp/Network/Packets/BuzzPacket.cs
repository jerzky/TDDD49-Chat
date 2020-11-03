using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Network.Packets
{
    public class BuzzPacket : IJSONPacket
    {
        public PacketHeader Header => PacketHeader.Buzz;
    }
}
