using Google.Protobuf;
using NetCommon;
using NetCore;
using NetPacket;
using System.Runtime.InteropServices;

public class NetManager : MonoBehaviourSingleton<NetManager>
{
    private static IOContext ioContext;

    private static IOContext.WorkGuard workGuard;

    public static TCP tcp;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (tcp == null)
        {
            return;
        }

        DispatchReceives();
    }

    private void Init()
    {
        ioContext = IOContext.GetInstance();
        {
            workGuard = ioContext.MakeWorkGuard();
            ioContext.Run();
        }

        tcp = TCP.GetInstance();
        {
            tcp.Init(ioContext, NetConfig.host, NetConfig.tcpPort);
            tcp.AsyncConnect();
        }
    }

    // 서버에서 온 메시지를 처리
    private void DispatchReceives()
    {
        var queue = tcp.Receive();
        foreach (var request in queue)
        {
            // 헤더 복원
            // 페이로드 복원
            PacketHeader header = MemoryMarshal.Read<PacketHeader>(request.Header.Memory.Span);
            IMessage payload = null;
            if (header.payloadSize > 0 && request.Payload != null)
            {
                if (PacketUtility.PacketParserMap.TryGetValue(header.TypeId, out var parser))
                {
                    payload = parser.ParseFrom(request.Payload.Memory.Span);
                }
            }

            // 패킷 타입에 매핑된 로직 실행
            NetLogic.Dispatch(header, payload);

            // 다 사용한것들 메모리 풀에 반환
            request.Header?.Dispose();
            request.Payload?.Dispose();
        }
    }
}
