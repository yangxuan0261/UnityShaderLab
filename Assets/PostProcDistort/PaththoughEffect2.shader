//屏幕收缩效果
//by：puppet_master
Shader "Custom/PaththoughEffect2"
 {
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_NoiseTex("Noise", 2D) = "black"{}
	}
	
	CGINCLUDE
	uniform sampler2D _MainTex;
	uniform sampler2D _NoiseTex;
	uniform float _DistortFactor;	//扭曲强度
	uniform float4 _DistortCenter;	//扭曲中心点xy值（0-1）屏幕空间
	uniform float _DistortStrength; 
	#include "UnityCG.cginc"
 
	fixed4 frag(v2f_img i) : SV_Target
	{
		//计算偏移的方向
		float2 dir = i.uv - _DistortCenter.xy;
		//最终偏移的值：方向 * （1-长度），越靠外偏移越小
		float2 scaleOffset = _DistortFactor * normalize(dir) * (1 - length(dir));
		//采样Noise贴图
		fixed4 noise = tex2D(_NoiseTex, i.uv);
		//noise的权重 = 参数 * 距离，越靠近外边的部分，扰动越严重
		float2 noiseOffset = noise.xy * _DistortStrength * dir;
		//计算最终offset = 两种扭曲offset的差（取和也行，总之效果好是第一位的）
		float2 offset = scaleOffset - noiseOffset;
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
