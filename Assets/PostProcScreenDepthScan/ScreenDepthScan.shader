//puppet_master
//https://blog.csdn.net/puppet_master
//2018.5.27  
//基于深度的扫描效果
Shader "DepthTexture/ScreenDepthScan" 
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
	sampler2D _CameraDepthTexture;
	sampler2D _MainTex;
	fixed4 _ScanLineColor;
	float _ScanValue;
	float _ScanLineWidth;
	float _ScanLightStrength;
	
	float4 frag_depth(v2f_img i) : SV_Target
	{
		float depthTextureValue = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
		float linear01EyeDepth = Linear01Depth(depthTextureValue);
		fixed4 screenTexture = tex2D(_MainTex, i.uv);
		
		if (linear01EyeDepth > _ScanValue && linear01EyeDepth < _ScanValue + _ScanLineWidth)
		{
			return screenTexture * _ScanLightStrength * _ScanLineColor;
		}
		return screenTexture;
	}
	ENDCG
 
	SubShader
	{
		Pass
		{
			ZTest Off
			Cull Off
			ZWrite Off
			Fog{ Mode Off }
 
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag_depth
			ENDCG
		}
	}
}
