//puppet_master
//2018.4.28
//体积光：RayMarching方式实现点光源体积光效果
Shader "test/VolumeLight/RayMarchingPointLight" 
{
    
    Properties 
    {
        _TintColor ("Tint Color", Color) = (1.0,1.0,1.0,1.0)
        _ExtictionFactor("ExtictionFactor", Range(0, 0.1)) = 0.01
        _ScatterFactor("ScatterFactor", Range(0, 1)) = 1
    }
    
    Category {
        //受自身Add点光阴影
        //Name "FORWARD_DELTA"
        Tags {   "RenderType"="Opaque" "Queue" = "AlphaTest"}
        Blend SrcAlpha One
        Cull Off 
        Lighting Off 
        //不写深度，永远通过ZTest，自己做检测
        ZWrite Off 
        ZTest Always
        Fog { Color (0,0,0,0) }
        
        SubShader {
            Pass {
                
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                
                #include "UnityCG.cginc"
                #include "UnityDeferredLibrary.cginc"
                //RayMarching步进次数
                #define RAYMARCHING_STEP_COUNT 4
                
                #pragma shader_feature SHADOWS_CUBE
                #pragma shader_feature POINT
                
                fixed4 _TintColor;
                sampler2D _DitherMap;
                float4x4 _LightMatrix;
                float4x4 _CustomMVP;
                float4 _VolumeLightPos;
                float4 _MieScatteringFactor;
                float _ExtictionFactor;
                float _ScatterFactor;
                
                struct v2f {
                    float4 pos : POSITION;
                    float3 worldNormal : TEXCOORD0;
                    float3 worldPos : TEXCOORD1;
                    float4 screenUV : TEXCOORD2;
                };
                
                float MieScatteringFunc(float3 lightDir, float3 rayDir)
                {
                    //MieScattering公式
                    // (1 - g ^2) / (4 * pi * (1 + g ^2 - 2 * g * cosθ) ^ 1.5 )
                    //_MieScatteringFactor.x = (1 - g ^ 2) / 4 * pai
                    //_MieScatteringFactor.y =  1 + g ^ 2
                    //_MieScatteringFactor.z =  2 * g
                    float lightCos = dot(lightDir, -rayDir);
                    return _MieScatteringFactor.x / pow((_MieScatteringFactor.y - _MieScatteringFactor.z * lightCos), 1.5);
                }
                
                //Beer-Lambert法则
                float ExtingctionFunc(float stepSize, inout float extinction)
                {
                    float density = 1.0; //密度，暂且认为为1吧，可以采样3DNoise贴图得到
                    float scattering = _ScatterFactor * stepSize * density;
                    extinction += _ExtictionFactor * stepSize * density;
                    return scattering * exp(-extinction);
                }
                
                float4 RayMarching(float3 rayOri, float3 rayDir, float rayLength, float2 ditherUV)
                {	
                    //dither
                    float2 offsetUV = (fmod(floor(ditherUV), 4.0));
                    float ditherValue = tex2D(_DitherMap, offsetUV / 4.0).a;
                    
                    float delta = rayLength / RAYMARCHING_STEP_COUNT;
                    float3 step = rayDir * delta;
                    float3 curPos = rayOri + step * ditherValue;
                    
                    float totalAtten = 0;
                    float extinction = 0;
                    for(int t = 0; t < RAYMARCHING_STEP_COUNT; t++)
                    {
                        
                        float3 tolight = (curPos - _VolumeLightPos.xyz);
                        //光源衰减
                        float atten = 2.0;
                        float att = dot(tolight, tolight) * _MieScatteringFactor.w;
                        atten *= tex2D(_LightTextureB0, att.rr).UNITY_ATTEN_CHANNEL;
                        //Mie散射
                        atten *= MieScatteringFunc(normalize(-tolight), rayDir);
                        //传播过程中吸收
                        atten *= ExtingctionFunc(delta, extinction);
                        #if defined (SHADOWS_CUBE)
                            //阴影
                            atten *= UnityDeferredComputeShadow(tolight, 0, float2(0, 0));
                        #endif
                        totalAtten += atten;
                        curPos += step;
                    }
                    //totalAtten = 0.1;
                    float4 color = float4(totalAtten, totalAtten, totalAtten, totalAtten);
                    return color * _TintColor;
                }
                
                v2f vert (appdata_base v)
                {
                    v2f o;
                    //o.pos = UnityObjectToClipPos(v.vertex);
                    o.pos = mul(_CustomMVP, v.vertex);
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;  
                    o.screenUV = ComputeScreenPos(o.pos);
                    return o;
                }
                
                fixed4 frag (v2f i) : COLOR
                {
                    float3 worldPos = i.worldPos;
                    float3 worldCamPos = _WorldSpaceCameraPos.xyz;
                    float rayDis = length(worldCamPos - worldPos);
                    
                    float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.screenUV.xy / i.screenUV.w);
                    float linearEyeDepth = LinearEyeDepth(depth);
                    rayDis = min(rayDis, linearEyeDepth);
                    
                    return RayMarching(worldCamPos, normalize(worldPos - worldCamPos), rayDis, i.pos.xy);
                }
                ENDCG 
            }
        } 	
    }
}