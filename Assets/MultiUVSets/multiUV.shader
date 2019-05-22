Shader "test/multiUV" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        [KeywordEnum(UV0, UV1, UV2)] _UVSet ("UV Set for textures", Float) = 0
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile _UVSET_UV0 _UVSET_UV1 _UVSET_UV2

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 texUV0 : TEXCOORD0;
                float2 texUV1 : TEXCOORD1;
                float2 texUV2 : TEXCOORD2;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half _UVSet;

            v2f vert (appdata v) {
                v2f o;
                
                #if _UVSET_UV0
                    float2 dstUv = v.texUV0;
                #elif _UVSET_UV1
                    float2 dstUv = v.texUV1;
                #else
                    float2 dstUv = v.texUV2;
                #endif

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(dstUv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            
            ENDCG
        }
    }
}
