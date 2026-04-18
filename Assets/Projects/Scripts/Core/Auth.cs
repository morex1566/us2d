using System;
using UnityEngine;
using UnityEngine.Networking;

public enum AuthType 
{ 
    Google, 
    Guest 
}

[System.Serializable]
public class AuthResult
{
    public bool IsSuccess;
    public string UserId;      
    public string DisplayName; 
    public string IdToken;    
    public string ErrorMessage;
}

public static class Auth
{
    public static class Google
    {
        public static readonly string ClientId = "1037531147924-tomb4bmakelqcuht2igfb9eomegacsso.apps.googleusercontent.com";
        public static readonly int Port = 5000;
        public static readonly string RedirectUri = $"http://localhost:{Port}/";
        public static readonly string EncodedRedirectUri = UnityWebRequest.EscapeURL(RedirectUri);
        public static readonly string AuthUrl = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                                                $"client_id={ClientId}&" +
                                                $"response_type=code&" +
                                                $"scope=openid%20email%20profile&" +
                                                $"redirect_uri={EncodedRedirectUri}";
    }
}
