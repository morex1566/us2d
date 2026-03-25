using Net.Protocol;
using Common;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Google.FlatBuffers;

namespace NetCore
{
    /// <summary>
    /// IOContext 기반 비동기 TCP 클라이언트.
    /// Connect / Receive / Send 완료 콜백을 IOContext.Post() 로 전달.
    /// </summary>
    public class UDP : Singleton<UDP>
    {
        /// <summary>
        /// Default constructor for Singleton
        /// </summary>
        public UDP()
        {

        }

        /// <summary>
        /// TCP Instance Initialization
        /// </summary>
        public void Init(IOContext context, string host, int port)
        {
            _context = context;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _endpoint = new IPEndPoint(IPAddress.Parse(host), port);
        }

        /// <summary>
        /// 서버에 비동기 연결 시도. 완료 시 IOContext.Post() 를 통해 onConnected 콜백 실행
        /// </summary>
        public void AsyncConnect()
        {
            Debug.Log("Start to AsyncConnect");

            _socket.ConnectAsync(_endpoint).ContinueWith((Task task) =>
            {
                _context.Dispatch(() =>
                {
                    // 서버 연결 성공
                    if (task.IsCompletedSuccessfully)
                    {
                        OnConnectComplete();
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
        /// 연결 해제
        /// </summary>
        public void Disconnect()
        {
            if (_connection != null)
            {
                _connection = null;
            }
           
            _socket.Close();
        }

        public FlatBufferBuilder Receive()
        {
            return _connection.RecvQueue;
        }

        /// <summary>
        /// 해당 서버와 연결된 Connection 클래스 생성
        /// </summary>
        private void OnConnectComplete()
        {
            // 연결 고유 id 받아옴
            int currConnectionId = Volatile.Read(ref _connectionId);
            Volatile.Write(ref _connectionId, _connectionId + 1);

            // 이전 연결이 있다면 해제
            if (_connection != null)
            {
                Disconnect();
            }

            // 새로운 연결 생성
            _connection = new Connection(_context, _socket, currConnectionId);
            {
                _connection.Init();
            }

            Debug.Log("Connection Completed.");
        }

        private void OnConnectFailed()
        {
            Debug.Log("Connection Failed.");
        }


        private IOContext _context;

        private Socket _socket;

        private IPEndPoint _endpoint;

        private Connection _connection;

        private int _connectionId;

        public bool IsConnected => _socket != null && _socket.Connected;
    }
}
