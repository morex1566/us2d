using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace NetCore
{
    /// <summary>
    /// asio::io_context 와 동일한 구조의 Proactor 이벤트 루프.
    /// Post() 로 완료 콜백을 큐에 적재하고, Run() 을 실행 중인 전용 스레드에서 drain.
    /// </summary>
    public class IOContext : Singleton<IOContext>
    {
        // ring buffer가 비어도 context loop가 유지될 수 있도록
        public sealed class WorkGuard : IDisposable
        {
            private readonly IOContext _context;

            private bool _disposed;

            internal WorkGuard(IOContext ctx)
            {
                _context = ctx;
                Interlocked.Increment(ref _context._workCount);
            }

            public void Dispose()
            {
                if (_disposed) return;
                _disposed = true;
                Interlocked.Decrement(ref _context._workCount);
                _context._signal.Set(); // Run() 루프에 변화 알림
            }
        }

        private readonly ConcurrentQueue<Action> _queue = new ConcurrentQueue<Action>();

        private readonly ManualResetEventSlim _signal = new ManualResetEventSlim(false);

        private bool _stopped;

        // WorkGuard 참조 카운터
        private int _workCount;

        // Dispatch용
        private int _workerThreadId;


        /// <summary>
        /// WorkGuard 생성. Dispose 전까지 Stop() 호출 이후에도 Run() 루프 유지
        /// </summary>
        public WorkGuard MakeWorkGuard() { return new WorkGuard(this); }

        /// <summary>
        /// 완료 콜백을 큐에 적재. 큐 가득 찬 경우 드롭 (용량 조정 필요)
        /// </summary>
        public void Post(Action handler)
        {
            if (handler == null) return;
            _queue.Enqueue(handler);
            _signal.Set();
        }

        /// <summary>
        /// 현재 스레드가 워커 스레드면 즉시 실행, 아니면 Post
        /// </summary>
        public void Dispatch(Action handler)
        {
            if (Thread.CurrentThread.ManagedThreadId == _workerThreadId)
            {
                handler?.Invoke();
            }
            else
            {
                Post(handler);
            }
        }

        /// <summary>
        /// 전용 스레드 진입점. Stop() 호출 + 큐가 빌 때까지 블로킹
        /// </summary>
        public void Run()
        {
            _stopped = false;

            Task.Run(() =>
            {
                _workerThreadId = Thread.CurrentThread.ManagedThreadId;

                while (true)
                {
                    // 큐 드레인
                    while (_queue.TryDequeue(out var handler))
                    {
                        handler.Invoke();
                    }

                    // 종료
                    if (_stopped)
                    {
                        break;
                    }

                    // busy-wait 방지
                    _signal.Wait();
                    _signal.Reset();
                }
            });
        }

        /// <summary>
        /// Run() 루프에 종료 신호를 보냄. WorkGuard가 없으면 즉시 종료
        /// </summary>
        public void Stop()
        {
            _stopped = true;
            _signal.Set();
        }
    }
}