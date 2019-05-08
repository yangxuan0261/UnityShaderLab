Shader "test/Unlit-Texture-Dissolve" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Noise ("Noise (RGB)", 2D) = "white" {}
		_BurnAmout ("Burn Amount", Range(0.0, 1.0)) = 0.0
		_EdgeWidth("EdgeWidth", Range(-1, 0)) = 0.05  
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
			fixed _BurnAmout;
			uniform float _EdgeWidth;  

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

				fixed isShow = step(val, _BurnAmout);
				clip(1 - isShow - 0.0001);

				fixed4 mainClr = tex2D(_MainTex, i.texcoord);
				fixed4 edgeClr = fixed4(1, 1, 0, 1);
				fixed4 finalClr;

				// 方式a
				// if (_BurnAmout - _EdgeWidth < val) {
				// 	finalClr = mainClr;
				// } else {
				// 	finalClr = edgeClr;
				// }

				// 方式b
				// fixed isEdge = 1 - step(_BurnAmout - _EdgeWidth, val);
				// finalClr = lerp(mainClr, edgeClr, isEdge);

				// 方式c 平滑边界颜色
				float edgeSmooth = 1 - smoothstep(val, _BurnAmout, _BurnAmout - _EdgeWidth);
				edgeClr *= edgeSmooth;
				finalClr = mainClr + edgeClr;

				// 方式a 与 方式b 等价, 但 方式b 更优于 gpu 计算
				UNITY_APPLY_FOG(i.fogCoord, finalClr);
				UNITY_OPAQUE_ALPHA(finalClr.a);
				return finalClr;
			}
			ENDCG
		}
	}
}
