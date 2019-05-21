// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// reference: https://zhuanlan.zhihu.com/p/42781261

Shader "Custom/PlanarShadow1"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}

		_ShadowColor ("ShadowColor", Color) = (0, 0, 0,1)
		_ShadowFalloff ("ShadowFalloff", float) = 1.35
		_LightDir("LightDir", vector) = (2, 2, 2, 0)
	}
	
	SubShader
	{
		Tags{ "RenderType" = "Opaque" "Queue" = "Geometry+10" }
		LOD 100
		
		Pass // 正常绘制 pass
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
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			
			ENDCG
		}
		Pass
		{
			Name "Shadow"

			//用使用模板测试以保证alpha显示正确
			Stencil
			{
				Ref 0
				Comp equal
				Pass incrWrap
				Fail keep
				ZFail keep
			}

			//透明混合模式
			Blend SrcAlpha OneMinusSrcAlpha

			//关闭深度写入
			ZWrite off

			//深度稍微偏移防止阴影与地面穿插
			Offset -1 , 0

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
			};

			float4 _LightDir;
			float4 _ShadowColor;
			float _ShadowFalloff;

			float3 ShadowProjectPos(float4 vertPos)
			{
				float3 shadowPos;

				//得到顶点的世界空间坐标
				float3 worldPos = mul(unity_ObjectToWorld , vertPos).xyz;

				//灯光方向
				float3 lightDir = normalize(_LightDir.xyz);

				//阴影的世界空间坐标（低于地面的部分不做改变）
				shadowPos.y = min(worldPos .y , _LightDir.w);
				shadowPos.xz = worldPos .xz - lightDir.xz * max(0 , worldPos .y - _LightDir.w) / lightDir.y; 

				return shadowPos;
			}

			v2f vert (appdata v)
			{
				v2f o;

				//得到阴影的世界空间坐标
				float3 shadowPos = ShadowProjectPos(v.vertex);

				//转换到裁切空间
				o.vertex = UnityWorldToClipPos(shadowPos);

				//得到中心点世界坐标
				float3 center =float3( unity_ObjectToWorld[0].w , _LightDir.w , unity_ObjectToWorld[2].w);
				//计算阴影衰减
				float falloff = 1-saturate(distance(shadowPos , center) * _ShadowFalloff);

				//阴影颜色
				o.color = _ShadowColor; 
				o.color.a *= falloff;

				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				return i.color;
			}
			ENDCG
		}
	}
}