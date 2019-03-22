Shader "test/testddxddy"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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
                float3 worldPos : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// // sample the texture
				// fixed4 col = tex2D(_MainTex, i.uv);
				// // apply fog
				// UNITY_APPLY_FOG(i.fogCoord, col);
				// return col;
				float3 dx = ddx(i.worldPos);
				float3 dy = ddy(i.worldPos);
				float3 normal = normalize(cross(dx, dy));

				normal = normal * 0.5 + 0.5;
				return fixed4(normal,1.0);
			}
			ENDCG
		}
	}
}
