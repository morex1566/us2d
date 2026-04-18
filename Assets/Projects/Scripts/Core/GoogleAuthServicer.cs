using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class GoogleAuthServicer : UIBehaviour
{
    private HttpListener listener;

    protected override void Awake()
    {
        listener = new HttpListener();
    }

    public void AsyncLogin()
    {
        listener.Prefixes.Add(Auth.Google.RedirectUri);
        listener.Start();

        Application.OpenURL(Auth.Google.AuthUrl);

        listener.GetContextAsync().ContinueWith(OnGetContextCompleted);
    }

    public void AsyncLogout()
    {

    }

    private void OnGetContextCompleted(Task<HttpListenerContext> task)
    {
        var context = task.Result;
        string code = context.Request.QueryString.Get("code");

        // context(브라우저)에 전달할 내용 및 설정
        byte[] contentBuffer = HTTP.GetCloseTabResponseBuffer();
        context.Response.ContentLength64 = contentBuffer.Length;
        context.Response.ContentType = HTTP.ContentType.HTML;
        context.Response.OutputStream.Write(contentBuffer, 0, contentBuffer.Length);
        context.Response.OutputStream.Close();

        listener.Stop();

        Debug.Log("<color=green>로그인 성공! 인증 코드: </color>" + code);
    }
}
