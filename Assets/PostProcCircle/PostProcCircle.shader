Shader "test/PostProcCircle"  {
	Properties {
		_MainTex("Base (RGB)", 2D) = "white" {}
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"

	sampler2D _MainTex;
	
	float _Radius;
	float _Width;
	float2 _Center;
	fixed4 _MainColor;
	
	float4 frag_circle(v2f_img i) : SV_Target {
		float4 col = tex2D(_MainTex, i.uv);

		float dtPos = distance(i.uv, _Center);

		float maskInt = smoothstep(_Radius, dtPos, _Radius - _Width);
		float maskOut = 1 - step(dtPos, _Radius);
		
		return col + _MainColor * (maskInt + maskOut);
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
			#pragma fragment frag_circle
			ENDCG
		}
	}
}
