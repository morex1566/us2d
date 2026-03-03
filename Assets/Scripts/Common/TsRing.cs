using System;
using System.Threading;

// Volatile : 메모리 베리어(가시성) 

namespace Common
{
    /// <summary>
    /// lock-free MPMC 링버퍼. Push/Pop 모두 CAS 기반으로 thread-safe
    /// </summary>
    public class TsRing<T>
    {
        private struct Node
        {
            public T Data;
            public bool IsReady;
        }

        private readonly Node[] _buffer;
        private readonly int _capacity;
        private int _front; // 다음 쓸 위치
        private int _back; // 다음 읽을 위치

        /// <summary>
        /// 링버퍼 최대 크기 반환
        /// </summary>
        public int Capacity => _capacity;

        /// <summary>
        /// 링버퍼 생성
        /// </summary>
        public TsRing(int capacity)
        {
            _capacity = capacity;
            _buffer = new Node[capacity];
            _front = 0;
            _back = 0;
        }

        /// <summary>
        /// 링버퍼에 데이터를 추가. 버퍼가 꽉 찬 경우 false 반환
        /// </summary>
        public bool Push(T item)
        {
            while (true)
            {
                int currFront = Volatile.Read(ref _front);
                int nextFront = (currFront + 1) % _capacity;

                // 실패 : 버퍼 꽉 참
                if (nextFront == Volatile.Read(ref _back))
                {
                    return false;
                }

                // CAS 전 write 인덱스를 별도 보관 (CAS 성공 시 expected가 변경됨)
                int writeIdx = currFront;
                if (Interlocked.CompareExchange(ref _front, nextFront, currFront) == currFront)
                {
                    _buffer[writeIdx].Data = item;
                    // 데이터 쓰기 완료 후 is_ready 플래그 설정
                    Volatile.Write(ref _buffer[writeIdx].IsReady, true);
                    return true;
                }
            }
        }

        /// <summary>
        /// 링버퍼에서 데이터를 꺼냄. 비어있거나 데이터가 준비 안된 경우 false 반환
        /// </summary>
        public bool Pop(out T item)
        {
            while (true)
            {
                int currBack = Volatile.Read(ref _back);

                // 실패 : 버퍼 비었음
                if (currBack == Volatile.Read(ref _front))
                {
                    item = default;
                    return false;
                }

                // 실패 : 데이터가 아직 안들어왔음
                if (!Volatile.Read(ref _buffer[currBack].IsReady))
                {
                    item = default;
                    return false;
                }

                int nextBack = (currBack + 1) % _capacity;

                // CAS 전 read 인덱스를 별도 보관 (CAS 성공 시 expected가 변경됨)
                int readIdx = currBack;
                if (Interlocked.CompareExchange(ref _back, nextBack, currBack) == currBack)
                {
                    item = _buffer[readIdx].Data;
                    _buffer[readIdx].Data = default;
                    Volatile.Write(ref _buffer[readIdx].IsReady, false);
                    return true;
                }
            }
        }
    }
}
