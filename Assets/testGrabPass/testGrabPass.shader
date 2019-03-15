// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//Grabpass shader
//by: puppet_master
//2017.4.23

// GrabPass非常耗时，在安卓平台也会有问题，虽然对于安卓机的性能，用shader lod直接干掉扭曲效果也是一个不错的选择，不过这个毕竟是下策，首先还是要解决这个问题。正常渲染是往frame buffer中渲染，但是grabpass应该是从当前的frame buffer中将内容再读出来，从显存往内存中拷贝

Shader "ITS/test/testGrabPass" 
{
	SubShader
	{
		ZWrite Off
		//GrabPass
		GrabPass
		{
			//此处给出一个抓屏贴图的名称，抓屏的贴图就可以通过这张贴图来获取，而且每一帧不管有多个物体使用了该shader，只会有一个进行抓屏操作
			//如果此处为空，则默认抓屏到_GrabTexture中，但是据说每个用了这个shader的都会进行一次抓屏！
			"_GrabTempTex"
		}

		Pass
		{
			Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
			

			CGPROGRAM
			sampler2D _GrabTempTex;
			float4 _GrabTempTex_ST;
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 grabPos : TEXCOORD0;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				//计算抓屏的位置，其中主要是将坐标从(-1,1)转化到（0,1）空间并处理DX和GL纹理反向的问题
				o.grabPos = ComputeGrabScreenPos(o.pos);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//根据抓屏位置采样Grab贴图,tex2Dproj等同于tex2D(grabPos.xy / grabPos.w)
				fixed4 color = tex2Dproj(_GrabTempTex, i.grabPos);
				return 1 - color;
			}

			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
}