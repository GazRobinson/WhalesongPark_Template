Shader "Unlit/PostProcessShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}

        _PalleteTex("Pallete Texture", 2D) = "white" {}

        _BackgroundTex("BackgroundTex", 2D) = "blue" {}

        _DitherXSize("DitherXSize", Float) = 0.02
        _DitherYSize("DitherYSize", Float) = 0.01
        _DitherMinus("DitherMinus", Float) = 0.042
        _DitherMaxDiv("Dither Max Divisible",Float) = 2.0

        _FogStart("FogStart",Float) = 0.4
        _FogFull("FogFull",Float) = 0.75

        _Col2DitherMinus("Colour 2 Dither Minus",Float) = 0.05
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
        }

        LOD 100

        Pass
        {
            Name "Unlit"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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

            sampler2D _PalleteTex;

            float4 _MainTex_ST;

            float _DitherXSize;
            float _DitherYSize;
            float _DitherMinus;
            float _DitherMaxDiv;

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
                fixed4 DepthCol = (Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv)));
                float DepthVal1 = DepthCol.w;

                float HoriDiff = _FogFull - _FogStart;
                float LocalHoriVal = DepthVal1 - _FogStart;
                float BackMulti = max(LocalHoriVal / HoriDiff, 0.0);
                BackMulti = clamp(BackMulti, 0.0, 1.0);
                BackMulti = sqrt(BackMulti);

                float ActDitherDiv = DepthVal1 < _FogStart ? 1.0f : (1.0f + ((_DitherMaxDiv - 1.0f) * (1.0 - BackMulti)));

                fixed2 FullDitherSizes = fixed2(_DitherXSize,_DitherYSize);
                fixed2 HalfDitherSizes = FullDitherSizes * 0.5;
                fixed2 DitherHalfToOne = 1.0 / HalfDitherSizes;
                fixed2 DitherMod = fmod(i.uv, FullDitherSizes);
                fixed2 DitherModDiv = DitherMod / ActDitherDiv;
                fixed2 DitherVec = floor(DitherModDiv * DitherHalfToOne);
                float DitherFinal = floor(((DitherVec.x + DitherVec.y) / 2.0));

                float MainBW = tex2D(_MainTex, i.uv).x;
                float BackBW = tex2D(_BackgroundTex, i.uv).x;

                fixed4 FinalBWCol = lerp(MainBW, BackBW, BackMulti);
                
                fixed4 Col1 = tex2D(_PalleteTex, fixed2(MainBW, 0.5));

                if (DepthVal1 > _FogFull)
                {
                    return tex2D(_PalleteTex, fixed2(BackBW, 0.5));
                }
                
                fixed4 Col2 = DepthVal1 < _FogStart ? tex2D(_PalleteTex, fixed2(MainBW - _DitherMinus, 0.5)) : tex2D(_PalleteTex, fixed2(BackBW, 0.5));

                fixed4 FinalUnlitCol = (Col1 * (1.0 - DitherFinal)) + (Col2 * (DitherFinal));

                return FinalUnlitCol;
            }
            ENDCG
        }
    }
}
