Shader "Custom/DebugRedURP"
{
    // URP 환경임을 명시
    Properties
    { }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // URP 필수 라이브러리
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
            };

            // 정점 변환
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            // 색상 출력 (R, G, B, A)
            half4 frag(Varyings IN) : SV_Target
            {
                // 완전한 빨간색 반환
                return half4(1.0, 0.0, 0.0, 1.0);
            }
            ENDHLSL
        }
    }
}