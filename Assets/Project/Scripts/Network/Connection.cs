using Google.FlatBuffers;
using System.Net.Sockets;


namespace TRPG.Runtime.Network
{
    public class Connection
    {
        public Connection(Socket socket, int connectionId)
        {
            this.socket = socket;
            this.connectionId = connectionId;
        }



        public void Init()
        {
            // Nagle off
            socket.NoDelay = true;
        }



        public static readonly int MaxReceiveBufferSize = 1 * 1024 * 1024; // MB

        public static readonly int MaxSendBufferSize = 1 * 1024 * 1024; // MB

        public static readonly int SendBatchInterval = 10; // MS



        private readonly Socket socket;

        private readonly int connectionId;

        private FlatBufferBuilder sendQueue = new(MaxSendBufferSize);

        private FlatBufferBuilder recvQueue = new(MaxReceiveBufferSize);



        public FlatBufferBuilder RecvQueue { get { return recvQueue; } }

        public FlatBufferBuilder SendQueue { get { return sendQueue; } }
    }
}
