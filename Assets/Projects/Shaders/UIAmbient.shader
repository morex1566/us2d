Shader "Custom/UI/UIAmbient"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _AmbientColor ("Ambient Color", Color) = (0,0,0,1) // 기본 검정색
        _PixelSize ("Pixel Density", Range(2, 256)) = 32 // 픽셀화 정도
        _Intensity ("Ambient Intensity", Range(0, 5)) = 1.5
    }
    
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4 _AmbientColor;
            float _PixelSize;
            float _Intensity;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 1. UV 픽셀화
                float2 pixelUV = floor(i.texcoord * _PixelSize) / _PixelSize;

                // 2. 중심으로부터의 방사형 거리 계산 (픽셀화된 UV 기준)
                float dist = distance(pixelUV, float2(0.5, 0.5));

                // 3. 앰비언트 강도 조절
                float ambientFactor = saturate(dist * _Intensity);

                // 4. UI 텍스처와 앰비언트 컬러 합성
                half4 texColor = tex2D(_MainTex, i.texcoord) * i.color;
                
                // 외곽으로 갈수록 Ambient Color가 덮어씌워짐
                half3 finalRGB = lerp(texColor.rgb, _AmbientColor.rgb, ambientFactor * _AmbientColor.a);
                
                return half4(finalRGB, texColor.a);
            }
            ENDCG
        }
    }
}