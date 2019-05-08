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
		_DissolveThreshold("DissolveThreshold", Float) = 0  
		_EdgeWidth("EdgeWidth", Range(-1, 1)) = 0.05  
	}
	
	CGINCLUDE
	#include "Lighting.cginc"
	uniform fixed4 _Diffuse;
	uniform fixed4 _DissolveColor;
	uniform sampler2D _MainTex;
	uniform float4 _MainTex_ST;
	uniform float _DissolveThreshold;  
	uniform float _EdgeWidth;  
	
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
		float isShow = step(i.objPos.x, _DissolveThreshold);
		clip(1 - isShow - 0.0001); // - 0.0001 确保值会小于0, 不然等于0时不会clip

		fixed3 mainClr = tex2D(_MainTex, i.uv).rgb;

		float edgeSmooth = 1 - smoothstep(i.objPos.x, _DissolveThreshold, _DissolveThreshold - _EdgeWidth);
		_DissolveColor *= edgeSmooth; 
		return fixed4(mainClr, 1) + _DissolveColor;
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
