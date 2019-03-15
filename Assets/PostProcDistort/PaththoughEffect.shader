//屏幕收缩效果
//by：puppet_master
Shader "Custom/PaththoughEffect"
 {
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	CGINCLUDE
	uniform sampler2D _MainTex;
	uniform float _DistortFactor;	//扭曲强度
	uniform float4 _DistortCenter;	//扭曲中心点xy值（0-1）屏幕空间
	#include "UnityCG.cginc"
 
	fixed4 frag(v2f_img i) : SV_Target
	{
		//计算偏移的方向
		float2 dir = i.uv - _DistortCenter.xy;
		//最终偏移的值：方向 * （1-长度），越靠外偏移越小
		float2 offset = _DistortFactor * normalize(dir) * (1 - length(dir));
		//计算采样uv值：正常uv值+从中间向边缘逐渐增加的采样距离
		float2 uv = i.uv + offset;
		return tex2D(_MainTex, uv);
	}
	ENDCG
 
	SubShader 
	{
		Pass
		{
			ZTest Always
			Cull Off 
			ZWrite Off
			Fog { Mode off }
			
			//调用CG函数	
			CGPROGRAM
			//使效率更高的编译宏
			#pragma fragmentoption ARB_precision_hint_fastest 
			//vert_img是在UnityCG.cginc中定义好的，当后处理vert阶段计算常规，可以直接使用自带的vert_img
			#pragma vertex vert_img
			#pragma fragment frag 
			ENDCG
		}
	}
}
