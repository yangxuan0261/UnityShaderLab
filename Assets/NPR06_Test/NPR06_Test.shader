// 参考: https://zhuanlan.zhihu.com/p/31805436

Shader "test/NPR06_Test" {
	Properties {
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_SpecularColor("Specular",Color) = (1,1,1,1)
		_RampTex("Ramp", 2D) = "white" {}
		_Gloss("Gloss", Range(8.0,256)) = 20
		_SpecIntensity("_SpecIntensity", Range(0, 1)) = 1
		_ShadowIntensity("ShadowIntensity", Range(0, 5)) = 1
	}

	SubShader {


		CGINCLUDE
		#include "Lighting.cginc"
		//计算阴影所用的宏包含在AutoLight.cginc文件中
		#include "AutoLight.cginc" 
		
		sampler2D _MainTex;
		sampler2D _RampTex;

		fixed4 _Color;
		fixed4 _SpecularColor;
		float   _Gloss;
		fixed _ShadowIntensity;
		fixed _SpecIntensity;

		struct a2v {
			float4  vertex:POSITION;
			float3  normal:NORMAL; 
			float4 texcoord : TEXCOORD0;
			float4 tangent : TANGENT;
		};

		struct v2f {
			float4 pos:SV_POSITION;
			float3 worldPos:TEXCOORD0;
			float3 worldNormal:TEXCOORD1;

			//该宏的作用是声明一个用于对阴影纹理采样的坐标
			//这个宏的参数是下一个可用的插值寄存器的索引值，上述中为2
			SHADOW_COORDS(2) 
			
			float2 uv:TEXCOORD3;
		};

		v2f vert_base(a2v v) {
			v2f o;
			o.pos=UnityObjectToClipPos(v.vertex);
			o.worldPos=mul(unity_ObjectToWorld,v.vertex).xyz;
			o.worldNormal=UnityObjectToWorldNormal(v.normal);
			o.uv = v.texcoord.xy;
			//该宏用于计算上一步声明的阴影纹理采样坐标
			TRANSFER_SHADOW(o);
			return o;
		}



		fixed4 frag_base(v2f i) : SV_Target {
			fixed4 albedo = tex2D(_MainTex, i.uv);

			float3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
			float3 worldNormal = normalize(i.worldNormal);
			float3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));

			float NdotL = dot(worldNormal, worldLightDir);
			float rampVal = NdotL * 0.5 + 0.5;

			fixed4 rampCol = tex2D(_RampTex, float2(rampVal, 0.3));


			fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
			fixed3 diffuse = _LightColor0.rgb * _Color.rgb *  rampCol.rgb * albedo; //*max(0,dot(worldLightDir,worldNormal));
			fixed3 halfDir = normalize(worldLightDir + worldViewDir);
			fixed3 specularColor = _LightColor0.rgb * _SpecularColor.rgb * pow(saturate(dot(halfDir,worldNormal)),_Gloss) * _SpecIntensity;

			//片元着色器中计算阴影值
			fixed shadow = SHADOW_ATTENUATION(i);
			shadow = shadow < 100 ? shadow * _ShadowIntensity : shadow;

			// UNITY_LIGHT_ATTENUATION(atten,i,i.worldPos); // unity提供的光照衰减
			fixed atten = 1.0;
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

		v2f vert_add(a2v v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
			o.worldNormal = UnityObjectToWorldNormal(v.normal);
			o.uv  =  v.texcoord.xy;
			return o;
		}

		fixed4 frag_add(v2f_add i) : SV_Target {
			fixed4 albedo  =  tex2D(_MainTex, i.uv);

			#ifdef USING_DIRECTIONAL_LIGHT
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
				fixed atten = 1.0;
			#else
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz-i.worldPos.xyz);
				float3 lightCoord = mul(unity_WorldToLight,float4(i.worldPos,1.0)).xyz;
				fixed atten = tex2D(_LightTexture0,dot(lightCoord,lightCoord).rr).UNITY_ATTEN_CHANNEL;
			#endif

			float3 worldNormal = normalize(i.worldNormal);
			float3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));

			fixed3 diffuse = _LightColor0.rgb*_Color.rgb*max(0,dot(worldLightDir,worldNormal));
			fixed3 halfDir = normalize(worldLightDir+worldViewDir);
			fixed3 specularColor = _LightColor0.rgb*_SpecularColor.rgb*pow(saturate(dot(halfDir,worldNormal)),_Gloss);

			return fixed4((diffuse*albedo+specularColor)*atten,1.0);
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
			#pragma vertex vert_add
			#pragma fragment frag_add
			ENDCG
		}
	}
	FallBack "Specular"
}
