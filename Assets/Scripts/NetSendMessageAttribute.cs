using Google.Protobuf;
using NetPacket;
using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method)]
public class NetSendMessageAttribute : Attribute
{
    public PacketType Type { get; }

    public NetSendMessageAttribute(PacketType type) { Type = type; }
}