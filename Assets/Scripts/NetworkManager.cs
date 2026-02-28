using System.Net.Sockets;
using UnityEngine;


public class NetworkManager : MonoBehaviour
{
    public Socket socket;
    public string serverIP;
    public int serverPort;

    private void Awake()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverIP = "192.168.0.3";
        serverPort = 9000;
    }

    private void Start()
    {
        // 서버에 연결
        socket.Connect(serverIP, serverPort);
    }
}
