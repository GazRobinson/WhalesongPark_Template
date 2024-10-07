// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/MainMatShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        _LightMinus("Light Max Minus", Float) = 0.1
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
                half3 objNormal : NORMAL;
            };

            struct v2f
            {
                half3 worldNormal : NORMAL;
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _LightDir;

            float _LightMinus;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = mul(float4(v.objNormal, 0.0), unity_WorldToObject).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 LightDir = float3(_LightDir.x,_LightDir.y,_LightDir.z);

                float BW = 1.0f - (_LightMinus * ((dot(LightDir, i.worldNormal) + 2.0) / 2.0));

                // sample the texture
                //float BW = tex2D(_MainTex, i.uv).x - (_LightMinus * ((dot(LightDir,i.worldNormal) + 2) / 2));
                // apply fog

                //return fixed4(LightDir.x, LightDir.y, LightDir.z, 1.0);
                //return fixed4(i.worldNormal.x, i.worldNormal.y,i.worldNormal.z, 1.0);
                return fixed4(BW, BW, BW, 1.0);
            }
            ENDCG
        }
    }
}
