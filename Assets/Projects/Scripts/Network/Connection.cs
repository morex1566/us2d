using Google.FlatBuffers;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using US2D.Network;


namespace US2D.Network.Core
{
    public class Connection
    {
        public Connection(IOContext context, Socket socket, int connectionId)
        {
            _context = context;
            _socket = socket;
            _connectionId = connectionId;

            //_readHeaderEvt = new SocketAsyncEventArgs();
            //{
            //    _readHeaderEvt.Completed += OnAsyncReadHeaderCompleted;
            //}

            //_readPayloadEvt = new SocketAsyncEventArgs();
            //{
            //    _readPayloadEvt.Completed += OnAsyncReadPayloadCompleted;
            //}

            //_writeEvt = new SocketAsyncEventArgs();
            //{
            //    _writeEvt.Completed += OnAsyncWriteCompleted;
            //}
        }



        public void Init()
        {
            // Nagle off
            _socket.NoDelay = true;
        }

        //public void Dispose()
        //{
        //    // 소켓 정리
        //    _socket?.Close();

        //    // 이벤트 정리
        //    _readHeaderEvt?.Dispose();
        //    _readPayloadEvt?.Dispose();
        //    _writeEvt?.Dispose();

        //    // 큐에 남은 모든 패킷 메모리 정리
        //    while (_recvQueue.TryTake(out var packet))
        //    {
        //        packet.Header?.Dispose();
        //        packet.Payload?.Dispose();
        //    }
        //    _recvQueue.Dispose();
        //    while (_sendQueue.TryTake(out var packet))
        //    {
        //        packet.Header?.Dispose();
        //        packet.Payload?.Dispose();
        //    }
        //    _sendQueue.Dispose();

        //    // 전송용 임시 캐시 정리
        //    _cachedHeaderOwner?.Dispose();
        //    _cachedPayloadOwner?.Dispose();
        //    _cachedSendBuffer.Clear();
        //    _cachedDisposeBuffer.Clear();
        //}

        //public void AsyncRead()
        //{
        //    // 큐잉 한계
        //    if (_recvQueue.IsAddingCompleted)
        //    {
        //        return;
        //    }

        //    AsyncReadHeader();
        //}

        //public void AsyncWrite()
        //{
        //    Common.Timer.Schedule(SendBatchInterval, FlushWrite, this);
        //}

        ///// <summary>
        ///// 클로저로 인한 힙 할당을 막기 위해 connection 자기 자신 매개변수로 지정
        ///// </summary>
        //private void FlushWrite(object state)
        //{
        //    Connection connection = (Connection)state;

        //    // 이미 송신 중이면 중복 진입 방지
        //    if (Interlocked.CompareExchange(ref connection.isWriting, 1, 0) != 0)
        //    {
        //        return;
        //    }

        //    // 패킷을 Threshold만큼 큐에서 추출하여 하나의 BufferList로 구성
        //    while (connection._cachedDisposeBuffer.Count < SendBatchThreshold && connection._sendQueue.TryTake(out var packetMemoryOwner))
        //    {
        //        if (MemoryMarshal.TryGetArray<byte>(packetMemoryOwner.Header.Memory, out var header))
        //        {
        //            connection._cachedSendBuffer.Add(header);
        //        }

        //        if (packetMemoryOwner.Payload != null && MemoryMarshal.TryGetArray<byte>(packetMemoryOwner.Payload.Memory, out var payload))
        //        {
        //            connection._cachedSendBuffer.Add(payload);
        //        }

        //        connection._cachedDisposeBuffer.Add(packetMemoryOwner);
        //    }

        //    // 전송할 데이터가 하나도 없을 때,
        //    if (connection._cachedDisposeBuffer.Count == 0)
        //    {
        //        Interlocked.Exchange(ref connection.isWriting, 0);
        //        return;
        //    }

        //    // 데이터 전송 시작
        //    connection._writeEvt.BufferList = connection._cachedSendBuffer;
        //    connection._writeEvt.UserToken = connection._cachedDisposeBuffer;

        //    // 동기 완료 대응
        //    if (!connection._socket.SendAsync(connection._writeEvt))
        //    {
        //        connection.OnAsyncWriteCompleted(null, connection._writeEvt);
        //    }
        //}

        //private void AsyncReadHeader()
        //{
        //    int headerSize = Marshal.SizeOf<Net.Protocol.PacketHeader>();

        //    // 헤더 수신 시작. Partial Read 대응을 위해 Memory.Slice 사용
        //    if (recvBytes <= 0)
        //    {
        //        _cachedHeaderOwner = MemoryPool<byte>.Shared.Rent(headerSize);
        //        _readHeaderEvt.SetBuffer(_cachedHeaderOwner.Memory);
        //    }
        //    else
        //    {
        //        int remainBytes = headerSize - recvBytes;
        //        _readHeaderEvt.SetBuffer(_cachedHeaderOwner.Memory.Slice(recvBytes, remainBytes));
        //    }

        //    if (!_socket.ReceiveAsync(_readHeaderEvt))
        //    {
        //        OnAsyncReadHeaderCompleted(null, _readHeaderEvt);
        //    }
        //}

        //private unsafe void AsyncReadPayload()
        //{
        //    int payloadSize = 0;
        //    fixed (byte* ptr = &_cachedHeaderOwner.Memory.Span[0])
        //    {
        //        PacketHeader header = *(PacketHeader*)ptr;
        //        payloadSize = header.payloadSize;
        //    }

        //    // 페이로드 수신 시작. 헤더에서 추출한 사이즈만큼 할당 및 수신
        //    if (recvBytes <= 0)
        //    {
        //        _cachedPayloadOwner = MemoryPool<byte>.Shared.Rent(payloadSize);
        //        _readPayloadEvt.SetBuffer(_cachedPayloadOwner.Memory);
        //    }
        //    else
        //    {

        //        int remainBytes = payloadSize - recvBytes;
        //        _readPayloadEvt.SetBuffer(_cachedPayloadOwner.Memory.Slice(recvBytes, remainBytes));
        //    }

        //    if (!_socket.ReceiveAsync(_readPayloadEvt))
        //    {
        //        OnAsyncReadPayloadCompleted(null, _readPayloadEvt);
        //    }
        //}



        //private void OnAsyncReadHeaderCompleted(object sender, SocketAsyncEventArgs evt)
        //{
        //    if (evt.SocketError != SocketError.Success || evt.BytesTransferred == 0)
        //    {
        //        // 소켓 오류 또는 연결 종료 처리
        //        return;
        //    }

        //    // 들어온 데이터 저장
        //    recvBytes += evt.BytesTransferred;

        //    // 아직 데이터 덜 들어옴
        //    int headerSize = Marshal.SizeOf<PacketHeader>();
        //    if (recvBytes < headerSize)
        //    {
        //        AsyncReadHeader();
        //        return;
        //    }

        //    // 데이터 다 들어옴
        //    recvBytes = 0;

        //    AsyncReadPayload();
        //}

        //private unsafe void OnAsyncReadPayloadCompleted(object sender, SocketAsyncEventArgs evt)
        //{
        //    if (evt.SocketError != SocketError.Success || evt.BytesTransferred == 0)
        //    {
        //        // 소켓 오류 또는 연결 종료 처리
        //        return;
        //    }

        //    // 들어온 데이터 저장
        //    recvBytes += evt.BytesTransferred;

        //    int payloadSize = 0;
        //    fixed (byte* ptr = &_cachedHeaderOwner.Memory.Span[0])
        //    {
        //        PacketHeader header = *(PacketHeader*)ptr;
        //        payloadSize = header.payloadSize;
        //    }

        //    // 아직 데이터 덜 들어옴
        //    if (recvBytes < payloadSize)
        //    {
        //        AsyncReadPayload();
        //        return;
        //    }

        //    // 데이터 다 들어옴
        //    recvBytes = 0;

        //    OnAsyncReadCompleted(sender, evt);
        //}

        //private void OnAsyncWriteCompleted(object sender, SocketAsyncEventArgs evt)
        //{
        //    // 전송 완료된 패킷 메모리 해제
        //    if (evt.UserToken is List<PacketMemoryOwner> packets)
        //    {
        //        foreach (var packet in packets)
        //        {
        //            packet.Header.Dispose();
        //            packet.Payload?.Dispose();
        //        }
        //    }

        //    // 이벤트 캐시 초기화 + 쓰기 상태 해제
        //    _cachedSendBuffer.Clear();
        //    _cachedDisposeBuffer.Clear();
        //    evt.BufferList = null;
        //    evt.UserToken = null;

        //    Interlocked.Exchange(ref isWriting, 0);
        //}

        //private void OnAsyncReadCompleted(object sender, SocketAsyncEventArgs evt)
        //{
        //    if (!_recvQueue.TryAdd(new PacketMemoryOwner
        //    {
        //        Header = _cachedHeaderOwner,
        //        Payload = _cachedPayloadOwner
        //    }))
        //    {
        //        Log.Trace("recv queue is full.");
        //    }

        //    _cachedHeaderOwner = null;
        //    _cachedPayloadOwner = null;

        //    // 다시 새로운 패킷 받게 설정
        //    AsyncReadHeader();
        //}



        public static readonly int MaxReceiveBufferSize = 1 * 1024 * 1024; // MB

        public static readonly int MaxSendBufferSize = 1 * 1024 * 1024; // MB

        public static readonly int SendBatchInterval = 10; // MS



        private readonly IOContext _context;

        private readonly Socket _socket;

        private readonly int _connectionId;

        private FlatBufferBuilder _sendQueue = new(MaxSendBufferSize);

        private FlatBufferBuilder _recvQueue = new(MaxReceiveBufferSize);

        public FlatBufferBuilder RecvQueue { get { return _recvQueue; } }

        public FlatBufferBuilder SendQueue { get { return _sendQueue; } }

        private SocketAsyncEventArgs _readHeaderEvt, _readPayloadEvt, _writeEvt;
    }
}
