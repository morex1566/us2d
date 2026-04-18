using Google.FlatBuffers;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using US2D.Network;

namespace US2D.Network.Core
{
    /// <summary>
    /// IOContext 기반 비동기 TCP 클라이언트.
    /// Connect / Receive / Send 완료 콜백을 IOContext.Post() 로 전달.
    /// </summary>
    public class TCP : Singleton<TCP>
    {
        /// <summary>
        /// Default constructor for Singleton
        /// </summary>
        public TCP()
        {

        }

        /// <summary>
        /// TCP Instance Initialization
        /// </summary>
        public void Init(IOContext context, string host, int port)
        {
            _context = context;
            _endpoint = new IPEndPoint(IPAddress.Parse(host), port);
        }

        /// <summary>
        /// 서버에 비동기 연결 시도. 완료 시 IOContext.Post() 를 통해 onConnected 콜백 실행
        /// </summary>
        public void AsyncConnect()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _socket.ConnectAsync(_endpoint).ContinueWith((Task task) =>
            {
                _context.Dispatch(() =>
                {
                    // 서버 연결 성공
                    if (task.IsCompletedSuccessfully)
                    {
                        OnConnectCompleted();
                    }
                    // 서버 연결 실패
                    else
                    {
                        OnConnectFailed();
                    }
                });
            });
        }

        /// <summary>
        /// 연결 해제 (소켓을 닫고 세션을 정리함)
        /// </summary>
        public void AsyncDisconnect()
        {
            if (IsDisconnectValid) return;

            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
            _context.Dispatch(OnDisconnectCompleted);
        }

        public FlatBufferBuilder Receive()
        {
            return _connection.RecvQueue;
        }

        /// <summary>
        /// 해당 서버와 연결된 Connection 클래스 생성
        /// </summary>
        private void OnConnectCompleted()
        {
            // 연결 고유 id 받아옴
            int currConnectionId = Volatile.Read(ref _connectionId);
            Volatile.Write(ref _connectionId, _connectionId + 1);

            // 이전 연결이 있다면 해제
            if (_connection != null)
            {
                AsyncDisconnect();
            }

            // 새로운 연결 생성
            _connection = new Connection(_context, _socket, currConnectionId);
            {
                _connection.Init();
            }

            Debug.Log("connect success.");
        }

        private void OnConnectFailed()
        {
            Debug.LogWarning("connect fail.");
        }

        private void OnDisconnectCompleted()
        {
            _socket = null;
            _connection = null;

            Debug.Log("disconnect success.");
        }


        private IOContext _context;

        private Socket _socket;

        private IPEndPoint _endpoint;

        private Connection _connection;

        private int _connectionId;

        public bool IsConnected => _socket != null && _socket.Connected;

        private bool IsDisconnectValid => _socket == null || !_socket.Connected;
    }
}
