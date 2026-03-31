using System;
using UnityEngine;

public enum AuthType 
{ 
    Google, 
    Guest 
}

[System.Serializable]
public class AuthResult
{
    public bool IsSuccess;
    public string UserId;      // ���� ���� ID (sub)
    public string DisplayName; // ����� �̸�
    public string IdToken;     // ���� ������ �ٽ� ��ū
    public string ErrorMessage;
}

public interface IAuthService
{
    void Login(Action<AuthResult> callback);
    void Logout();
}