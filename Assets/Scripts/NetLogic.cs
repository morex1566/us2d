using Google.Protobuf;
using Net.Protocol;
using NetPacket;
using UnityEngine;

public static partial class NetLogic
{
    [NetReceivedMessage(NetPacket.PacketType.Transformation)]
    public static void MoveCharacter(IMessage<transformation> s2cPayload)
    {

    }

    [NetReceivedMessage(NetPacket.PacketType.Chat)]
    public static void Chat(IMessage<chat> s2cPayload)
    {

    }

    public static partial void Dispatch(PacketHeader header, IMessage payload);
}