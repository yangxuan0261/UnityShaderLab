Shader "ProceduralNoise/SimpleNoise"  {
	Properties {
		_Scale ("Scale", Range(1, 100)) = 1
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
	#include "ProceduralNoise.cginc"
	float _Scale ; 
	
	fixed4 frag(v2f_img i) : SV_Target {
		float OutVal = 1;
		Unity_SimpleNoise_float(i.uv, _Scale, OutVal);
		return fixed4(OutVal, OutVal, OutVal,  1);
	}
	ENDCG

	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			ENDCG
		}
	}
}
