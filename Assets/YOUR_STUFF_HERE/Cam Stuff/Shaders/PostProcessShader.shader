Shader "Unlit/PostProcessShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}

        _BackgroundTex("BackgroundTex", 2D) = "blue" {}

        _FogStart("FogStart",Float) = 0.4
        _FogFull("FogFull",Float) = 0.75
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            sampler2D _BackgroundTex;

            float4 _MainTex_ST;

            float _FogStart;
            float _FogFull;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 DepthVal1 = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv));

                float HoriDiff = _FogFull - _FogStart;
                float LocalHoriVal = DepthVal1 - _FogStart;
                float BackMulti = max(LocalHoriVal / HoriDiff,0.0);
                BackMulti = clamp(BackMulti, 0.0, 1.0);
                BackMulti = sqrt(BackMulti);

                fixed4 MainCol = tex2D(_MainTex, i.uv);
                fixed4 BackCol = tex2D(_BackgroundTex, i.uv);                             

                //return MainCol;
                //return fixed4(DepthVal1.x, DepthVal1.x, DepthVal1.x, 1.0);
                return lerp(BackCol, MainCol, 1.0 - BackMulti);
            }
            ENDCG
        }
    }
}
