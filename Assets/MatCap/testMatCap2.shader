// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// MatCap Shader, (c) 2015-2017 Jean Moreno

Shader "ITS/test/Bumped_Textured_Multiply2"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MatCap_0 ("MatCap (RGB)", 2D) = "white" {}
		_Color("Tint", Color) = (1, 1, 1, 1)
		// [Toggle(ENABLE_GRAY)] _Fancy ("Gray?", Float) = 0
	}
	
	Subshader
	{
		Tags { "RenderType"="Opaque" }
		
		Pass
		{
			Tags { "LightMode" = "Always" }
			
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				// #pragma multi_compile __ ENABLE_GRAY
				#pragma multi_compile_fog
				#include "UnityCG.cginc"


				struct v2f
				{
					float4 pos	: SV_POSITION;
					float4 uv 	: TEXCOORD0;
					// float2 cap	: TEXCOORD1;
					UNITY_FOG_COORDS(2)
				};
				
				uniform float4 _MainTex_ST;
				
				v2f vert (appdata_base v)
				{
					v2f o;
					// 方式一
					// o.pos = UnityObjectToClipPos (v.vertex);

					// 方式二
					// float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
					// o.pos = mul(UNITY_MATRIX_VP, worldPos);

					// 方式三
					float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
					o.pos.x = mul(UNITY_MATRIX_VP[0], worldPos);
					o.pos.y = mul(UNITY_MATRIX_VP[1], worldPos);
					o.pos.z = mul(UNITY_MATRIX_VP[2], worldPos);
					o.pos.w = mul(UNITY_MATRIX_VP[3], worldPos);

					o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
					half2 capCoord;
					


					// 变换 法线 从 模型空间 -> 观察空间
					// 方式一
					float3 worldNorm = UnityObjectToWorldNormal(v.normal).xyz;
					float3 viewNormal = mul((float3x3)UNITY_MATRIX_V, worldNorm); // 将法线转到观察空间下, 因为matcap贴图是摄像机看到的贴图
					o.uv.zw = viewNormal.xy; // 转换法线值为贴图值



					// 方式二
					// float3 viewNormal = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal.xyz);
					// o.uv.zw = viewNormal.xy;

					// 方式三
					// o.uv.z = mul(UNITY_MATRIX_IT_MV[0], v.normal);
					// o.uv.w = mul(UNITY_MATRIX_IT_MV[1], v.normal);

					UNITY_TRANSFER_FOG(o, o.pos);

					return o;
				}
				
				uniform sampler2D _MainTex;
				uniform sampler2D _MatCap_0;
				uniform fixed4 _Color;
				
				fixed4 frag (v2f i) : COLOR
				{
					fixed4 tex = tex2D(_MainTex, i.uv.xy);
					tex.rgba = tex.rgba * _Color;

					float2 mUv = i.uv.zw  * 0.5 + 0.5; // 转换法线值为贴图值
					// fixed4 mc = tex2D(_MatCap_0, mUv);
					fixed4 mc = tex2D(_MatCap_0, mUv) * tex; // 这里使用叠加的方式必然会使原图更加暗色, 可以点参数来提高亮度
					
					// #if ENABLE_GRAY
					// float gray = dot( mc.rgb, float3(0.299, 0.587, 0.114 ));
					// mc.rgb = fixed3(gray, gray, gray);
					// #endif

					UNITY_APPLY_FOG(i.fogCoord, mc);

					return mc;
				}
			ENDCG
		}
	}
	
	Fallback "VertexLit"
}
