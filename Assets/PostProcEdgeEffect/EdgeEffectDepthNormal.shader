/********************************************************************
 FileName: EdgeEffectDepthNormal.shader
 Description: 后处理描边效果，使用DepthNormalTexture检测
 history: 13:11:2018 by puppet_master
 https://blog.csdn.net/puppet_master
*********************************************************************/
Shader "Edge/EdgeEffectDepthNormal"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
	struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};
	
	struct v2f
	{
		float2 uv[5] : TEXCOORD0;
		float4 vertex : SV_POSITION;
	};
	
	sampler2D _MainTex;
	float4 _MainTex_TexelSize;
	sampler2D _CameraDepthNormalsTexture;
	fixed4 _EdgeColor;
	fixed4 _NonEdgeColor;
 
	float _SampleRange;
	float _NormalDiffThreshold;
	float _DepthDiffThreshold;
	
	float CheckEdge(fixed4 s1, fixed4 s2)
	{
		float2 normalDiff = abs(s1.xy - s2.xy);
		float normalEdgeVal = (normalDiff.x + normalDiff.y) < _NormalDiffThreshold;
		
		float s1Depth = DecodeFloatRG(s1.zw);
		float s2Depth = DecodeFloatRG(s2.zw);
		float depthEdgeVal = abs(s1Depth - s2Depth) < 0.1 * s1Depth * _DepthDiffThreshold;
		return depthEdgeVal * normalEdgeVal;
	}
	
	v2f vert (appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv[0] = v.uv + float2(-1, -1) * _MainTex_TexelSize * _SampleRange;
		o.uv[1] = v.uv + float2( 1, -1) * _MainTex_TexelSize * _SampleRange;
		o.uv[2] = v.uv + float2(-1,  1) * _MainTex_TexelSize * _SampleRange;
		o.uv[3] = v.uv + float2( 1,  1) * _MainTex_TexelSize * _SampleRange;
		o.uv[4] = v.uv;
		return o;
	}
	
	fixed4 frag (v2f i) : SV_Target
	{
		fixed4 col = tex2D(_MainTex, i.uv[4]);
		fixed4 s1 = tex2D(_CameraDepthNormalsTexture, i.uv[0]);
		fixed4 s2 = tex2D(_CameraDepthNormalsTexture, i.uv[1]);
		fixed4 s3 = tex2D(_CameraDepthNormalsTexture, i.uv[2]);
		fixed4 s4 = tex2D(_CameraDepthNormalsTexture, i.uv[3]);
		
		float result = 1.0;
		result *= CheckEdge(s1, s4);
		result *= CheckEdge(s2, s3);
		col.rgb = lerp(_EdgeColor, _NonEdgeColor, result);
		return col;
	}
	
	ENDCG
	
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
		
		//Pass 0 Roberts Operator
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
		
		
	}
}
