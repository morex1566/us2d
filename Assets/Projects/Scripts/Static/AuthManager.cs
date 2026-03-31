using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Common;

namespace US2D.Auth
{
    public class AuthManager : MonoBehaviourSingleton<AuthManager>
    {
        [Header("Authentication Events")]
        public UnityEvent<AuthResult> OnLoginSuccess = new();
        public UnityEvent<string> OnLoginFailed = new();

        private Dictionary<AuthType, IAuthService> authServices = new();

        private void Awake()
        {
            // 4�ܰ迡�� ������ ���񽺵��Դϴ�.
            // authServices[AuthType.Google] = new GoogleAuthServer(); 
            // authServices[AuthType.Guest] = new GuestAuthService();
        }

        public void Login(AuthType type)
        {
            Debug.Log($"[AuthManager] Starting login with: {type}");

            if (authServices.TryGetValue(type, out var service))
            {
                service.Login(result =>
                {
                    // ���⼭ ������ IdToken�� ���� �����Դϴ�.
                    if (result.IsSuccess)
                    {
                        Debug.Log($"[AuthManager] Login Success! User ID: {result.UserId}");
                        OnLoginSuccess?.Invoke(result);
                    }
                    else
                    {
                        Debug.LogError($"[AuthManager] Login Failed: {result.ErrorMessage}");
                        OnLoginFailed?.Invoke(result.ErrorMessage);
                    }
                });
            }
            else
            {
                Debug.LogWarning($"[AuthManager] {type} service not initialized yet.");
                // �׽�Ʈ�� ���� ���� �̺�Ʈ ȣ��
                OnLoginFailed?.Invoke("Service not implemented");
            }
        }
    }
}
