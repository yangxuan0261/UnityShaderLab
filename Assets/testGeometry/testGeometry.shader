// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ITS/test/GeomShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" "LightMode" = "ForwardBase"}
		
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom

			#include "UnityCG.cginc"
			#include "Lighting.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
				float3 worldPosition : TEXCOORD1;
			};

			sampler2D _MainTex;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.normal = v.normal;
				o.worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;
				return o;
			}

			[maxvertexcount(3)]                                                //表示最后outputStream中的v2f数据是3个
			void geom(triangle v2f input[3], inout TriangleStream<v2f> OutputStream)
			{
				v2f test = (v2f)0;//这里直接重构v2f这个结构体，也可以定义v2g，g2f两个结构体来完成这个传递过程
				float3 normal = normalize(cross(input[1].worldPosition.xyz - input[0].worldPosition.xyz, input[2].worldPosition.xyz - input[0].worldPosition.xyz));
				for (int i = 0; i < 3; i++)
				{
					test.normal = normal;  //顶点变	为这个三角图元的法线方向
					test.vertex = input[i].vertex; //+ float4(normal, 0) * 1;
					test.uv = input[i].uv;
					OutputStream.Append(test);
				}
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 albedo = tex2D(_MainTex, i.uv);
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo.rgb;
				float3 lightDir = -_WorldSpaceLightPos0.xyz; 
				fixed3 diffuse = _LightColor0.rgb * albedo.rgb * saturate(dot(i.normal, normalize(lightDir)));

				return fixed4(ambient + diffuse, 1.0);
			}
			ENDCG
		}
	}
}
