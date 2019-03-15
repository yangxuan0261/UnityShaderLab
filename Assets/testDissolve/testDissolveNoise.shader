// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ITS/test/DissolveNoise" 
{
	Properties {
		_MainTex ("Main tex", 2D) = "white" {}
		_NoiseTex ("Noise tex", 2D) = "white" {}
		_BurnAmout ("Burn Amount", Range(0.0, 1.0)) = 0.0
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
	
	sampler2D _MainTex;
	sampler2D _NoiseTex;
	float _BurnAmout;

	float4 _MainTex_ST;
	float4 _NoiseTex_ST;

	struct v2f {
		float4 pos : SV_POSITION;
		float2 uvMain : TEXCOORD0;
		float2 uvNoise : TEXCOORD1;
	};

	v2f vert(appdata_base v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uvMain = TRANSFORM_TEX(v.texcoord, _MainTex);
		o.uvNoise = TRANSFORM_TEX(v.texcoord, _NoiseTex);
		return o;
	}

	fixed4 frag(v2f i) : SV_TARGET {
		fixed4 clr = tex2D(_MainTex, i.uvMain);
		fixed3 burn = tex2D(_NoiseTex, i.uvNoise).rgb;
		clip(burn.r - _BurnAmout);

		return clr;
	}
	ENDCG

	SubShader {
		Pass {
			Tags { "RenderType" = "Opaque"}		

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
}
