// 参考: https://zhuanlan.zhihu.com/p/31805436

Shader "test/NPR07_Test" {
	Properties {
		_MainTex("Base (RGB)", 2D) = "white" {}
		_NormalTex("NormalMap", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_SpecularColor("Specular",Color) = (1,1,1,1)
		_RampTex("Ramp", 2D) = "white" {}
		_Gloss("Gloss", Range(8.0,256)) = 20
		_SpecIntensity("_SpecIntensity", Range(0, 1)) = 1
		_ShadowIntensity("ShadowIntensity", Range(0, 5)) = 1
		_Cubemap("CubeMap", Cube) = ""{}
		_CubeAmount("CubeAmount", Range(0, 5)) = 1
	}

	SubShader {


		CGINCLUDE
		#include "Lighting.cginc"
		//计算阴影所用的宏包含在AutoLight.cginc文件中
		#include "AutoLight.cginc" 
		#pragma shader_feature _CUBEMAP_ON
		
		sampler2D _MainTex;
		sampler2D _RampTex;
		sampler2D _NormalTex;
		samplerCUBE _Cubemap;

		fixed4 _Color;
		fixed4 _SpecularColor;
		float _Gloss;
		fixed _ShadowIntensity;
		fixed _SpecIntensity;
		float _CubeAmount;

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

			fixed3 tangentNormal = UnpackNormal(tex2D(_NormalTex, i.uv));

			// 构建 转换矩阵
			float3x3 worldNormalMatrix = float3x3(i.TtoW0.xyz, i.TtoW1.xyz, i.TtoW2.xyz);
			fixed3 worldNormal = normalize(mul(worldNormalMatrix, tangentNormal));

			float NdotL = dot(worldNormal, worldLightDir);
			float rampVal = NdotL * 0.5 + 0.5;

			// diffuse term
			fixed4 rampCol = tex2D(_RampTex, float2(rampVal, 0.3));
			fixed3 diffuse = _LightColor0.rgb * _Color.rgb * albedo * rampCol.rgb; // * max(0,dot(worldLightDir,worldNormal));

			// specular term
			fixed3 halfDir = normalize(worldLightDir + worldViewDir);
			fixed3 specularColor = _LightColor0.rgb * _SpecularColor.rgb * pow(saturate(dot(halfDir,worldNormal)),_Gloss) * _SpecIntensity;

			// shadow term
			fixed shadow = SHADOW_ATTENUATION(i);
			shadow = shadow < 100 ? shadow * _ShadowIntensity : shadow;

			// reflect term
			#if _CUBEMAP_ON
				float3 worldRef = reflect(-worldViewDir, normalize(worldNormal));
				fixed4 colCube = texCUBE(_Cubemap, worldRef) * _CubeAmount;
				diffuse *= colCube;
			#endif

			// UNITY_LIGHT_ATTENUATION(atten,i,i.worldPos); // unity提供的光照衰减
			fixed atten = 1.0;

			fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
			return fixed4(ambient + (diffuse + specularColor) * atten * shadow, 1.0); // 有阴影时的计算方式
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

		// fixed4 frag_add(v2f_add i) : SV_Target {
			// 	fixed4 albedo  =  tex2D(_MainTex, i.uv);

			// 	#ifdef USING_DIRECTIONAL_LIGHT
			// 		fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
			// 		fixed atten = 1.0;
			// 	#else
			// 		fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz-i.worldPos.xyz);
			// 		float3 lightCoord = mul(unity_WorldToLight,float4(i.worldPos,1.0)).xyz;
			// 		fixed atten = tex2D(_LightTexture0,dot(lightCoord,lightCoord).rr).UNITY_ATTEN_CHANNEL;
			// 	#endif

			// 	float3 worldNormal = normalize(i.worldNormal);
			// 	float3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));

			// 	fixed3 diffuse = _LightColor0.rgb*_Color.rgb*max(0,dot(worldLightDir,worldNormal));
			// 	fixed3 halfDir = normalize(worldLightDir+worldViewDir);
			// 	fixed3 specularColor = _LightColor0.rgb*_SpecularColor.rgb*pow(saturate(dot(halfDir,worldNormal)),_Gloss);

			// 	return fixed4((diffuse*albedo+specularColor)*atten,1.0);
		// }

		
		fixed4 frag_add(v2f i) : SV_Target {
			fixed4 albedo = tex2D(_MainTex, i.uv);

			float3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
			float3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));

			fixed3 tangentNormal = UnpackNormal(tex2D(_NormalTex, i.uv));

			// 构建 转换矩阵
			float3x3 worldNormalMatrix = float3x3(i.TtoW0.xyz, i.TtoW1.xyz, i.TtoW2.xyz);
			fixed3 worldNormal = normalize(mul(worldNormalMatrix, tangentNormal));

			float NdotL = dot(worldNormal, worldLightDir);
			float rampVal = NdotL * 0.5 + 0.5;

			fixed4 rampCol = tex2D(_RampTex, float2(rampVal, 0.3));


			fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
			fixed3 diffuse = _LightColor0.rgb * _Color.rgb * albedo * max(0,dot(worldLightDir,worldNormal));
			fixed3 halfDir = normalize(worldLightDir + worldViewDir);
			fixed3 specularColor = _LightColor0.rgb * _SpecularColor.rgb * pow(saturate(dot(halfDir,worldNormal)),_Gloss) * _SpecIntensity;

			// UNITY_LIGHT_ATTENUATION(atten,i,i.worldPos); // unity提供的光照衰减
			fixed atten = 1;
			return fixed4((diffuse + specularColor) * atten, 1.0); // 有阴影时的计算方式
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
