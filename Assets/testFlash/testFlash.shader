// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//流光效果
//by:puppet_master
//2017.7.30
Shader "ITS/test/FlashEffect" 
{
	Properties
	{
		_MainTex("MainTex(RGB)", 2D) = "white" {}
		_FlashTex("FlashTex", 2D) = "black" {}
		_FlashColor("FlashColor",Color) = (1,1,1,1)
		_FlashFactor("FlashFactor", Vector) = (0, 1, 0.5, 0.5)
		_FlashStrength ("FlashStrength", Range(0, 5)) = 1
	}
	
	CGINCLUDE
	#include "Lighting.cginc"
	uniform sampler2D _MainTex;
	uniform float4 _MainTex_ST;
	uniform sampler2D _FlashTex;
	uniform fixed4 _FlashColor;
	//改为一个vector4，减少传参次数消耗
	uniform fixed4 _FlashFactor;
	uniform fixed _FlashStrength;

	struct v2f 
	{
		float4 pos : SV_POSITION;
		float3 worldNormal : NORMAL;
		float2 uv : TEXCOORD0;
		float3 worldLight : TEXCOORD1;
		float4 worldPos : TEXCOORD2;
	};

	v2f vert(appdata_base v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
		//顶点转化到世界空间
		o.worldPos = v.vertex; // 使用模型空间的坐标 // mul(unity_ObjectToWorld, v.vertex);
		// o.worldNormal = UnityObjectToWorldNormal(v.normal);
		// o.worldLight = UnityObjectToWorldDir(_WorldSpaceLightPos0.xyz);
		return o;
	}
			
	fixed4 frag(v2f i) : SV_Target
	{
		// half3 normal = normalize(i.worldNormal);
		// half3 light = normalize(i.worldLight);
		// fixed diff = max(0, dot(normal, light));
		fixed4 albedo = tex2D(_MainTex, i.uv);
		//通过时间偏移模型坐标对flashTex进行采样
		half2 flashuv = i.worldPos.yx * _FlashFactor.zw - _FlashFactor.xy * _Time.y;
		fixed4 flash = tex2D(_FlashTex, flashuv) * _FlashColor * _FlashStrength;
		fixed4 c;
		//将flash图与原图叠加
		// c.rgb = diff * albedo + flash.rgb;
		c.rgb = albedo + flash.rgb;
		c.a = 1;
		return c;
	}
	ENDCG

	SubShader
	{
		
		Pass
		{
			Tags{ "RenderType" = "Opaque" }
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG	
		}
	}
	FallBack "Diffuse"
}
