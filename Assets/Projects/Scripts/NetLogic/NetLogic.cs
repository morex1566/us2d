using Net.Protocol;
using UnityEngine;

public static partial class NetLogic
{
    [NetReceivedMessage(PacketTypeId.S2C_Transform)]
    public static void MoveCharacter(Net.Protocol.Transform s2cPayload)
    {

    }

    [NetReceivedMessage(PacketTypeId.S2C_Chat)]
    public static void Chat(Net.Protocol.Chat s2cPayload)
    {

    }
}