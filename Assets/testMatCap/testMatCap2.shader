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
					o.pos = UnityObjectToClipPos (v.vertex);
					o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
					half2 capCoord;
					
					// float3 worldNorm = normalize(unity_WorldToObject[0].xyz * v.normal.x + unity_WorldToObject[1].xyz * v.normal.y + unity_WorldToObject[2].xyz * v.normal.z);
					float3 worldNorm = UnityObjectToWorldNormal(v.normal).xyz;
					worldNorm = mul((float3x3)UNITY_MATRIX_V, worldNorm); // 将法线转到观察空间下, 因为matcap贴图是摄像机看到的贴图
					o.uv.zw = worldNorm.xy * 0.5 + 0.5; // 转换法线值为贴图值
					
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

					fixed4 mc = tex2D(_MatCap_0, i.uv.zw) * tex * 2.0;
					
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
