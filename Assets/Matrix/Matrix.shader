Shader "Unlit/Matrix"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _right ("right", Vector) = (1, 1, 1, 1)
        _up ("up", Vector) = (1, 1, 1, 1)
        _forward ("right", Vector) = (1, 1, 1, 1)
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float3 _right;
            float3 _up;
            float3 _forward;

            v2f vert (appdata v) {
                v2f o;
                // 方式一: 一步到位
                // o.vertex = UnityObjectToClipPos(v.vertex);

                // 方式二: 自己变换空间
                // float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                // 构建世界矩阵
                float4x4 worldMatrix = float4x4(_right, 0, _up, 0, _forward, 0, 0, 0, 0, 1); // 构建 世界空间矩阵
                float4 worldPos = mul(worldMatrix, v.vertex);
                float4 viewPos = mul(UNITY_MATRIX_V, worldPos);
                o.vertex = mul(UNITY_MATRIX_P, viewPos);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
