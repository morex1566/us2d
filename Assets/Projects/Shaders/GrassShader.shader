Shader "Custom/GrassShader"
{
    Properties
    {
        [MainTexture] _BaseMap("Grass Texture (Sprite)", 2D) = "white" {}
        _WindSpeed("Wind Speed", Float) = 3.0
        _WindStrength("Wind Strength", Float) = 0.1
        _PixelSize("Pixel Snap Density", Float) = 16.0
        _Cutoff("Alpha Cutoff", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType"="TransparentCutout" "Queue"="AlphaTest" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
                float4 color        : COLOR;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float4 color        : COLOR;
            };

            sampler2D _BaseMap;
            float _WindSpeed;
            float _WindStrength;
            float _PixelSize;
            float _Cutoff;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                // 픽셀 아트 느낌을 위해 시간을 끊어서 계산 (Quantized Time)
                float time = floor(_Time.y * _WindSpeed) / _WindSpeed;
                
                // 좌우 흔들림 계산
                float wind = sin(time) * _WindStrength;
                
                // 풀의 윗부분(UV.y가 높은 곳)만 흔들리도록 설정
                // 픽셀 단위로 끊기게 wind 값을 가공
                float snappedWind = floor(wind * _PixelSize) / _PixelSize;
                
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                positionWS.x += snappedWind * IN.uv.y;

                OUT.positionCS = TransformWorldToHClip(positionWS);
                OUT.uv = IN.uv;
                OUT.color = IN.color;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 texColor = tex2D(_BaseMap, IN.uv) * IN.color;
                
                // 투명도 처리 (픽셀 아트는 보통 Cutout 방식 사용)
                clip(texColor.a - _Cutoff);
                
                return texColor;
            }
            ENDHLSL
        }
    }
}