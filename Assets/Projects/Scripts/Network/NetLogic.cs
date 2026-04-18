using Net.Protocol;
using UnityEngine;

namespace US2D.Network.Logic
{
    public static partial class NetLogic
    {
        [NetReceivedMessage(PacketTypeId.Transform)]
        public static void OnRecvMoveCharacter(Net.Protocol.Transform payload)
        {

        }

        [NetReceivedMessage(PacketTypeId.Chat)]
        public static void OnRecvChat(Net.Protocol.Chat payload)
        {

        }
    }
}
