//漩涡扭曲效果
//by：puppet_master
Shader "Custom/RotationDistortEffect"
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
		//平移坐标点到中心点,同时也是当前像素点到中心的方向
		fixed2 dir = i.uv - _DistortCenter.xy;
		//计算旋转的角度：对于像素点来说，距离中心越远，旋转越少，所以除以距离。相当于用DistortFactor作为旋转的角度值Distort/180 * π，π/180 = 0.1745
		float rot = _DistortFactor * 0.1745 / (length(dir) + 0.001);//+0.001防止除零
		//计算sin值与cos值，构建旋转矩阵
		fixed sinval, cosval;
		sincos(rot, sinval, cosval);
		float2x2  rotmatrix = float2x2(cosval, -sinval, sinval, cosval);
		//旋转
		dir = mul(dir, rotmatrix);
		//再平移回原位置
		dir += _DistortCenter.xy;
		//采样noise图
		fixed4 noise = tex2D(_NoiseTex, i.uv);
		//noise的权重 = 参数 * 距离，越靠近外边的部分，扰动越严重
		float2 noiseOffset = noise.xy * _DistortStrength * dir;
		//用偏移过的uv+扰动采样MainTex
		return tex2D(_MainTex, dir + noiseOffset);
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
