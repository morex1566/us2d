using UnityEngine;

public static class HTTP
{
    public static class ContentType
    {
        public static readonly string HTML = "text/html; charset=utf-8";
    }

    private static readonly string closeTabResponse = @"
    <html>
        <head><title>Login Success</title></head>
        <body onload='window.open(""about:blank"",""_self"").close(); window.close();'>
            <div style='text-align:center; margin-top:20%; font-family:sans-serif;'>
                <h1>로그인 완료!</h1>
                <p>창이 자동으로 닫히지 않으면 직접 닫아주세요.</p>
            </div>
            <script>
                // 여러 가지 방법으로 창 닫기 시도
                window.open('', '_self', ''); 
                window.close();
            </script>
        </body>
    </html>";

    public static byte[] GetCloseTabResponseBuffer() => System.Text.Encoding.UTF8.GetBytes(closeTabResponse);
}
