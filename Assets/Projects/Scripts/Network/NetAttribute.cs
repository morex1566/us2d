using Net.Protocol;
using System;

namespace US2D.Network.Logic
{
    [AttributeUsage(AttributeTargets.Field)]
    public class NetFieldAttribute : Attribute
    {
        public PacketTypeId Type { get; }

        public NetFieldAttribute(PacketTypeId type) { Type = type; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class NetReceivedMessageAttribute : Attribute
    {
        public PacketTypeId Type { get; }

        public NetReceivedMessageAttribute(PacketTypeId type) { Type = type; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class NetSendMessageAttribute : Attribute
    {
        public PacketTypeId Id { get; }

        public NetSendMessageAttribute(PacketTypeId id) { Id = id; }
    }
}
