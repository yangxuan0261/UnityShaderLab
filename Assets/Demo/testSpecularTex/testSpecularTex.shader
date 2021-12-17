// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "test/testSpecularTex"
{
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Diffuse ("Diffuse", Color) = (1, 1, 1, 1)
		_SpecularTex ("SpecularTex", 2D) = "white" {}
		_SpecularPower("SpecularPower", float) = 1
		_SpecularColor ("SpecularColor", Color) = (1, 1, 1, 1)
	}
	SubShader {
		pass{
			ZWrite On
			ColorMask 0
		}
		pass{
			ZWrite On
			ColorMask 0
		}
		pass{
			Tags { "LightMode"="ForwardBase" }
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			
			fixed4 _Diffuse;
			sampler2D _MainTex;

			sampler2D _SpecularTex;
			float _SpecularPower;
			float4 _SpecularColor;

			struct a2v {
				float4 vertex : POSITION;
				//存储模型的顶点法线信息
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};
			
			struct v2f {
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
				//需要把法线传给片元着色器
				fixed3 worldNormal : TEXCOORD1;
				float4 worldPos : TEXCOORD2;
			};
			
			v2f vert(a2v v) {
				v2f o;
				//把顶点位置从模型空间转换到裁剪空间
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.pos = mul(UNITY_MATRIX_VP, o.worldPos);
				// o.pos = UnityObjectToClipPos(v.vertex);
				
				o.worldNormal = mul((float3x3)unity_ObjectToWorld, v.normal);
				// o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.uv = v.uv;
				return o;
			}
			
			//漫反射光照模型——逐像素光照
			fixed4 frag(v2f i) : COLOR {
				float attenuation = LIGHT_ATTENUATION(i); // 很多时候衰减值直接定为 1.0

				//得到环境光
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;

				float3 worldLightDir=normalize(UnityWorldSpaceLightDir(i.worldPos));
				float3 worldNormal=normalize(i.worldNormal);
				float3 worldViewDir=normalize(UnityWorldSpaceViewDir(i.worldPos));

				// blinn-phong 高光模型
				float3 specularTexCol = tex2D(_SpecularTex, i.uv).rgb;
				float3 halfVector = normalize(worldLightDir + worldViewDir);
				float specular = pow(max(0, dot(worldNormal, halfVector)) , _SpecularPower);
				float3 specularColor =_LightColor0.rgb* specularTexCol * specular * _SpecularColor.rgb * attenuation * 10;

				fixed3 halfLambert = dot(worldNormal, worldLightDir)*0.5+0.5;
				//利用半兰伯特公式计算漫反射光
				fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * halfLambert * attenuation;
				//加上环境光的影响
				fixed3 color = ambient + diffuse + specularColor;
				float3 texColor = tex2D(_MainTex, i.uv).rgb;
				return fixed4(color*texColor, 1.0);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}