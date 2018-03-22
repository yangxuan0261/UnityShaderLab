// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "ITS/test/Unlit-Transparent02" {
Properties {
	_TintColor("Tint Color", Color) = (1, 1, 1, 1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100
	
	//最终颜色 = SrcColor * SrcFactor + DstColor * DstFactor

	ZWrite Off
	// Blend SrcAlpha OneMinusSrcAlpha //正常模式(透明度混合) 
	// Blend OneMinusDstColor One //柔和相加(soft Additive) 
	// Blend DstColor Zero //正片叠底 (Multiply)相乘 
	// Blend DstColor SrcColor //两倍相乘 (2X Multiply)
	// Blend OneMinusDstColor One //滤色 
	// Blend One One //线性变淡 

	// BlendOp Min //变暗
	// Blend One One
	
	// BlendOp Max //变亮
	// Blend One One

	// Blend SrcColor Zero //
	Blend DstColor Zero //


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
				UNITY_FOG_COORDS(1)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _TintColor;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord) * _TintColor;
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
		ENDCG
	}
}

}
