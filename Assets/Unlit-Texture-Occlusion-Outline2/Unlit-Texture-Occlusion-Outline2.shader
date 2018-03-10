// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unlit shader. Simplest possible textured shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Custom/Unlit-Texture-Occlusion-Outline2" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_OutlineColor("Outline Color", Color) = (1,1,0,1)
		_Outline("Outline width", Range(0.0, 0.5)) = 0.03
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	struct appdata_t {
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
		float3 normal : NORMAL;
	};

	struct v2f {
		float4 vertex : SV_POSITION;
		half2 texcoord : TEXCOORD0;
	};

	sampler2D _MainTex;
	float4 _MainTex_ST;
	float _Outline;
	float4 _OutlineColor;

	v2f vert(appdata_t v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

		return o;
	}

	v2f vert_outline(appdata_t v)
	{
		v2f o = (v2f)0;
		//o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		//float3 norm = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
		//float2 offset = TransformViewToProjection(norm.xy); //float2 offset =  mul((float2x2)UNITY_MATRIX_P, norm.xy);
		//o.vertex.xy += offset * o.vertex.z * _Outline;

		float4 viewPos = mul(UNITY_MATRIX_MV, v.vertex);
		float3 viewNorm = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
		float3 offset = normalize(viewNorm) * _Outline;
		viewPos.xyz += offset;
		o.vertex = mul(UNITY_MATRIX_P, viewPos);

		return o;
	}

	ENDCG

	SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Opaque" }


	Pass{
		Stencil
		{
			Ref 1
			Comp Always
			Pass Replace
			ZFail Replace
		}

		ZTest LEqual
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag


		fixed4 frag(v2f i) : SV_Target
		{
			fixed4 col = tex2D(_MainTex, i.texcoord);
			return col;
		}

		ENDCG
	}

	//Pass{
	//	ZTest Greater
	//	ZWrite Off
	//	Blend SrcAlpha OneMinusSrcAlpha

	//	Stencil
	//	{
	//		Ref 1
	//		Comp Always
	//		Pass Replace
	//	}

	//	CGPROGRAM
	//	#pragma vertex vert
	//	#pragma fragment frag

	//	fixed4 frag(v2f i) : SV_Target
	//	{
	//		return fixed4(0,0,0,0);
	//		//fixed4 col = tex2D(_MainTex, i.texcoord);
	//		//return col;
	//	}

	//	ENDCG
	//}


	Pass{
		ZTest Greater
		ZWrite Off
		//Blend DstAlpha OneMinusDstAlpha

		Stencil{
			Ref 1
			Comp NotEqual

		}
		CGPROGRAM
		#pragma vertex vert_outline
		#pragma fragment frag
		half4 frag(v2f i) :COLOR
		{
			return _OutlineColor;
		}
		ENDCG
	}
}

}

