using System.Collections.Generic;
using System.Threading;

namespace Common
{
    /// <summary>
    /// 다중 읽기 가능, thread-safe 쓰기 가능한 해시맵
    /// </summary>
    public class TsMap<TKey, TValue>
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly Dictionary<TKey, TValue> _data = new Dictionary<TKey, TValue>();

        /// <summary>
        /// 현재 항목 수 반환
        /// </summary>
        public int Count
        {
            get
            {
                _lock.EnterReadLock();
                try { return _data.Count; }
                finally { _lock.ExitReadLock(); }
            }
        }

        /// <summary>
        /// 키-값 쌍을 삽입하거나 덮어씀
        /// </summary>
        public void Insert(TKey key, TValue value)
        {
            _lock.EnterWriteLock();
            try { _data[key] = value; }
            finally { _lock.ExitWriteLock(); }
        }

        /// <summary>
        /// 키로 값을 조회. 존재하면 true 반환
        /// </summary>
        public bool TryGet(TKey key, out TValue value)
        {
            _lock.EnterReadLock();
            try { return _data.TryGetValue(key, out value); }
            finally { _lock.ExitReadLock(); }
        }

        /// <summary>
        /// 키로 값을 조회. 없으면 default 반환
        /// </summary>
        public TValue Find(TKey key)
        {
            _lock.EnterReadLock();
            try
            {
                _data.TryGetValue(key, out var value);
                return value;
            }
            finally { _lock.ExitReadLock(); }
        }

        /// <summary>
        /// 키에 해당하는 항목 삭제
        /// </summary>
        public void Remove(TKey key)
        {
            _lock.EnterWriteLock();
            try { _data.Remove(key); }
            finally { _lock.ExitWriteLock(); }
        }

        /// <summary>
        /// 키 존재 여부 확인
        /// </summary>
        public bool Contains(TKey key)
        {
            _lock.EnterReadLock();
            try { return _data.ContainsKey(key); }
            finally { _lock.ExitReadLock(); }
        }
    }
}
