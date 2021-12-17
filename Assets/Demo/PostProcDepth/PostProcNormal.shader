
Shader "test/PostProcNormal"  {
	Properties {
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
	sampler2D _CameraDepthNormalsTexture; // 包含深度与法线的纹理
	
	fixed4 frag_depth(v2f_img i) : SV_Target {
		float3 normal = DecodeViewNormalStereo(tex2D(_CameraDepthNormalsTexture, i.uv)); // 观察空间下的法线

		// float depth = 1;
		// float3 normal = 1;
		// DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, i.uv), depth, normal); // DecodeDepthNormal 内部也是调用 DecodeViewNormalStereo

		normal = (normal + 1)/2; // 转到 0~1 范围
		return fixed4(normal, 1);
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
