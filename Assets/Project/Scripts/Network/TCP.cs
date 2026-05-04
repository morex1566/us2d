using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using TRPG.Runtime;
using System.Threading;

namespace TRPG.Runtime.Network
{
    /// <summary>
    /// Oracle trpg_server에 연결하거나, listen-server의 host/client로 사용되는 TCP 통신 클래스.
    /// </summary>
    public class TCP : Singleton<TCP>
    {
        private const int connectionTimeOutMs = 10000;

        private Socket socket = null;

        private IPEndPoint endpoint = null;

        private ConnectionStateType state = ConnectionStateType.Disconnected;

        public ConnectionStateType State => state;





        /// <summary>
        /// TCP 연결 대상 초기화.
        /// </summary>
        public bool Init(string host, int port)
        {
            if (IPAddress.TryParse(host, out IPAddress address) == false)
            {
                Debug.LogError($"Invalid IP address: {host}");
                return false;
            }

            if (port <= 0 || port > 65535)
            {
                Debug.LogError($"Invalid TCP port: {port}");
                return false;
            }

            if (state != ConnectionStateType.Disconnected)
            {
                Disconnect();
            }

            endpoint = new IPEndPoint(address, port);
            state = ConnectionStateType.Disconnected;

            return true;
        }





        /// <summary>
        /// Oracle trpg_server에 비동기 연결 시도.
        /// </summary>
        public async Task<bool> ConnectAsync()
        {
            if (state != ConnectionStateType.Disconnected)
            {
                Debug.LogWarning($"TCP is not disconnected. Current state: {state}");
                return false;
            }

            if (endpoint == null)
            {
                Debug.LogError("TCP.Init() must be called.");
                return false;
            }

            Socket connectSocket = CreateSocket();
            state = ConnectionStateType.Connecting;

            return CompleteConnect(connectSocket, await TryConnectAsync(connectSocket));
        }

        private Socket CreateSocket()
        {
            CloseSocket(ref socket);

            Socket newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            {
                newSocket.NoDelay = true;
            }

            socket = newSocket;
            return newSocket;
        }

        /// <summary>
        /// 서버에 비동기 연결 시도.
        /// </summary>
        private async Task<bool> TryConnectAsync(Socket connectSocket)
        {
            try
            {
                await connectSocket.ConnectAsync(endpoint).WaitAsyncEx(connectionTimeOutMs);

                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"TCP socket connect failed. {e.Message}");
                return false;
            }
        }

        private bool CompleteConnect(Socket connectSocket, bool succeeded)
        {
            // 연결 중 소켓이 교체됨
            if (socket != connectSocket)
            {
                CloseSocketImpl(connectSocket);
                return false;
            }

            // 소켓 에러
            if (!succeeded)
            {
                CloseSocket(ref socket);
                state = ConnectionStateType.Disconnected;
                return false;
            }

            state = ConnectionStateType.Connected;
            return true;
        }





        /// <summary>
        /// Oracle trpg_server와의 연결 해제.
        /// </summary>
        public bool Disconnect()
        {
            bool succeeded = CloseSocket(ref socket);
            state = ConnectionStateType.Disconnected;

            return succeeded;
        }

        private bool CloseSocket(ref Socket targetSocket)
        {
            Socket closeTarget = targetSocket;
            targetSocket = null;

            return CloseSocketImpl(closeTarget);
        }

        /// <summary>
        /// 타겟 TCP 소켓 정리.
        /// </summary>
        /// <param name="closeTarget"></param>
        private bool CloseSocketImpl(Socket closeTarget)
        {
            if (closeTarget == null) return true;

            bool succeeded = true;

            try
            {
                if (closeTarget.Connected)
                {
                    closeTarget.Shutdown(SocketShutdown.Both);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"TCP socket shutdown failed. {e.Message}");
            }

            try
            {
                closeTarget.Close();
            }
            catch (Exception e)
            {
                succeeded = false;
                Debug.LogWarning($"TCP socket close failed. {e.Message}");
            }

            return succeeded;
        }
    }
}
