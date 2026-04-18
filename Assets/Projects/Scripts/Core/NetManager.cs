using UnityEngine;
using US2D.Network;
using US2D.Network.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum NetProtocol
{
    TCP,
    UDP
}

public class NetManager : MonoBehaviourSingleton<NetManager>
{
    private static readonly string Host = "192.168.0.3";

    private static readonly int TcpPort = 60000;

    private static IOContext ioContext;

    private static IOContext.WorkGuard workGuard;

    private static TCP tcp;

    public static TCP TCP => tcp;


#if UNITY_EDITOR
    [MenuItem("Network/Init")]
#endif
    public static void Init()
    {
        GetInstance();
        {
            ioContext = IOContext.GetInstance();
            {
                workGuard = ioContext.MakeWorkGuard();
                ioContext.Run();
            }

            tcp = TCP.GetInstance();
            {
                tcp.Init(ioContext, Host, TcpPort);
            }
        }
    }

#if UNITY_EDITOR
    [MenuItem("Network/Connect")]
#endif
    public static void Connect()
    {
        tcp.AsyncConnect();
    }

#if UNITY_EDITOR
    [MenuItem("Network/Disconnect")]
#endif
    public static void Disconnect()
    {
        tcp.AsyncDisconnect();
    }
}
