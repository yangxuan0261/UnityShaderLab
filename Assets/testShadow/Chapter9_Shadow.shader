// 参考: https://zhuanlan.zhihu.com/p/31805436

Shader "Custom/Chapter9_Shadow" {
	Properties{
	_Color("Color",Color)=(1,1,1,1)
	_SpecularColor("Specular",Color)=(1,1,1,1)
	_Gloss("Gloss",Range(8.0,256))=20
	}

	SubShader{
	Pass{
		Tags{"LightMode"="ForwardBase"}

		CGPROGRAM
		#pragma multi_compile_fwdbase
		#pragma vertex vert
		#pragma fragment frag

		#include "Lighting.cginc"
		#include "AutoLight.cginc" 
		//计算阴影所用的宏包含在AutoLight.cginc文件中

		fixed4 _Color;
		fixed4 _SpecularColor;
		float   _Gloss;

		struct a2v{
			float4  vertex:POSITION;
			float3  normal:NORMAL; 
		};

		struct v2f{
			float4 pos:SV_POSITION;
			float3 worldPos:TEXCOORD0;
			float3 worldNormal:TEXCOORD1;

			//该宏的作用是声明一个用于对阴影纹理采样的坐标
			//这个宏的参数是下一个可用的插值寄存器的索引值，上述中为2
			SHADOW_COORDS(2) 
		};

		v2f vert(a2v v){
			v2f o;
			o.pos=UnityObjectToClipPos(v.vertex);
			o.worldPos=mul(unity_ObjectToWorld,v.vertex).xyz;
			o.worldNormal=UnityObjectToWorldNormal(v.normal);

			//该宏用于计算上一步声明的阴影纹理采样坐标
			TRANSFER_SHADOW(o);

			return o;
		}

		fixed4 frag(v2f i):SV_Target{
			float3 worldLightDir=normalize(UnityWorldSpaceLightDir(i.worldPos));
			float3 worldNormal=normalize(i.worldNormal);
			float3 worldViewDir=normalize(UnityWorldSpaceViewDir(i.worldPos));

			fixed3 ambient=UNITY_LIGHTMODEL_AMBIENT.xyz;
			fixed3 diffuse=_LightColor0.rgb*_Color.rgb*max(0,dot(worldLightDir,worldNormal));
			fixed3 halfDir=normalize(worldLightDir+worldViewDir);
			fixed3 specularColor=_LightColor0.rgb*_SpecularColor.rgb*pow(saturate(dot(halfDir,worldNormal)),_Gloss);

			//片元着色器中计算阴影值
			fixed shadow=SHADOW_ATTENUATION(i);

			// UNITY_LIGHT_ATTENUATION(atten,i,i.worldPos); // unity提供的光照衰减
			fixed atten=1.0;
			return fixed4(ambient+(diffuse+specularColor)*atten*shadow,1.0); // 有阴影时的计算方式
		}
		ENDCG
	}

	Pass{
		Tags{"LightMode"="ForwardAdd"}
		Blend One One
		CGPROGRAM
		#pragma multi_compile_fwdadd
		#pragma vertex vert
		#pragma fragment frag

		#include "Lighting.cginc"
		#include "AutoLight.cginc"

		fixed4 _Color;
		fixed4 _SpecularColor;
		float   _Gloss;

		struct a2v{
			float4  vertex:POSITION;
			float3  normal:NORMAL; 
		};

		struct v2f{
			float4 pos:SV_POSITION;
			float3 worldPos:TEXCOORD0;
			float3 worldNormal:TEXCOORD1;
		};

		v2f vert(a2v v){
			v2f o;
			o.pos=UnityObjectToClipPos(v.vertex);
			o.worldPos=mul(unity_ObjectToWorld,v.vertex).xyz;
			o.worldNormal=UnityObjectToWorldNormal(v.normal);

			return o;
		}

		fixed4 frag(v2f i):SV_Target{
			#ifdef USING_DIRECTIONAL_LIGHT
				fixed3 worldLightDir=normalize(_WorldSpaceLightPos0.xyz);
				fixed atten=1.0;
			#else
				fixed3 worldLightDir=normalize(_WorldSpaceLightPos0.xyz-i.worldPos.xyz);
				float3 lightCoord=mul(unity_WorldToLight,float4(i.worldPos,1.0)).xyz;
				fixed atten=tex2D(_LightTexture0,dot(lightCoord,lightCoord).rr).UNITY_ATTEN_CHANNEL;
			#endif
			float3 worldNormal=normalize(i.worldNormal);
			float3 worldViewDir=normalize(UnityWorldSpaceViewDir(i.worldPos));

			fixed3 diffuse=_LightColor0.rgb*_Color.rgb*max(0,dot(worldLightDir,worldNormal));
			fixed3 halfDir=normalize(worldLightDir+worldViewDir);
			fixed3 specularColor=_LightColor0.rgb*_SpecularColor.rgb*pow(saturate(dot(halfDir,worldNormal)),_Gloss);

			
			return fixed4((diffuse+specularColor)*atten,1.0);
		}
		ENDCG
	}
	}
	FallBack "Specular"
	}  

/*
第二个Pass是为了计算场景中其他光源对物体的影响。阴影的计算主要在第一个Pass中 (主光源) ，也就是"LightMode"="ForwardBase" 。 在前向渲染中，SHADOW_COORDS 声明一个阴影纹理坐标， TRANSFER_SHADOW 根据平台不同使用屏幕空间阴影纹理映射技术，将顶点坐标从模型空间变换到光源空间后存储到之前声明的阴影纹理坐标中， SHADOW_ATTENUATION 负责使用阴影映射纹理坐标对相关纹理进行采样，得到阴影信息。
这里需要注意的是 这些宏会使用上下文变量来进行相关计算，为了确保宏正确工作，要保证自定义的变量名与宏使用的变量名相匹配，a2f结构体中顶点坐标变量名必须是 vertex ,顶点着色器的输入结构体a2v 必须命名为 v v2f中的顶点位置变量名必须是 pos。
*/