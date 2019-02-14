// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/DepthGrayscale" {
SubShader {
Tags { "RenderType"="Opaque" }
 
Pass{
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
 
 //仍然要声明一下 _CameraDepthTexture 这个变量，虽然Unity这个变量是unity内部赋值
sampler2D _CameraDepthTexture;
float4 _MainTex_TexelSize;
 
struct v2f {
   float4 pos : SV_POSITION;
   // float2 uv : TEXCOORD0;
   float4 projPos : TEXCOORD2;
};
 
//Vertex Shader
v2f vert (appdata_base v){
   v2f o;
   o.pos = UnityObjectToClipPos (v.vertex);
   o.projPos = ComputeScreenPos (o.pos);
//    o.uv = v.texcoord.xy;
// #if UNITY_UV_STARTS_AT_TOP
// if (_MainTex_TexelSize.y < 0)
//    o.uv.y = 1 - o.uv.y;
// #endif	
   return o;
}
 
//Fragment Shader
fixed4 frag (v2f i) : SV_Target
{
   // unity 已抛弃这种方式获取深度纹理中的采样值
   // Deprecated; use SAMPLE_DEPTH_TEXTURE & SAMPLE_DEPTH_TEXTURE_PROJ instead
   //  float depth = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv));
   float depth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos));

    fixed linear01Depth = Linear01Depth(depth);
    return fixed4(linear01Depth, linear01Depth, linear01Depth, 1);
}
ENDCG
}
}
FallBack "Diffuse"
}
