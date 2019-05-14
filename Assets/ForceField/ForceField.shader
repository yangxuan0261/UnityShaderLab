/*
Created by chenjd
http://www.cnblogs.com/murongxiaopifu/
*/
Shader "chenjd/ForceField"
{
	Properties
	{
		_Color("Color", Color) = (0,0,0,0)
		_NoiseTex("NoiseTexture", 2D) = "white" {}
		_DistortStrength("DistortStrength", Range(0,1)) = 0.2
		_DistortTimeFactor("DistortTimeFactor", Range(0,1)) = 0.2
		_RimStrength("RimStrength",Range(0, 10)) = 2
		_IntersectPower("IntersectPower", Range(0, 3)) = 2
	}

	SubShader
	{
		ZWrite Off
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha

		Tags
		{
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
		}

		GrabPass
		{
			"_GrabTempTex"
		}

		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 screenPos : TEXCOORD1;
				float4 grabPos : TEXCOORD2;
				float3 normal : NORMAL;
				float3 viewDir : TEXCOORD3;
			};

			sampler2D _GrabTempTex;
			float4 _GrabTempTex_ST;
			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;
			float _DistortStrength;
			float _DistortTimeFactor;
			float _RimStrength;
			float _IntersectPower;

			sampler2D _CameraDepthTexture;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.grabPos = ComputeGrabScreenPos(o.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _NoiseTex);
				o.screenPos = ComputeScreenPos(o.vertex); // ComputeScreenPos 
				COMPUTE_EYEDEPTH(o.screenPos.z); // 计算出模型顶点位置的 视空间 z 值, 保存到 o.screenPos.z 中, 等下需要用来与 场景视空间的 z 值比较
				// #	define COMPUTE_EYEDEPTH(o) o = -UnityObjectToViewPos( v.vertex ).z
				// 注意, 这个宏使用到了 v.vertex, 所以结构体 appdata 对象空间的顶点 位置 的命名一定要是 vertex

				o.normal = UnityObjectToWorldNormal(v.normal);
				o.viewDir = normalize(UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, v.vertex)));
				return o;
			}

			fixed4 _Color;


			fixed4 frag(v2f i) : SV_Target
			{
				//判断相交
				// #   	define SAMPLE_DEPTH_TEXTURE_PROJ(sampler, uv) (tex2Dproj(sampler, uv).r)
				// #	define UNITY_PROJ_COORD(a) a
				float depth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)); // 用模型的 屏幕坐标([0, 1]区间) 去 采样 场景深度图 获取场景 深度值 (非线性的 [0, 1] 区间)
				// float depth = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)).r; // 等价于上一行代码

				float sceneZ = LinearEyeDepth(depth); // 转换到 视空间 的值

				float partZ = i.screenPos.z;

				float diff = sceneZ - partZ;
				float intersect = (1 - diff) * _IntersectPower;

				//圆环
				float rim = 1 - abs(dot(i.normal, normalize(i.viewDir))) * _RimStrength;
				float glow = max(intersect, rim);

				//扭曲
				float4 offset = tex2D(_NoiseTex, i.uv - _Time.xy * _DistortTimeFactor);
				// i.grabPos.xy -= offset.xy * _DistortStrength;
				fixed4 color = tex2Dproj(_GrabTempTex, i.grabPos);

				fixed4 col = _Color * glow + color;
				return col;
			}

			ENDCG
		}
	}
}