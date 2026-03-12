using NetPacket;
using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method)]
public class NetReceivedMessageAttribute : Attribute
{
    public PacketType Type { get; }

    public NetReceivedMessageAttribute(PacketType type) { Type = type; }
}