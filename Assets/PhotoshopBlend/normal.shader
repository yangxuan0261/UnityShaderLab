﻿Shader "MyBlend/normal"
{
	Properties
	{
		_Layer1Tex ("ly1", 2D) = "white" {}
		_Layer2Tex ("ly2", 2D) = "white" {}
	}
	SubShader
	{
			
		Tags { "Queue"="Transparent"  }
		LOD 100

		// c = n*a + (1-n)*b
		Blend SrcAlpha OneMinusSrcAlpha

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
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col1 = tex2D(_Layer1Tex, i.uv);
				fixed4 col2 = tex2D(_Layer2Tex, i.uv);
				return col1;
			}
			ENDCG
		}
	}
}