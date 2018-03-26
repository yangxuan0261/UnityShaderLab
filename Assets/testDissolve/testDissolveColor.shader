// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//消失效果
//by：puppet_master
//2017.8.11

Shader "ITS/test/DissolveColor" 
{
	Properties{
		_Diffuse("Diffuse", Color) = (1,1,1,1)
		_DissolveColor("Dissolve Color", Color) = (0,0,0,0)
		_MainTex("Base 2D", 2D) = "white"{}
		_ColorFactor("ColorFactor", Range(0,1)) = 0.7
		_DissolveThreshold("DissolveThreshold", Float) = 0  
	}
	
	CGINCLUDE
	#include "Lighting.cginc"
	uniform fixed4 _Diffuse;
	uniform fixed4 _DissolveColor;
	uniform sampler2D _MainTex;
	uniform float4 _MainTex_ST;
	uniform float _ColorFactor;
	uniform float _DissolveThreshold;  
	
	struct v2f
	{
		float4 pos : SV_POSITION;
		float3 worldNormal : TEXCOORD0;
		float2 uv : TEXCOORD1;
		float4 objPos : TEXCOORD2; 
	};
	
	v2f vert(appdata_base v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
		o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
		o.objPos = v.vertex;  
		return o;
	}
	
	fixed4 frag(v2f i) : SV_Target
	{
		float factor = i.objPos.x - _DissolveThreshold;
		clip(factor); 
		fixed3 color = tex2D(_MainTex, i.uv).rgb;
		//等价于下面注释代码的操作
		fixed lerpFactor = saturate(sign(_ColorFactor - factor));
		return lerpFactor * _DissolveColor + (1 - lerpFactor) * fixed4(color, 1);
		/*
		if (factor < _ColorFactor)
		{
			return _DissolveColor;
		}
		return fixed4(color, 1);*/
	}
	ENDCG
	
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		Pass
		{
			//不让模型穿帮，关掉了背面裁剪
			// Cull Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag	
			ENDCG
		}
	}
	FallBack "Diffuse"
}
