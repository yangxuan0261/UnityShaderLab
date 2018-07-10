// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unlit shader. Simplest possible textured shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "ITS/test/Unlit-Texture-Dissolve" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Noise ("Noise (RGB)", 2D) = "white" {}
	_MainColor ("Color", Color) = (1, 1, 1, 1)
	_BurnAmout ("Burn Amount", Range(0.0, 1.0)) = 0.0
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 100
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				float2 uvNoise : TEXCOORD1;

				UNITY_FOG_COORDS(1)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainColor;
			fixed _BurnAmout;

			sampler2D _Noise;
			float4 _Noise_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uvNoise = TRANSFORM_TEX(v.texcoord, _Noise);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 val = tex2D(_Noise, i.uvNoise);
				clip(val.r - _BurnAmout); // 

				fixed4 col = tex2D(_MainTex, i.texcoord);
				col *= _MainColor;
				UNITY_APPLY_FOG(i.fogCoord, col);
				UNITY_OPAQUE_ALPHA(col.a);
				return col;
			}
		ENDCG
	}
}

}
