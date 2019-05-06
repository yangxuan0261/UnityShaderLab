Shader "test/Unlit-Texture-Dissolve" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Noise ("Noise (RGB)", 2D) = "white" {}
		_MainColor ("Color", Color) = (1, 1, 1, 1)
		_BurnAmout ("Burn Amount", Range(0.0, 1.0)) = 0.0
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100
		
		Pass {  
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				float2 uvNoise : TEXCOORD1;

				UNITY_FOG_COORDS(1)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _MainColor;
			fixed _BurnAmout;

			sampler2D _Noise;
			float4 _Noise_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uvNoise = TRANSFORM_TEX(v.texcoord, _Noise);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed val = tex2D(_Noise, i.uvNoise).r;

				fixed diff = val - _BurnAmout;
				fixed edgeWidth = 0.05; // 这个是边缘宽度, 可以提取乘一个参数暴露给材质动态控制
				
				clip(diff);

				fixed4 col = tex2D(_MainTex, i.texcoord) * _MainColor;
				fixed4 edgeClr = fixed4(1, 1, 0, 1);
				fixed4 finalClr;

				// 方式a
				// if (diff < edgeWidth) {
				// 	finalClr = edgeClr;
				// } else {
				// 	finalClr = col;
				// }

				// 方式b
				// fixed isEdge = saturate(sign(edgeWidth - diff));
				fixed isEdge = step(diff, edgeWidth); // 与上一行代码等价
				// finalClr = isEdge * edgeClr + (1 - isEdge)*col;
				finalClr = lerp(col, edgeClr, isEdge); // 与上一行代码等价

				// finalClr = isEdge * lerp(col*edgeClr, edgeClr, (diff - edgeWidth) / edgeWidth) + (1 - isEdge)*col; // 做一个差值, 
				

				// 方式a 与 方式b 等价, 但 方式b 更优于 gpu 计算
				UNITY_APPLY_FOG(i.fogCoord, finalClr);
				UNITY_OPAQUE_ALPHA(finalClr.a);
				return finalClr;
			}
			ENDCG
		}
	}
}
