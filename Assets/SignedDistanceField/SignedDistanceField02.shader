Shader "test/sdf/SignedDistanceField02"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Width ("Width", Range(0.0001, 15)) = 5
    }
    SubShader
    {
        Tags { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
        }

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            // #define fwidth(x) (abs(ddx(x)) + abs(ddy(x))) // fwidth 是 dx11 的函数, 可以这样定义才能使用到其它平台

            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                // float2 texcoord2 : TEXCOORD1; 可以考虑用多一套uv
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Width;

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f IN) : COLOR {
                float smoothValue = 0;
                //fwidth 必须dx11. 因为是UI，可以用CPU算好一个像素的_Delta,用uniform传给GPU;
                float delta = _Width * fwidth(IN.uv.x);//fwidth(IN.uv.x);
                float v = IN.uv.x + delta;
                //if(v >= 1.0f){
                    // smoothValue = (v -1) / delta;
                //}
                smoothValue = step(1.0, v) * (v -1) / delta;
                float a = smoothstep(1, 0.0f, smoothValue);
                return float4(1, 1, 1, a);
            }
            ENDCG
        }
    }
}
