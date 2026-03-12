using Google.Protobuf;
using Net.Protocol;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace NetPacket
{
    /// <summary>
    /// 패킷 종류를 식별하는 열거형
    /// </summary>
    public enum PacketType : byte
    {
        None,

        Connected,

        Disconnected,

        Transformation,

        Chat,

        ServerStats,
    }

    /// <summary>
    /// C++ 서버와 동일한 구조의 패킷 헤더 (18바이트)
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketHeader
    {
        public PacketType TypeId;

        public int ConnectionId;

        public int Sequence;

        public int Timestamp;

        public int payloadSize; // Payload payloadSize

        public byte Checksum;
    }

    /// <summary>
    /// 패킷 데이터를 담는 구조체
    /// </summary>
    public struct Packet
    {
        public PacketHeader Header;

        public byte[] Payload;
    }

    public struct PacketMemoryOwner
    {
        public IMemoryOwner<byte> Header;

        public IMemoryOwner<byte> Payload;
    }

    public static class PacketUtility
    {
        public static readonly int HeaderSize = Marshal.SizeOf(typeof(PacketHeader));

        /// <summary>
        /// 헤더와 페이로드를 포함한 전체 패킷 버퍼 생성
        /// </summary>
        //public static byte[] Serialize(PacketType typeId, IMessage payload)
        //{
        //    int payloadSize = (payload != null) ? (int)payload.CalculateSize() : 0;
        //    int headerSize = Marshal.SizeOf<PacketHeader>();
        //    byte[] totalBuffer = new byte[headerSize + payloadSize];

        //    // 헤더 데이터 준비
        //    PacketHeader header = new PacketHeader();
        //    {
        //        header.TypeId = typeId;
        //        header.payloadSize = payloadSize;
        //    }

        //    // 헤더를 바이트 배열의 앞부분에 복사
        //    IntPtr ptr = Marshal.AllocHGlobal(headerSize);
        //    {
        //        try
        //        {
        //            Marshal.StructureToPtr(header, ptr, false);
        //            Marshal.Copy(ptr, totalBuffer, 0, headerSize);
        //        }
        //        finally
        //        {
        //            Marshal.FreeHGlobal(ptr);
        //        }
        //    }

        //    // 페이로드가 있다면 헤더 바로 뒤에 직렬화
        //    if (payload != null && payloadSize > 0)
        //    {
        //        using (MemoryStream ms = new MemoryStream(totalBuffer, headerSize, payloadSize))
        //        {
        //            using (CodedOutputStream output = new CodedOutputStream(ms))
        //            {
        //                payload.WriteTo(output);
        //                output.Flush();
        //            }
        //        }
        //    }

        //    return totalBuffer;
        //}

        public static unsafe PacketMemoryOwner Serialize(PacketType typeId, IMessage payload)
        {
            int payloadSize = (payload != null) ? (int)payload.CalculateSize() : 0;

            // 메모리 렌트
            IMemoryOwner<byte> headerOwner = MemoryPool<byte>.Shared.Rent(HeaderSize);
            IMemoryOwner<byte> payloadOwner = (payloadSize > 0) ? MemoryPool<byte>.Shared.Rent(payloadSize) : null;

            // 헤더 직렬화 (포인터 직접 복사)
            fixed (byte* ptr = &headerOwner.Memory.Span[0])
            {
                *(PacketHeader*)ptr = new PacketHeader()
                {
                    TypeId = typeId
                };
            }

            // 페이로드 직렬화
            if (payload != null && payloadSize > 0)
            {
                // MemoryMarshall을 이용해 렌트한 버퍼의 실제 배열에 직접 접근 (추가 할당 없음)
                if (MemoryMarshal.TryGetArray<byte>(payloadOwner.Memory, out var segment))
                {
                    using var ms = new MemoryStream(segment.Array, segment.Offset, payloadSize);
                    payload.WriteTo(ms);
                }
            }

            return new PacketMemoryOwner
            {
                Header = headerOwner,
                Payload = payloadOwner
            };
        }

        /// <summary>
        /// 페이로드 바이트 데이터를 구체적인 메시지 객체로 변환
        /// </summary>
        public static T Deserialize<T>(byte[] payload) where T : IMessage<T>, new()
        {
            T message = new T();
            {
                message.MergeFrom(payload);
            }

            return message;
        }

        /// <summary>
        /// 패킷 타입별 파서 매핑
        /// </summary>
        public static readonly Dictionary<PacketType, MessageParser> PacketParserMap = new Dictionary<PacketType, MessageParser>
        {
            { PacketType.Transformation, transformation.Parser },
            { PacketType.Chat, chat.Parser },
            { PacketType.ServerStats, server_stats.Parser }
        };

        public static readonly Dictionary<PacketType, Func<byte[], IMessage>> DeserializeMap = new Dictionary<PacketType, Func<byte[], IMessage>>
        {
            { PacketType.Transformation, Deserialize<transformation> },
            { PacketType.Chat, Deserialize<chat> },
            { PacketType.ServerStats, Deserialize<server_stats> }
        };
    }
}
