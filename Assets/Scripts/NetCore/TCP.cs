using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NetCore
{
    /// <summary>
    /// IOContext 기반 비동기 TCP 클라이언트.
    /// Connect / Receive / Send 완료 콜백을 IOContext.Post() 로 전달.
    /// </summary>
    public class TCP
    {
        private readonly IOContext _context;

        private Socket _socket;

        private IPEndPoint _endpoint;

        public bool IsConnected => _socket != null && _socket.Connected;

        /// <summary>
        /// IOContext 주입 생성자
        /// </summary>
        public TCP(IOContext context, string host, int port)
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
        /// 소켓 닫기 및 리소스 해제
        /// </summary>
        public void Disconnect()
        {
            if (_socket == null)
            {
                return;
            }
            
            _socket.Close();
        }

        // ─── 내부 콜백 ───────────────────────────────────────────

        private void OnConnectComplete()
        {
            Debug.Log("Connection Completed.");
        }

        private void OnConnectFailed()
        {
            Debug.Log("Connection Failed.");
        }

        private void OnReceiveComplete()
        {

        }

        private void OnSendComplete()
        {

        }
    }
}
