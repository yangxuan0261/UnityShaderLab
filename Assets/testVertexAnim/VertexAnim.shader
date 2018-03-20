Shader "ITS/test/VertexAnim"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Sp ("Speed", Float) = 1
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
			float _Sp;
			
			v2f vert (appdata v)
			{
				v2f o;
				float offsetY=sin(v.vertex.x * 0.5 + _Time.y * _Sp); // 乘以0.5偏移会更加平滑
				o.vertex.xyz=float3(v.vertex.x,v.vertex.y+offsetY,v.vertex.z);
				o.vertex = UnityObjectToClipPos(o.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
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
