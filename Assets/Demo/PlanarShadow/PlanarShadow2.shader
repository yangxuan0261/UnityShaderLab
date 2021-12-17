// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// reference: https://zhuanlan.zhihu.com/p/42781261

Shader "Custom/PlanarShadow2"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ShadowInvLen ("ShadowInvLen", float) = 1.0 //0.4449261
		//_ShadowFalloff ("ShadowFalloff", float) = 1.35
		_Offset ("Offset", Vector) = (0, 0.01, 0, 0)
		_ShadowPlane ("ShadowPlane", Vector) = (0, 1, 0, 0)
		_ShadowFadeParams ("ShadowFadeParams", Vector) = (0, 1.5, 0.7, 0)
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

		Pass // 阴影绘制 pass
		{		
			Blend SrcAlpha  OneMinusSrcAlpha
			ZWrite Off
			Cull Back
			ColorMask RGB
			
			Stencil
			{
				Ref 0			
				Comp Equal			
				WriteMask 255		
				ReadMask 255
				//Pass IncrSat
				Pass Invert
				Fail Keep
				ZFail Keep
			}
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag

			float4 _ShadowPlane;
			float3 _ShadowProjDir;
			float3 _WorldPos;
			float _ShadowInvLen;
			float3 _ShadowFadeParams;
			// float _ShadowFalloff;
			float3 _Offset;

			
			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 xlv_TEXCOORD0 : TEXCOORD0;
				float3 xlv_TEXCOORD1 : TEXCOORD1;
			};

			v2f vert(appdata v) {
				v2f o;

				float3 lightdir = normalize(_ShadowProjDir);
				float3 worldpos = mul(unity_ObjectToWorld, v.vertex).xyz;
				// _ShadowPlane.w = p0 * n  // 平面的w分量就是p0 * n
				float distance = (_ShadowPlane.w - dot(_ShadowPlane.xyz, worldpos)) / dot(_ShadowPlane.xyz, lightdir.xyz);
				worldpos = worldpos + distance * lightdir.xyz + _Offset.xyz;
				o.vertex = mul(unity_MatrixVP, float4(worldpos, 1.0));
				o.xlv_TEXCOORD0 = _WorldPos.xyz;
				o.xlv_TEXCOORD1 = worldpos;

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float3 posToPlane_2 = (i.xlv_TEXCOORD0 - i.xlv_TEXCOORD1);
				fixed4 color;
				color.xyz = float3(0.0, 0.0, 0.0);
				
				// 下面两种阴影衰减公式都可以使用(当然也可以自己写衰减公式)
				// 1. 王者荣耀的衰减公式
				color.w = (pow((1.0 - clamp(((sqrt(dot(posToPlane_2, posToPlane_2)) * _ShadowInvLen) - _ShadowFadeParams.x), 0.0, 1.0)), _ShadowFadeParams.y) * _ShadowFadeParams.z);

				// 2. https://zhuanlan.zhihu.com/p/31504088 这篇文章介绍的另外的阴影衰减公式
				//color.w = 1.0 - saturate(distance(i.xlv_TEXCOORD0, i.xlv_TEXCOORD1) * _ShadowFalloff);

				return color;
			}
			
			ENDCG
		}
	}
}