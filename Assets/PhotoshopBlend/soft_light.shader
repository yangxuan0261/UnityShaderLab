﻿Shader "MyBlend/soft_light"
{
	Properties
	{
		_Layer1Tex ("ly1", 2D) = "white" {}
		_Layer2Tex ("ly2", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _Layer1Tex;
			sampler2D _Layer2Tex;
			float4 _Layer1Tex_ST;
			float4 _Layer2Tex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _Layer1Tex);
				return o;
			}
			
			// 逐通道进行运算
			// 若 a <= 0.5, c = 2ab + b^{2} (1-2a)
			// 若 a > 0.5, c = 2b(1-a) + \sqrt{b} (2a-1)
			fixed calcu(fixed a, fixed b) {
				return a > 0.5 ? 2*b*(1-a) + sqrt(b)*(2*a-1) : 2*a*b + pow(b, 2)*(1-2*a);
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col1 = tex2D(_Layer1Tex, i.uv);
				fixed4 col2 = tex2D(_Layer2Tex, i.uv);
				return fixed4(calcu(col1.r,col2.r), calcu(col1.g,col2.g), calcu(col1.b,col2.b), 1);
			}
			ENDCG
		}
	}
}
