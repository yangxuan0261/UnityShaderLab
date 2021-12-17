// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/light/half-lambert" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Diffuse ("Diffuse", Color) = (1, 1, 1, 1)
	}
	SubShader {
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
			};
 
			v2f vert(a2v v) {
				v2f o;
				//把顶点位置从模型空间转换到裁剪空间
				o.pos = UnityObjectToClipPos(v.vertex);
				//把顶点法线从模型空间转换到世界空间
				//_World2Object: 顶点变换矩阵的逆转置矩阵,这里取矩阵的前三行前三列。
				o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
				o.uv = v.uv;
				return o;
			}
			
			//漫反射光照模型——逐像素光照
			fixed4 frag(v2f i) : COLOR {
				float attenuation = LIGHT_ATTENUATION(i); // 很多时候衰减值直接定为 1.0

				//得到环境光
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
				//得到在世界空间上的顶点法线
				fixed3 worldNormal = normalize(i.worldNormal);
				//得到光源方向(注意，此方法只适合仅有一个平行光的时候)
				fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);

				fixed3 halfLambert = dot(worldNormal, worldLightDir)*0.5+0.5;
				//利用半兰伯特公式计算漫反射光
				fixed3 diffuse = _LightColor0.rgb * _Diffuse.rgb * halfLambert * attenuation;
				//加上环境光的影响
				fixed3 color = ambient + diffuse;
				float3 texColor = tex2D(_MainTex, i.uv).rgb;
				return fixed4(color*texColor, 1.0);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}