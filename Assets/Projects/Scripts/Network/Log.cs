using UnityEngine;
using System;
using System.Reflection;

namespace US2D.Network
{
    public class CustomLogHandler : ILogHandler
    {
        private readonly ILogHandler m_DefaultHandler = Debug.unityLogger.logHandler;

        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            // 1. 시간 및 로그 레벨 설정
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string level = logType.ToString().ToUpper();

            // 2. 메시지 조립
            string message = string.Format(format, args);

            // 3. spdlog 스타일 패턴 적용: [%Y-%m-%d %H:%M:%S.%e] [%l] %v
            // (참고: C# 환경에서 호출 파일/라인은 StackTrace 사용 시 성능 비용이 발생함)
            string finalMessage = $"[{timestamp}] [{level}] {message}";

            m_DefaultHandler.LogFormat(logType, context, "{0}", finalMessage);
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
            m_DefaultHandler.LogException(exception, context);
        }
    }

    // 초기화 스크립트
    public static class LogInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            Debug.unityLogger.logHandler = new CustomLogHandler();
        }
    }
}
