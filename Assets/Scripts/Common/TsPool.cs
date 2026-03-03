using System;
using System.Collections.Generic;
using System.Threading;

namespace Common
{
    /// <summary>
    /// Thread-safe 오브젝트 풀.
    /// - Acquire() : 풀에서 객체를 꺼내 반환 (자동 반납 Action 포함)
    /// - 풀 고갈 시 fallback new 할당; Release()에서 범위 검사 후 폐기
    /// - T에 Clear() 메서드가 있으면 반납 시 자동 호출 가능 (별도 onRelease 콜백 활용)
    /// </summary>
    public class TsPool<T> where T : class, new()
    {
        public const int MaxCapacity = 40000;

        private readonly object _lock = new object();
        private readonly Stack<T> _pool;
        private readonly int _capacity;
        private readonly Action<T> _onRelease;

        /// <summary>
        /// 풀 생성
        /// </summary>
        public TsPool(int capacity = MaxCapacity, Action<T> onRelease = null)
        {
            _capacity = Math.Min(capacity, MaxCapacity);
            _onRelease = onRelease;
            _pool = new Stack<T>(_capacity);

            for (int i = 0; i < _capacity; ++i)
            {
                _pool.Push(new T());
            }
        }

        /// <summary>
        /// 풀에서 객체를 꺼냄. 풀 고갈 시 새로 할당
        /// </summary>
        public T Acquire()
        {
            lock (_lock)
            {
                if (_pool.Count > 0)
                    return _pool.Pop();
            }
            // 풀 고갈 → fallback 할당
            return new T();
        }

        /// <summary>
        /// 객체를 풀에 반납. 풀이 가득 찬 경우 그냥 폐기
        /// </summary>
        public void Release(T item)
        {
            if (item == null)
                return;

            _onRelease?.Invoke(item);

            lock (_lock)
            {
                if (_pool.Count < _capacity)
                    _pool.Push(item);
                // 용량 초과 시 GC에 맡김
            }
        }

        /// <summary>
        /// 현재 풀에 남은 개수 반환
        /// </summary>
        public int Available
        {
            get { lock (_lock) { return _pool.Count; } }
        }
    }
}
