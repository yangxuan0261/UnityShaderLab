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
	
	float4 frag_depth(v2f_img i) : SV_Target {
		float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
		float lnr01Depth = Linear01Depth(depth);
		fixed4 screenTexture = tex2D(_MainTex, i.uv);

		float near = smoothstep(_ScanValue, lnr01Depth, _ScanValue - _ScanLineWidth);
		float far = smoothstep(_ScanValue, lnr01Depth, _ScanValue + _ScanLineWidth);
		fixed4 emissionClr = _ScanLineColor * (near + far);
		
		return screenTexture + emissionClr;
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
