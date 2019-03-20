Shader "MyTest/TestShaderGUI"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BlendTex("Blend", 2D) = "white" {}

        [Toggle] _UseTwoColors("Use two colors?", Int) = 0
        _Color1("Color 1", Color) = (1,0,0,1)
        _Color2("Color 2", Color) = (0,0,1,1)
    }
    SubShader
    {
        Tags{ "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM

        #pragma surface surf Lambert addshadow

        #pragma shader_feature CS_BOOL
        #pragma shader_feature _BLENDMAP
        #pragma multi_compile _OVERLAY_NONE _OVERLAY_ADD _OVERLAY_MULTIPLY
        #pragma multi_compile _OVERLAY_NONE2 _OVERLAY_ADD2
        
        sampler2D _MainTex;

#if _BLENDMAP
        sampler2D _BlendTex;
#endif

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            half4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;

#if CS_BOOL
            o.Albedo.gb *= 0.5;
#endif

#if _BLENDMAP
        half4 bc = tex2D(_BlendTex, IN.uv_MainTex);
        o.Albedo.rgb *= bc.rgb;
#endif

        }

        ENDCG
    }
    CustomEditor "TestShaderGUI"
}