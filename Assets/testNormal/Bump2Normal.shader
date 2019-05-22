Shader "test/Bump2Normal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Offset ("Offset", Float) = 0.5
		_Strength ("Strength", Float) = 10

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

            float _Offset;
            float _Strength;

            // sampler2D _MainTex;
            float4 _MainTex_ST;

            UNITY_DECLARE_TEX2D(_MainTex);

            // void Unity_NormalFromTexture_float(Texture texture, SamplerState Sampler, float2 UV, float Offset, float Strength, out float3 Out)
            // {
            //     Offset = pow(Offset, 3) * 0.1;
            //     float2 offsetU = float2(UV.x + Offset, UV.y);
            //     float2 offsetV = float2(UV.x, UV.y + Offset);
            //     float normalSample = Texture.Sample(Sampler, UV);
            //     float uSample = Texture.Sample(Sampler, offsetU);
            //     float vSample = Texture.Sample(Sampler, offsetV);
            //     float3 va = float3(1, 0, (uSample - normalSample) * Strength);
            //     float3 vb = float3(0, 1, (vSample - normalSample) * Strength);
            //     Out = normalize(cross(va, vb));
                
            // }

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float offset = _Offset;
                float Strength = _Strength;
                float3 Out = float3(0, 0, 0);
                float2 offsetUV = i.uv;


                offset = pow(offset, 3) * 0.1;
                float2 offsetU = float2(offsetUV.x + offset, offsetUV.y);
                float2 offsetV = float2(offsetUV.x, offsetUV.y + offset);
                float normalSample = UNITY_SAMPLE_TEX2D(_MainTex, offsetUV);
                float uSample = UNITY_SAMPLE_TEX2D(_MainTex, offsetU);
                float vSample = UNITY_SAMPLE_TEX2D(_MainTex, offsetV);
                float3 va = float3(1, 0, (uSample - normalSample) * Strength);
                float3 vb = float3(0, 1, (vSample - normalSample) * Strength);
                Out = normalize(cross(va, vb));

                // fixed4 col = tex2D(_MainTex, i.uv);
                return fixed4(Out, 1);
            }
            ENDCG
        }
    }
}
