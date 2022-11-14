public class BasePacket : IPacketInfo
{
    public long PacketID { get; set; }

    public BasePacket(long packetID)
    {
        PacketID = packetID;
    }
}