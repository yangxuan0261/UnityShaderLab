
Shader "test/PostProcDepth"  {
	Properties {
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
	sampler2D _CameraDepthTexture;
	
	float4 frag_depth(v2f_img i) : SV_Target {
		float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv); // 深度图里存放了[0,1]范围的非线性分布的深度值，这些深度值来自NDC坐标。
		float lnr01Depth = Linear01Depth(depth);
		
		return fixed4(lnr01Depth, lnr01Depth, lnr01Depth, 1);
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
