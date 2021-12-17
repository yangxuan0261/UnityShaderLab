Shader "test/testIf"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Height ("Height", float) = 0.5
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

            float _Height;

            static const float _ConstHeight = 0.2;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                // if (i.uv.y < _Height) {
                //     col *= fixed4(1, 0, 0, 1);
                // } else {
                //     col *= fixed4(0, 1, 0, 1);
                // }

                float a = 2.0;
                // if (a < _ConstHeight) {
                if (i.uv.y < _ConstHeight) {
                    col *= fixed4(1, 0, 0, 1);
                } else {
                    col *= fixed4(0, 1, 0, 1);
                }

                /* 
                float isLt = step(i.uv.y, _ConstHeight);
                col *= isLt * fixed4(1, 0, 0, 1) + (1-isLt) * fixed4(0, 1, 0, 1);
                */     

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
