// 参考: https://zhuanlan.zhihu.com/p/31805436

Shader "test/NPR07_Test" {
	Properties {
		_MainTex("Base (RGB)", 2D) = "white" {}
		_RampTex("Ramp", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)

		_NormalTex("NormalMap", 2D) = "white" {}

		_SpecularColor("Specular",Color) = (1,1,1,1)
		_Gloss("Gloss", Range(8.0,256)) = 20
		_SpecIntensity("SpecIntensity", Range(0, 1)) = 1
		_FresnelScale("FresnelScale", Range(0, 1)) = 1
		_RimPower("RimPower", Range(0, 10)) = 1

		_ShadowIntensity("ShadowIntensity", Range(0, 5)) = 1

		_Cubemap("CubeMap", Cube) = ""{}
		_CubeAmount("CubeAmount", Range(0, 5)) = 1

		_BrightnessAmount("Brightess Amount",Range(0.0,5)) = 2.0
		_satAmount("Saturation Amount",Range(0.0,5)) = 1.0
		_conAmount("Contrast Amount",Range(0.0,5)) = 1.0

	}

	SubShader {
		CGINCLUDE
		#include "Lighting.cginc"
		//计算阴影所用的宏包含在AutoLight.cginc文件中
		#include "AutoLight.cginc"

		#pragma shader_feature _CUBEMAP_ON
		#pragma shader_feature _SHADOW_ON
		#pragma shader_feature _OHTER_LIGHT_OFF
		
		sampler2D _MainTex;
		sampler2D _RampTex;
		sampler2D _NormalTex;
		samplerCUBE _Cubemap;

		fixed4 _Color;
		fixed4 _SpecularColor;
		float _Gloss;
		float _ShadowIntensity;
		
		float _SpecIntensity;
		float _FresnelScale;
		float _RimPower;
		float _CubeAmount;

		fixed _BrightnessAmount;
		fixed _satAmount;
		fixed _conAmount;

		struct a2v {
			float4  vertex:POSITION;
			float3  normal:NORMAL; 
			float4 texcoord : TEXCOORD0;
			float4 tangent : TANGENT;
		};

		struct v2f {
			float4 pos:SV_POSITION;
			float3 worldPos:TEXCOORD0;
			float2 uv:TEXCOORD1;

			float3 TtoW0 : TEXCOORD2;
			float3 TtoW1 : TEXCOORD3;
			float3 TtoW2 : TEXCOORD4;

			//这个宏的参数是下一个可用的插值寄存器的索引值
			SHADOW_COORDS(5) 
		};


		float3 ContrastSaturationBrightness(float3 color,float brt,float sat,float con)
		{
			float AvgLumR = 0.5;
			float AvgLumG =0.5;
			float AvgLumB = 0.5;
			float3 LuminanceCoeff = float3(0.2125,0.7154,0.0721);
			float3 AvgLumin = float3(AvgLumR,AvgLumG,AvgLumB);
			float3 brtColor = color * brt;
			float intensityf = dot(brtColor,LuminanceCoeff);
			float3 intensity = float3(intensityf,intensityf,intensityf);
			float3 satColor = lerp(intensity,brtColor,sat);
			float3 conColor = lerp(AvgLumin,satColor,con);
			return conColor;
		}

		v2f vert_base(a2v v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
			o.uv = v.texcoord.xy;

			// // 切线空间的三个向量
			fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
			fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
			fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w; // 根据 法线和切线 计算出 副切线, 组成 切线空间 的坐标系

			o.TtoW0 = float3(worldTangent.x, worldBinormal.x, worldNormal.x);
			o.TtoW1 = float3(worldTangent.y, worldBinormal.y, worldNormal.y);
			o.TtoW2 = float3(worldTangent.z, worldBinormal.z, worldNormal.z);

			//该宏用于计算上一步声明的阴影纹理采样坐标
			TRANSFER_SHADOW(o);
			return o;
		}

		fixed4 frag_base(v2f i) : SV_Target {
			fixed4 albedo = tex2D(_MainTex, i.uv);

			float3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
			float3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));

			float3 tangentNormal = UnpackNormal(tex2D(_NormalTex, i.uv));

			// 构建 转换矩阵
			float3x3 worldNormalMatrix = float3x3(i.TtoW0.xyz, i.TtoW1.xyz, i.TtoW2.xyz);
			float3 worldNormal = normalize(mul(worldNormalMatrix, tangentNormal));

			float NdotL = dot(worldNormal, worldLightDir);
			float rampVal = NdotL * 0.5 + 0.5;

			// diffuse term
			float4 rampCol = tex2D(_RampTex, float2(rampVal, 0.3));
			float3 diffuse = _LightColor0.rgb * _Color.rgb * albedo.rgb * rampCol.rgb; // * max(0,dot(worldLightDir,worldNormal));

			// specular term
			float3 halfVector = normalize(worldLightDir + worldViewDir);
			float3 specBase = pow(saturate(dot(halfVector, worldNormal)), _Gloss) * _SpecIntensity;

			// fresnel
			float fresnel = pow(1.0 - dot(worldViewDir, halfVector), 5.0);
			fresnel = _FresnelScale + (1.0 - _FresnelScale) * fresnel;

			float3 finalSpec = specBase * fresnel * _LightColor0.rgb * _SpecularColor.rgb;

			// shadow term
			float shadow2 = 1;
			#if _SHADOW_ON
				shadow2 = UNITY_SHADOW_ATTENUATION(i, i.worldPos);
				// shadow2 = shadow2 < _ShadowThd ? shadow2 * _ShadowIntensity : 1;
				// float3 shdClr = float3(shadow2, shadow2, shadow2);
				// shdClr = ContrastSaturationBrightness(shdClr, _BrightnessAmount, _satAmount, _conAmount);
				// shadow2 = shdClr.r;
			#endif

			// reflect term
			#if _CUBEMAP_ON
				float3 worldRef = reflect(-worldViewDir, normalize(worldNormal));
				fixed4 colCube = texCUBE(_Cubemap, worldRef) * _CubeAmount;
				diffuse *= colCube;
			#endif

			// UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos); // unity提供的光照衰减
			float atten = 1.0;
			
			//rim light term
			half rim = 1.0 - saturate(dot(worldViewDir, worldNormal));
			rim = pow(rim, _RimPower) * 0.5;

			float3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
			fixed4 finalClr = fixed4(ambient + (diffuse + finalSpec + rim) * atten * shadow2, 1.0); // 有阴影时的计算方式
			finalClr.rgb = ContrastSaturationBrightness(finalClr.rgb, _BrightnessAmount, _satAmount, _conAmount);
			return finalClr;
			// return fixed4(rampCol.rbg, 1);
		}


		//----------------------------
		// struct a2v_add{
			// 	float4  vertex:POSITION;
			// 	float3  normal:NORMAL; 
		// };

		struct v2f_add {
			float4 pos:SV_POSITION;
			float3 worldPos:TEXCOORD0;
			float3 worldNormal:TEXCOORD1;
			float2 uv:TEXCOORD2;
		};

		// v2f vert_add(a2v v) {
			// 	v2f o;
			// 	o.pos = UnityObjectToClipPos(v.vertex);
			// 	o.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
			// 	o.worldNormal = UnityObjectToWorldNormal(v.normal);
			// 	o.uv  =  v.texcoord.xy;
			// 	return o;
		// }

		fixed4 frag_add(v2f i) : SV_Target {
			#if _OHTER_LIGHT_OFF
			return fixed4(0,0,0,1);
#endif

			fixed4 albedo = tex2D(_MainTex, i.uv);

			float3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
			float3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));

			float3 tangentNormal = UnpackNormal(tex2D(_NormalTex, i.uv));

			// 构建 转换矩阵
			float3x3 worldNormalMatrix = float3x3(i.TtoW0.xyz, i.TtoW1.xyz, i.TtoW2.xyz);
			float3 worldNormal = normalize(mul(worldNormalMatrix, tangentNormal));

			float NdotL = dot(worldNormal, worldLightDir);
			float rampVal = NdotL * 0.5 + 0.5;

			// diffuse term
			fixed3 lambert = max(0, dot(worldLightDir,worldNormal)); // 兰伯特
			float3 diffuse = _LightColor0.rgb * _Color.rgb * albedo.rgb * lambert;

			// specular term
			float3 halfVector = normalize(worldLightDir + worldViewDir);
			float3 specBase = pow(saturate(dot(halfVector, worldNormal)), _Gloss) * _SpecIntensity;

			// fresnel
			float fresnel = pow(1.0 - dot(worldViewDir, halfVector), 5.0);
			fresnel = _FresnelScale + (1.0 - _FresnelScale) * fresnel;

			float3 finalSpec = specBase * fresnel * _LightColor0.rgb * _SpecularColor.rgb;

			// reflect term
			#if _CUBEMAP_ON
				float3 worldRef = reflect(-worldViewDir, normalize(worldNormal));
				fixed4 colCube = texCUBE(_Cubemap, worldRef) * _CubeAmount;
				diffuse *= colCube;
			#endif

			// UNITY_LIGHT_ATTENUATION(atten,i,i.worldPos); // unity提供的光照衰减
			float atten = 1.0;

			float3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
			return fixed4((diffuse + finalSpec) * atten, 1.0); // 有阴影时的计算方式
		}

		ENDCG


		Pass {
			Tags {"LightMode" = "ForwardBase"}

			CGPROGRAM
			#pragma multi_compile_fwdbase
			#pragma vertex vert_base
			#pragma fragment frag_base
			ENDCG
		}

		Pass {
			Tags{"LightMode" = "ForwardAdd"}
			Blend One One
			CGPROGRAM
			#pragma multi_compile_fwdadd
			#pragma vertex vert_base
			#pragma fragment frag_add
			ENDCG
		}
	}

	FallBack "Specular"
	CustomEditor "NPRShaderGUI"
}
