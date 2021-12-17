// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//按照方向消失的效果
//by:puppet_master
//2017.8.10
Shader "ITS/test/DissolveEffectX" 
{
	Properties
	{
		_MainTex("MainTex(RGB)", 2D) = "white" {}
		_DissolveVector("DissolveVector", Vector) = (0,0,0,0)
	}
	
	CGINCLUDE
	#include "Lighting.cginc"
	uniform sampler2D _MainTex;
	uniform float4 _MainTex_ST;
	uniform float4 _DissolveVector;

	struct v2f 
	{
		float4 pos : SV_POSITION;
		// float3 worldNormal : NORMAL;
		float2 uv : TEXCOORD0;
		// float3 worldLight : TEXCOORD1;
		float4 objPos : TEXCOORD2;
	};

	v2f vert(appdata_base v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
		//模型空间
		o.objPos = v.vertex;
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
		//不满足条件的discard
		clip(i.objPos.xyz - _DissolveVector.xyz); // TODO: 模型坐标系不同，但y轴是一致的
		fixed4 c;
		// c.rgb = diff * albedo;
		c.rgb =  albedo;
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
