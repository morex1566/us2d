

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Common
{
    /// <summary>
    /// 고성능 전역 타이머 시스템.
    /// 단일 System.Threading.Timer를 사용하여 모든 네트워크 배치 및 틱 타이머를 관리함으로써 GC 할당을 최소화합니다.
    /// </summary>
    public static class Timer
    {
        public static double GetCurrentMs()
        {
            return _stopwatch.Elapsed.TotalMilliseconds;
        }

        public static double Capture()
        {
            double currMs = GetCurrentMs();
            double gap = currMs - _lastMs;
            _lastMs = currMs;
            return gap;
        }

        /// <summary>
        /// 특정 지연 시간(ms) 후 콜백을 실행하도록 예약합니다.
        /// </summary>
        public static void Schedule(int delayMs, Action<object> callback, object state)
        {
            if (callback == null)
            {
                return;
            }

            ScheduleInternal(delayMs, callback, state);
        }

        private static void ScheduleInternal(int delayMs, Action<object> callback, object state)
        {
            double targetTime = GetCurrentMs() + delayMs;

            lock (_lock)
            {
                TimerTask newTask = new TimerTask
                {
                    executionTime = targetTime,
                    callback = callback,
                    state = state
                };

                // 실행 시간 순으로 정렬 유지하며 삽입 (O(log N))
                int index = _tasks.BinarySearch(newTask);
                {
                    if (index < 0)
                    {
                        index = ~index;
                    }
                }
                _tasks.Insert(index, newTask);

                // 이번에 추가된 태스크가 가장 빠른 실행 건이라면 시스템 타이머 재조정
                if (index == 0)
                {
                    _scheduler.Change(Math.Max(0, delayMs), Timeout.Infinite);
                }
            }
        }

        private static void OnTick(object state)
        {
            double now = GetCurrentMs();

            List<TimerTask> toRun = null;

            lock (_lock)
            {
                int count = 0;
                {
                    // 만료된 태스크들 일괄 추출
                    while (count < _tasks.Count && _tasks[count].executionTime <= now + 0.5)
                    {
                        count++;
                    }
                }

                if (count > 0)
                {
                    toRun = _tasks.GetRange(0, count);
                    _tasks.RemoveRange(0, count);
                }

                // 다음 대기 중인 태스크로 시스템 타이머 예약 (Re-use)
                if (_tasks.Count > 0)
                {
                    double nextDelay = Math.Max(0, _tasks[0].executionTime - GetCurrentMs());
                    _scheduler.Change((int)nextDelay, Timeout.Infinite);
                }
            }

            if (toRun != null)
            {
                foreach (var task in toRun)
                {
                    try
                    {
                        task.callback.Invoke(task.state);
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.LogError($"Timer Callback Error: {ex.Message}");
                    }
                }
            }
        }

        private struct TimerTask : IComparable<TimerTask>
        {
            public double executionTime;

            public Action<object> callback;

            public object state;

            public int CompareTo(TimerTask other)
            {
                return executionTime.CompareTo(other.executionTime);
            }
        }

        private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        private static readonly System.Threading.Timer _scheduler = new System.Threading.Timer(OnTick, null, Timeout.Infinite, Timeout.Infinite);

        private static readonly List<TimerTask> _tasks = new List<TimerTask>();

        private static readonly object _lock = new object();

        private static double _lastMs = 0;
    }
}