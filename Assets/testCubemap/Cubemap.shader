Shader "Custom/Cubemap"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Cubemap("Diffuse Convolution Cubemap", Cube) = ""{}
		_Amount("Diffuse Amount", Range(0,10)) = 1
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
                float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldRef : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float3 worldViewDir : TEXCOORD3;
				float2 uv : TEXCOORD4;
				UNITY_FOG_COORDS(1)
            };

			sampler2D _MainTex;
			float4 _MainTex_ST;
			samplerCUBE _Cubemap;
			float _Amount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldNormal = mul(unity_ObjectToWorld, v.normal);
                o.worldViewDir =  normalize(_WorldSpaceCameraPos.xyz - o.worldPos.xyz); // 这里用 观察向量 模拟入射光, 实际应该用 入射光向量
				// o.worldViewDir =normalize(UnityWorldSpaceLightDir(o.worldPos)); // 顶点到光源的向量, 反方向则为 入射光向量
                o.worldRef = reflect(-o.worldViewDir,normalize(o.worldNormal));
                return o;
            }
			
			fixed4 frag (v2f i) : SV_Target
            {
				fixed4 colMain = tex2D(_MainTex, i.uv);
                fixed4 colCube = texCUBE(_Cubemap, i.worldRef) * _Amount;
				UNITY_APPLY_FOG(i.fogCoord, col);

                return colMain*colCube;
            }
			ENDCG
		}
	}
}
