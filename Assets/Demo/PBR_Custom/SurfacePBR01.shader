Shader "Custom/SurfacePBR01" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NormalTex ("Normal (RGB)", 2D) = "white" {}
		_MetallicTex ("Metallic (RGB)", 2D) = "white" {}
		_NoiseTex ("Noise (RGB)", 2D) = "white" {}
		_Cutoff ("CutOff", Range(0,1)) = 0
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		// Cull Off # 双面渲染

		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _NormalTex;
		sampler2D _MetallicTex;
		sampler2D _NoiseTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed _Cutoff;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color

			fixed4 nc = tex2D(_NoiseTex, IN.uv_MainTex);
			clip(nc.r - _Cutoff);

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			// o.Metallic = _Metallic;
			// o.Smoothness = _Glossiness;
			o.Alpha = c.a;

			fixed4 ms = tex2D(_MetallicTex, IN.uv_MainTex);
			o.Metallic = ms.r;
			o.Smoothness = ms.a * _Glossiness;
			o.Normal = UnpackNormal(tex2D(_NormalTex, IN.uv_MainTex)); // 采样 法线贴图 并把 贴图值0~1转成 法线值-1~1
		}
		ENDCG
	}
	FallBack "Diffuse"
}
