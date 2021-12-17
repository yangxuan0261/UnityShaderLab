// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
// 参考: Unity3D手游项目的总结和思考(2) - 角色渲染 - https://blog.csdn.net/qq18052887/article/details/80375546#commentBox

Shader "test/CharacterStandard_PBR" 
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_NormalTex("Normal Texture", 2D) = "bump" {}
		_GlossTex ("Gloss Texture (R:gloss) (G:roughness) B(:flowlight)", 2D) = "white" {}
		_SpecularColor ("Specular Color", Color) = (1, 1, 1, 0.5)	
		_SpecularColor2 ("Specular Color 2", Color) = (1, 1, 1, 1)
		_Roughness ("Roughness", Range (0, 1)) = 0
		_RoughnessBias ("Roughness Bias", Float) = 0
	    _RefectionTex("Refection Texture (Cubemap)", Cube) = "" {}
		_RefectionColor ("Refection Color", Color) = (1, 1, 1, 1)	
		_DissolveTex ("Dissolve Texture", 2D) = "white" {}
		_DissolveColor ("Dissolve Color", Color) = (1, 0, 0, 1)
		_DissolveCutoff ("Dissolve Cutoff", Range (0, 1)) = 0
		_FlowLightTex ("Flow Light Texture", 2D) = "white" {}
		_FlowLightColor ("Flow Light Color", Color) = (1, 0, 0, 1)		
		_FlowDirectionX("Flow Speed&Direction X", Float) = 0
		_FlowDirectionY("Flow Speed&Direction Y", Float) = 8
		_FlowDistortionIntensity("Flow Distortion Intensity", Range (0, 0.5)) = 0.2
		_FlashColor ("Flash Color", Color) = (1, 1, 1, 1)
 
		// 给程序动态控制用, 不给美术调用
		[HideInInspector] _FlashIntensity("", Range (0, 1)) = 0 
		[HideInInspector] _DoubleSided("", Float) = 2.0
		[HideInInspector] _CutOff("", Float) = 0
		[HideInInspector] _UseRoughness("", Float) = 0
	}
 
	SubShader
	{
		Tags
		{
			"Queue" = "Geometry" 
			"RenderType" = "Opaque" // 支持渲染到_CameraDepthNormalsTexture
		}
		
		Pass
		{
			Lighting Off
			Cull[_DoubleSided] // CullMode: Off(0) Front(1) Back(2)
 
			CGPROGRAM
 
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#pragma fragmentoption ARB_precision_hint_fastest
 
			#include "UnityCG.cginc"
			#include "TerrainEngine.cginc"
			#include "UnityStandardBRDF.cginc"
			#include "UnityStandardUtils.cginc"
 
			#pragma shader_feature _ALPHA_TEST_ON
			#pragma shader_feature _REFLECTION_ON
			#pragma shader_feature _NORMAL_MAP
			#pragma shader_feature _DISSOLVE_ON
			#pragma shader_feature _FLOW_LIGHT_ON
			#pragma shader_feature _USE_ROUGHNESS
 
			#pragma multi_compile __ _ORIGIN_ALPHA
			#pragma multi_compile __ _POINT_LIGHT
			#pragma multi_compile __ _FANCY_STUFF
			
			struct appdata_custom
			{
				half4 vertex : POSITION;
				half2 texcoord : TEXCOORD0;
#if _FANCY_STUFF
				half3 normal : NORMAL;
	#if _NORMAL_MAP
				half4 tangent : TANGENT;
	#endif
#endif 
			};
 
			// SM2.0的texture interpolator只有8个,要合理规划.
			struct v2f 
			{
				half4 pos : SV_POSITION;
				half2 uv0 : TEXCOORD0;
#if _FLOW_LIGHT_ON
				half2 uv1 : TEXCOORD1;
#endif
				UNITY_FOG_COORDS(2)
#if _FANCY_STUFF
				float3 posWorld : TEXCOORD3;
				half3 normalWorld : TEXCOORD4;
	#if _NORMAL_MAP
				half3 tangentWorld : TEXCOORD5;
				half3 binormalWorld : TEXCOORD6;
	#endif
#endif 
			};
		
			sampler2D _MainTex;
			half4 _MainTex_ST;
			fixed4 _Color;
			fixed4 _FlashColor;		
			fixed _FlashIntensity;
			sampler2D _GlossTex;
			fixed4 _SpecularColor;	
			fixed4 _SpecularColor2;
 
#if _DISSOLVE_ON
			sampler2D _DissolveTex;
			fixed4 _DissolveColor;
			half _DissolveCutoff;
#endif
 
#if _FLOW_LIGHT_ON
			sampler2D _FlowLightTex;
			half4 _FlowLightTex_ST;
			fixed4 _FlowLightColor;
			fixed _FlowDirectionX;
			fixed _FlowDirectionY;
			fixed _FlowDistortionIntensity;
#endif
 
#if _FANCY_STUFF
			fixed4 _GlobalAmbientColor;
			half4 _GlobalMainLightDir;
			fixed4 _GlobalMainLightColor;
			half4 _GlobalBackLightDir;
			fixed4 _GlobalBackLightColor;
	#if _POINT_LIGHT
			float4 _GlobalPointLightPos;
			fixed4 _GlobalPointLightColor;
			fixed _GlobalPointLightRange;
	#endif
			fixed _Roughness;
			fixed _RoughnessBias;
 
	#if _REFLECTION_ON
			uniform samplerCUBE _RefectionTex;
			fixed4 _RefectionColor;
	#endif
 
	#if _NORMAL_MAP
			uniform sampler2D _NormalTex;
	#endif
#endif 
			v2f vert(appdata_custom i)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(i.vertex);
				o.uv0 = TRANSFORM_TEX(i.texcoord, _MainTex);
#if _FLOW_LIGHT_ON
				o.uv1 = TRANSFORM_TEX(i.texcoord, _FlowLightTex);
#endif
#if _FANCY_STUFF
				o.posWorld = mul(unity_ObjectToWorld, i.vertex).xyz;
				o.normalWorld = UnityObjectToWorldNormal(i.normal);
	#if _NORMAL_MAP
				o.tangentWorld = UnityObjectToWorldDir(i.tangent);
				o.binormalWorld = cross(o.normalWorld, o.tangentWorld) * i.tangent.w;
	#endif
#endif 
				UNITY_TRANSFER_FOG(o, o.pos);
				return o;
			}
 
#if _ALPHA_TEST_ON
			uniform fixed _CutOff;
#endif
		
			fixed4 frag(v2f i) : COLOR
			{		
				// main color	
#if _FANCY_STUFF		
				fixed4 mainColor = tex2Dbias(_MainTex, fixed4(i.uv0, 0, -1.5)) * _Color; // 通过TextureImporter来设置mipmapbias对mobile平台无效		
#else
				fixed4 mainColor = tex2D(_MainTex, i.uv0) * _Color;
#endif
				
				fixed alpha = mainColor.a; 				
 
				// alpha test
#if _ALPHA_TEST_ON
				clip(alpha - _CutOff);
#endif	
 
				// dissolve
#if _DISSOLVE_ON
				if (_DissolveCutoff > 0) // performance
				{
					fixed dissolve = tex2D(_DissolveTex, i.uv0).r;
					fixed clipValue = dissolve - _DissolveCutoff;
					clip(clipValue);
 
					// edge range [0, 0.1)
					if (clipValue < 0.1)
					{	
						fixed3 edgeColor = fixed3(_DissolveColor.x, clipValue / 0.1, _DissolveColor.z);					
						fixed colorTotal = edgeColor.x + edgeColor.y + edgeColor.z;
						mainColor.rgb = (mainColor.rgb * edgeColor * colorTotal * colorTotal) * 3;		
					}					
				}
#endif							
				// gloss			
				fixed4 glossTex = tex2D(_GlossTex, i.uv0);	
				if (glossTex.r > _SpecularColor.a)
					mainColor.rgb *= _SpecularColor.rgb;
				if (glossTex.a > _SpecularColor2.a)
					mainColor.rgb *= _SpecularColor2.rgb;
				fixed4 finalColor = mainColor;
 
#if _FANCY_STUFF	
				// normalmap
	#if _NORMAL_MAP
				fixed3x3 tangentToWorld = fixed3x3(i.tangentWorld, i.binormalWorld, i.normalWorld);
				half3 normalMap = UnpackNormal(tex2D(_NormalTex, i.uv0));
				half3 fixedNormal = normalize(mul(normalMap, tangentToWorld));
	#else
				half3 fixedNormal = normalize(i.normalWorld);
	#endif	
				// common PBR params 	
	#if _USE_ROUGHNESS
				fixed roughness = _Roughness; 			
	#else
				fixed roughness = saturate(glossTex.g + _RoughnessBias); 
	#endif			
				fixed oneMinusRoughness = 1 - roughness;
				half specularPower = RoughnessToSpecPower(roughness);	
					
				// specular workflow
				fixed oneMinusReflectivity;
				fixed3 specColor = glossTex.r * mainColor.rgb; // monochrome specular -> color specular
				fixed3 diffColor = EnergyConservationBetweenDiffuseAndSpecular (mainColor.rgb, specColor, /*out*/ oneMinusReflectivity);
 
				// metallic workflow
				//fixed metallic = glossTex.r;	
				//fixed3 specColor;
				//fixed oneMinusReflectivity;		
				//fixed3 diffColor = DiffuseAndSpecularFromMetallic(mainColor.rgb, metallic, /*out*/ specColor, /*out*/ oneMinusReflectivity);
 
				fixed reflectivity = 1 - oneMinusReflectivity;
				half3 viewDir = normalize(_WorldSpaceCameraPos - i.posWorld);
				half nv = DotClamped(fixedNormal, viewDir);
				alpha *= reflectivity;
 
				// main light PBR				
				half3 halfDir = Unity_SafeNormalize(_GlobalMainLightDir + viewDir);		
				half nl = DotClamped(fixedNormal, _GlobalMainLightDir);
				half nh = DotClamped(fixedNormal, halfDir);				
				half lh = DotClamped(_GlobalMainLightDir, halfDir);						
				half invV = lh * lh * oneMinusRoughness + roughness * roughness; 
				half invF = lh;
				half specular = ((specularPower + 1) * pow (nh, specularPower)) / (8 * invV * invF + 1e-4h);
				if (IsGammaSpace())
					specular = sqrt(max(1e-4h, specular));
				specular = clamp(specular, 0.0, 100.0); // Prevent FP16 overflow on mobiles
				finalColor.rgb = _GlobalAmbientColor * diffColor + (diffColor + specular * specColor) * _GlobalMainLightColor * nl;
					
				// env reflection
	#if _REFLECTION_ON
				half3 reflUVW = normalize(reflect(-viewDir, fixedNormal));		
				fixed3 envColor = texCUBE(_RefectionTex, reflUVW) * _RefectionColor.rgb;
				half realRoughness = roughness * roughness;		
				half surfaceReduction = IsGammaSpace() ? 0.28 : (0.6 - 0.08*roughness);
				surfaceReduction = 1.0 - realRoughness * roughness * surfaceReduction;						
				half grazingTerm = saturate(oneMinusRoughness + reflectivity);	
				finalColor.rgb += surfaceReduction * envColor * FresnelLerpFast(specColor, grazingTerm, nv);
	#endif		
				// back light PBR
				halfDir = Unity_SafeNormalize(_GlobalBackLightDir + viewDir);		
				nl = DotClamped(fixedNormal, _GlobalBackLightDir);
				nh = DotClamped(fixedNormal, halfDir);				
				lh = DotClamped(_GlobalBackLightDir, halfDir);						
				invV = lh * lh * oneMinusRoughness + roughness * roughness; 
				invF = lh;
				specular = ((specularPower + 1) * pow (nh, specularPower)) / (8 * invV * invF + 1e-4h);
				if (IsGammaSpace())
					specular = sqrt(max(1e-4h, specular));
				specular = clamp(specular, 0.0, 100.0); // Prevent FP16 overflow on mobiles
				finalColor.rgb += (diffColor + specular * specColor) * _GlobalBackLightColor * nl;
 
				// point light PBR	
	#if _POINT_LIGHT	
				half3 toLight = _GlobalPointLightPos.xyz - i.posWorld ;
				half ratio = saturate(length(toLight) / _GlobalPointLightRange);
				//half attenuation = 1 - ratio; // linear attenuation
				ratio *= ratio;
				half attenuation = 1.0 / (1.0 + 0.01 * ratio) * (1 - ratio); // quadratic attenuation
				if (attenuation > 0) // performance
				{
					halfDir = Unity_SafeNormalize(toLight + viewDir);
					nl = DotClamped(fixedNormal, toLight);
					nh = DotClamped(fixedNormal, halfDir);
					lh = DotClamped(toLight, halfDir);
					invV = lh * lh * oneMinusRoughness + roughness * roughness; 
					invF = lh;
					specular = ((specularPower + 1) * pow (nh, specularPower)) / (8 * invV * invF + 1e-4h);
					if (IsGammaSpace())
						specular = sqrt(max(1e-4h, specular));		
					specular = clamp(specular, 0.0, 100.0); // Prevent FP16 overflow on mobiles
					finalColor.rgb += (diffColor + specular * specColor) * _GlobalPointLightColor * nl * attenuation;
				}
	#endif
#else
				finalColor.rgb *= 1.4;
#endif 
				// flow light
#if _FLOW_LIGHT_ON	
				fixed3 flow = tex2D(_FlowLightTex, frac(i.uv1 + _Time.xx * half2(_FlowDirectionX, _FlowDirectionY)) + mainColor.xy * _FlowDistortionIntensity).rgb;
				finalColor.rgb += flow * _FlowLightColor.rgb *_FlowLightColor.a * 2 * glossTex.b;
#endif
				// flash
				finalColor.rgb += _FlashIntensity * _FlashColor;
 
				// fog
				UNITY_APPLY_FOG(i.fogCoord, finalColor);
 
				// alpha
#if _ORIGIN_ALPHA
				finalColor.a = mainColor.a;
#else
				finalColor.a = alpha;
#endif
				
				return  finalColor;
			}
			ENDCG
		}
 
		// 没用Unity自带的阴影,只是用来来渲染_CameraDepthsTexture.
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
 
			Fog { Mode Off }
			ZWrite On
			Offset 1, 1
			Cull[_DoubleSided]
 
			CGPROGRAM
 
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma fragmentoption ARB_precision_hint_fastest
 
			#pragma shader_feature _ALPHA_TEST_ON
 
			#include "UnityCG.cginc"
 
			struct v2f
			{
				V2F_SHADOW_CASTER;
#if _ALPHA_TEST_ON
				fixed2 uv0 : TEXCOORD1;
#endif
			};
 
#if _ALPHA_TEST_ON
			sampler2D _MainTex;
			half4 _MainTex_ST;
			uniform fixed _CutOff;
#endif
 
			v2f vert(appdata_base v)
			{
				v2f o;
				TRANSFER_SHADOW_CASTER(o)
#if _ALPHA_TEST_ON
				o.uv0 = TRANSFORM_TEX(v.texcoord, _MainTex);
#endif
				return o;
			}
 
			fixed4 frag(v2f i) : COLOR
			{
#if _ALPHA_TEST_ON
				fixed4 col = tex2D(_MainTex, i.uv0);
				clip(col.a - _CutOff);
#endif
				SHADOW_CASTER_FRAGMENT(i)
			}
 
			ENDCG
		}
	}
 
	Fallback off
	CustomEditor "CharacterStandard_PBR_ShaderGUI"
}
