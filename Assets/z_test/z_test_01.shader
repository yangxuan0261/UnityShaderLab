Shader "Unlit/z_test_01"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_v1 ("v1", Color) = (1, 1, 1, 1)
	}
	SubShader
	{
		Tags { 
			"Queue"="Transparent+1"
			"RenderType"="Transparent" 
			// "RenderType"="Opaque"
			 }
		LOD 100

		Pass
		{
			Cull Off
			// ZTest Greater
			// ZTest LEqual
			// ZWrite On
			// AlphaTest 
			// Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				fixed4 clr : COLOR;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _v1;
			
			v2f vert (appdata v)
			{
				v2f o;
				// o.vertex = UnityObjectToClipPos(v.vertex);

				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				// float4 worldPos = mul(v.vertex, unity_WorldToObject);
				float4 viewPos = mul(UNITY_MATRIX_V, worldPos);
				o.vertex = mul(UNITY_MATRIX_P, viewPos);

				// o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv = v.texcoord;

				o.clr = _v1;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				col *= i.clr;
				// return i.clr;
				return col;
			}
			ENDCG
		}
	}
}
