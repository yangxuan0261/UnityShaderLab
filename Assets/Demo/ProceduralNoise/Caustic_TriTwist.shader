Shader "ProceduralNoise/Caustic_TriTwist"  {
	Properties {
		_TileNum ("TileNum", float) = 1
		_Speed ("Speed", Range(0.0001, 5)) = 1
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
	#include "ProceduralNoise.cginc"
	float _TileNum ; 
	float _Speed ; 
	
	fixed4 frag(v2f_img i) : SV_Target {
		float2 uv = _TileNum * i.uv;
		float time = _Time.y * _Speed;
		float val = CausticTriTwist(uv,time); 
		return fixed4(val,val,val, 1);
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
