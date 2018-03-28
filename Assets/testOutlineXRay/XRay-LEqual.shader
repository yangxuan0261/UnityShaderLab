// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//X光效果
//by：puppet_master
//2017.6.20

Shader "ITS/test/XRayEffectLEqual"
{
	Properties
	{
		_MainTex("Base 2D", 2D) = "white"{}
		_XRayColor("XRay Color", Color) = (1,1,1,1)
	}

	SubShader
	{
		// Tags{ "Queue" = "Geometry+100" "RenderType" = "Opaque" }
		Tags {"Queue"="Transparent" "RenderType"="Opaque" }
		
		//渲染X光效果的Pass
		Pass
		{
			// Blend SrcAlpha One
			// ZWrite Off
			// ZTest Greater
			// Cull Back

			Blend SrcAlpha One//打开混合模式
            ZWrite off
            Lighting off

			CGPROGRAM
			#include "Lighting.cginc"
			fixed4 _XRayColor;
			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 normal : normal;
				float3 viewDir : TEXCOORD0;
				float4 clr : COLOR;
			};

			// // 方式一: 在片段点中计算，性能较 方式二 差点
			// v2f vert (appdata_base v)
			// {
			// 	v2f o;
			// 	o.pos = UnityObjectToClipPos(v.vertex);
			// 	o.viewDir = ObjSpaceViewDir(v.vertex); //对象空间 的 顶点 到 摄像机 的方向
			// 	o.normal = v.normal;
			// 	return o;
			// }

			// fixed4 frag(v2f i) : SV_Target
			// {
			// 	float3 normal = normalize(i.normal);
			// 	float3 viewDir = normalize(i.viewDir);
			// 	float rim = 1 - dot(normal, viewDir); //夹角越小，点乘x 越小，1-x 强度越大
			// 	return _XRayColor * rim;
			// }

		
			// 方式二: 在顶点中计算好，性能会好一点
			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.viewDir = ObjSpaceViewDir(v.vertex); //对象空间 的 顶点 到 摄像机 的方向
				o.normal = v.normal;

				float3 normal = normalize(v.normal);
				float3 viewDir = normalize(o.viewDir);
				float rim = 1 - dot(normal, viewDir); //夹角越小，点乘x 越小，1-x 强度越大
				o.clr =	_XRayColor * rim;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
	
				return i.clr;
			}

			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}

		// //正常渲染的Pass
		// Pass
		// {
		// 	ZWrite On
		// 	CGPROGRAM
		// 	#include "Lighting.cginc"
		// 	sampler2D _MainTex;
		// 	float4 _MainTex_ST;

		// 	struct v2f
		// 	{
		// 		float4 pos : SV_POSITION;
		// 		float2 uv : TEXCOORD0;
		// 	};

		// 	v2f vert(appdata_base v)
		// 	{
		// 		v2f o;
		// 		o.pos = UnityObjectToClipPos(v.vertex);
		// 		o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
		// 		return o;
		// 	}

		// 	fixed4 frag(v2f i) : SV_Target
		// 	{
		// 		return tex2D(_MainTex, i.uv);
		// 	}

		// 	#pragma vertex vert
		// 	#pragma fragment frag	
		// 	ENDCG
		// }
	}
	
	FallBack "Diffuse"
}
