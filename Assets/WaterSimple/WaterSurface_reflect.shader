// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/WaterSurface_reflect"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_RefractTex ("Refract Texture", 2D) = "white" {}
		_BumpTex ("Bump Texture", 2D) = "white"{}
		_BumpStrength ("Bump strength", Range(0.0, 10.0)) = 1.0
		_BumpDirection ("Bump direction(2 wave)", Vector)=(1,1,1,-1)
		_BumpTiling ("Bump tiling", Vector)=(0.0625,0.0625,0.0625,0.0625)
		_FresnelTex("Fresnel Texture", 2D) = "white" {}
		_Skybox("skybox", Cube)="white"{}
		_Specular("Specular Color", Color)=(1,1,1,0.5)
		_Params("shiness,Refract Perturb,Reflect Perturb", Vector)=(128, 0.025, 0.05, 0)
		_SunDir("sun direction", Vector)=(0,0,0,0)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "LightMode"="ForwardBase"}
		LOD 100

		Pass
		{
			offset 1,1
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv:TEXCOORD0;
			};

			struct v2f
			{
				half2 uv : TEXCOORD0;
				half4 bumpCoords:TEXCOORD1;
				half3 viewVector:TEXCOORD2;
				half4 screenUV : TEXCOORD3;
				half4 vertex : SV_POSITION;
			};
			half4 _Color;
			sampler2D _RefractTex;
			sampler2D _ReflectTex;
			sampler2D _BumpTex;
			half _BumpStrength;
			half4 _BumpDirection;
			half4 _BumpTiling;
			sampler2D _FresnelTex;
			samplerCUBE _Skybox;
			half4 _Specular;
			half4 _Params;
			half4 _SunDir;
			
			half3 PerPixelNormal(sampler2D bumpMap, half4 coords, half bumpStrength) 
			{
				float2 bump = (UnpackNormal(tex2D(bumpMap, coords.xy)) + UnpackNormal(tex2D(bumpMap, coords.zw))) * 0.5;
				//bump += (UnpackNormal(tex2D(bumpMap, coords.xy*2))*0.5 + UnpackNormal(tex2D(bumpMap, coords.zw*2))*0.5) * 0.5;
				//bump += (UnpackNormal(tex2D(bumpMap, coords.xy*8))*0.5 + UnpackNormal(tex2D(bumpMap, coords.zw*8))*0.5) * 0.5;
				float3 worldNormal = float3(0,0,0);
				worldNormal.xz = bump.xy * bumpStrength;
				worldNormal.y = 1;
				return worldNormal;
			}
			
			inline float FastFresnel(half3 I, half3 N, half R0)
			{
				float icosIN = saturate(1-dot(I, N));
				float i2 = icosIN*icosIN;
				float i4 = i2*i2;
				return R0 + (1-R0)*(i4*icosIN);
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				half3 worldPos = mul(unity_ObjectToWorld, v.vertex);

				o.uv.xy = v.uv;
				o.screenUV = ComputeScreenPos(o.vertex);
				o.bumpCoords.xyzw = (worldPos.xzxz + _Time.yyyy * _BumpDirection.xyzw) * _BumpTiling.xyzw;
				o.viewVector = (worldPos - _WorldSpaceCameraPos.xyz);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 result = fixed4(0,0,0,1);
				half3 worldNormal = normalize(PerPixelNormal(_BumpTex, i.bumpCoords, _BumpStrength));
				half3 viewVector = normalize(i.viewVector);
				half3 halfVector = normalize(normalize(_SunDir.xyz)-viewVector);
				half2 screenUV = i.screenUV.xy/i.screenUV.w;

				half2 offsets = worldNormal.xz*viewVector.y;
				half4 refractColor = tex2D(_RefractTex, i.uv.xy+offsets*_Params.y)*_Color;
				half4 reflectColor = tex2D(_ReflectTex, screenUV+offsets*_Params.z);
				//
				half3 reflUV = reflect( viewVector, worldNormal);
				half3 skyColor = texCUBE(_Skybox, reflUV);
				//
				half2 fresnelUV = half2( saturate(dot(-viewVector, worldNormal)), 0.5);
				half fresnel = tex2D(_FresnelTex, fresnelUV).r;

				reflectColor.xyz = lerp(skyColor, reflectColor.xyz, reflectColor.a);
				//
				if(IsGammaSpace())
				{
					fresnel = pow(fresnel, 2.2);
				}
				//fresnel = FastFresnel(-viewVector, worldNormal, 0.02);

				result.xyz = lerp(refractColor.xyz, reflectColor.xyz, fresnel);
				//spec
				half3 specularColor = _Specular.w*pow(max(0, dot(worldNormal, halfVector)), _Params.x);
				result.xyz += _Specular.xyz*specularColor;
	
				return result;
			}
			ENDCG
		}
	}
//	FallBack "Diffuse"
}
