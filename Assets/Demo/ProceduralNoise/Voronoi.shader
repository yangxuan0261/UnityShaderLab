Shader "ProceduralNoise/Voronoi"  {
	Properties {
		_CellDensity ("CellDensity", float) = 3
		_Speed ("Speed", Range(1, 10)) = 1
		_Power ("Power", Range(1, 20)) = 1
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
	#include "ProceduralNoise.cginc"
	float _CellDensity ; 
	float _Speed ; 
	float _Power ; 
	
	fixed4 frag(v2f_img i) : SV_Target {
		float OutVal = 1;
		float Cells = 1;
		float time = _Time.y * _Speed;
		float2 uv = i.uv;
		Unity_Voronoi_float(uv, time, _CellDensity, OutVal, Cells);
		OutVal = pow(OutVal, _Power);
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
