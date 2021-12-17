Shader "test/Tessellation" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _DispTex ("Disp Texture", 2D) = "gray" {}
        _NormalMap ("Normalmap", 2D) = "bump" {}
        _RoughnessMap ("RoughnessMap", 2D) = "white" {}
        _AoMap ("AoMap", 2D) = "white" {}
        _Displacement ("Displacement", Range(0, 1.0)) = 0.3
        _Color ("Color", color) = (1,1,1,0)
        _Metallic ("Metallic", Range(0, 1.0)) = 1.0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 300
        
        CGPROGRAM
        #include "UnityPBSLighting.cginc"
        #pragma surface surf Standard addshadow fullforwardshadows vertex:disp nolightmap
        #pragma target 4.6

        struct appdata {
            float4 vertex : POSITION;
            float4 tangent : TANGENT;
            float3 normal : NORMAL;
            float2 texcoord : TEXCOORD0;
        };

        sampler2D _DispTex;
        float _Displacement;

        void disp (inout appdata v) {
            float d = tex2Dlod(_DispTex, float4(v.texcoord.xy,0,0)).r * _Displacement; // 采样高度值
            v.vertex.xyz += v.normal * d; // 往法线偏移
        }

        struct Input {
            float2 uv_MainTex;
        };

        sampler2D _MainTex;
        sampler2D _NormalMap;
        sampler2D _RoughnessMap;
        sampler2D _AoMap;
        fixed4 _Color;
        float _Metallic;

        void surf (Input IN, inout SurfaceOutputStandard o) {
            half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));
            o.Smoothness = 1 - tex2D(_RoughnessMap, IN.uv_MainTex).r;
            o.Occlusion = tex2D(_AoMap, IN.uv_MainTex).r;
        }
        ENDCG
    }
    FallBack "Diffuse"
}