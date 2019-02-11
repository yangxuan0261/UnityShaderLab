/********************************************************************
 FileName: EdgeEffect.shader
 Description: 后处理描边效果，使用Roberts和Sobel算子，可调强度&检测距离
 history: 11:11:2018 by puppet_master
 https://blog.csdn.net/puppet_master
 参考: https://blog.csdn.net/puppet_master/article/details/83759180
*********************************************************************/
Shader "Edge/EdgeEffect"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
	struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};
	
	struct v2f
	{
		float2 uvRoberts[5] : TEXCOORD0;
		float2 uvSobel[9] : TEXCOORD5;
		
		float4 vertex : SV_POSITION;
	};
	
	sampler2D _MainTex;
	float4 _MainTex_TexelSize;
	fixed4 _EdgeColor;
	fixed4 _NonEdgeColor;
	float _EdgePower;
	float _SampleRange;
	
	float Sobel(v2f i)
	{
		const float Gx[9] = 
		{
			-1, -2, -1,
			0,  0,  0,
			1,  2,  1
		};
		
		const float Gy[9] =
		{
			1, 0, -1,
			2, 0, -2,
			1, 0, -1
		};
		
		float edgex, edgey;
		for(int j = 0; j < 9; j++)
		{
			fixed4 col = tex2D(_MainTex, i.uvSobel[j]);
			float lum = Luminance(col.rgb);
			
			edgex += lum * Gx[j];
			edgey += lum * Gy[j];
		}
		return 1 - abs(edgex) - abs(edgey);
	}
	
	float Roberts(v2f i)
	{
		const float Gx[4] = 
		{
			-1,  0,
			0,  1
		};
		
		const float Gy[4] =
		{
			0, -1,
			1,  0
		};
		
		float edgex, edgey;
		for(int j = 0; j < 4; j++)
		{
			fixed4 col = tex2D(_MainTex, i.uvRoberts[j]);
			float lum = Luminance(col.rgb);
			
			edgex += lum * Gx[j];
			edgey += lum * Gy[j];
		}
		return 1 - abs(edgex) - abs(edgey);
	}
	
	v2f vert_Sobel (appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uvSobel[0] = v.uv + float2(-1, -1) * _MainTex_TexelSize * _SampleRange;
		o.uvSobel[1] = v.uv + float2( 0, -1) * _MainTex_TexelSize * _SampleRange;
		o.uvSobel[2] = v.uv + float2( 1, -1) * _MainTex_TexelSize * _SampleRange;
		o.uvSobel[3] = v.uv + float2(-1,  0) * _MainTex_TexelSize * _SampleRange;
		o.uvSobel[4] = v.uv + float2( 0,  0) * _MainTex_TexelSize * _SampleRange;
		o.uvSobel[5] = v.uv + float2( 1,  0) * _MainTex_TexelSize * _SampleRange;
		o.uvSobel[6] = v.uv + float2(-1,  1) * _MainTex_TexelSize * _SampleRange;
		o.uvSobel[7] = v.uv + float2( 0,  1) * _MainTex_TexelSize * _SampleRange;
		o.uvSobel[8] = v.uv + float2( 1,  1) * _MainTex_TexelSize * _SampleRange;
		return o;
	}
	
	fixed4 frag_Sobel (v2f i) : SV_Target
	{
		fixed4 col = tex2D(_MainTex, i.uvSobel[4]);
		float g = Sobel(i);
		g = pow(g, _EdgePower);
		col.rgb = lerp(_EdgeColor, _NonEdgeColor, g);
	
		return col;
	}
	
	v2f vert_Roberts (appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uvRoberts[0] = v.uv + float2(-1, -1) * _MainTex_TexelSize * _SampleRange;
		o.uvRoberts[1] = v.uv + float2( 1, -1) * _MainTex_TexelSize * _SampleRange;
		o.uvRoberts[2] = v.uv + float2(-1,  1) * _MainTex_TexelSize * _SampleRange;
		o.uvRoberts[3] = v.uv + float2( 1,  1) * _MainTex_TexelSize * _SampleRange;
		o.uvRoberts[4] = v.uv;
		return o;
	}
	
	fixed4 frag_Roberts (v2f i) : SV_Target
	{
		fixed4 col = tex2D(_MainTex, i.uvRoberts[4]);
		float g = Roberts(i);
		g = pow(g, _EdgePower);
		col.rgb = lerp(_EdgeColor, _NonEdgeColor, g);
	
		return col;
	}
	
	ENDCG
	
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
 
		//Pass 0 Sobel Operator
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_Sobel
			#pragma fragment frag_Sobel
			ENDCG
		}
		
		//Pass 1 Roberts Operator
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_Roberts
			#pragma fragment frag_Roberts
			ENDCG
		}
		
		
	}
}
