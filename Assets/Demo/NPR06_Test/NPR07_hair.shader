Shader "test/NPR07_hair"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color",Color) = (1,1,1,1)

        _Ramp ("Ramp", 2D) = "white" {}
        _Bump ("Normal", 2D) = "white" {}
        _HairLightRamp ("HairLightRamp", 2D) = "white" {}
        _LightMapMask ("LightMapMask", 2D) = "white" {}

        [Space(10)][Header(xxxxxxxxxxxxxxxx)]
        _Specular("Specular",Color) = (1,1,1,1)
        _SpecularScale("SpecularScale", Range(0, 5)) = 1

        [Space(10)][Header(xxxxxxxxxxxxxxxx)]
        _MainSpecularSmooth("MainHairSpecularSmooth", Range(-10, 100)) = 1
        _FuSpecularSmooth("FuHairSpecularSmooth", Range(-10, 100)) = 1
        _MainSpecularOff("MainHairSpecularOff", Range(-10, 10)) = 1
        _FuSpecularOff("FuHairSpecularOff", Range(-10, 10)) = 1

        [Space(10)][Header(xxxxxxxxxxxxxxxx)]
        _RimPower("RimPower", Range(0.2, 10)) = 1
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass {
            Tags { "LightMode"="ForwardBase" }
            
            Cull off
            
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            
            #pragma multi_compile_fwdbase
            
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            #include "UnityShaderVariables.cginc"
            
            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _Bump;
            float4 _Bump_ST;

            sampler2D _Ramp;

            fixed4 _Specular;
            fixed _SpecularScale;

            fixed _MainSpecularSmooth;
            fixed _FuSpecularSmooth;
            float _MainSpecularOff;
            float _FuSpecularOff;

            sampler2D _HairLightRamp;
            float4 _HairLightRamp_ST;

            float _RimPower;

            sampler2D _LightMapMask;
            
            struct a2v {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
                float4 tangent : TANGENT;
            }; 
            
            struct v2f {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                
                SHADOW_COORDS(3)
                float3 tangent : TEXCOORD4;
                float2 hairLightUV:TEXCOORD5;
                float2 uv_Bump : TEXCOORD6;
                float3 normal : TEXCOORD7;
            };
            
            v2f vert (a2v v) {
                v2f o;
                
                o.pos = UnityObjectToClipPos( v.vertex);
                o.normal = v.normal;
                o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
                o.hairLightUV = TRANSFORM_TEX(v.texcoord, _HairLightRamp);
                o.uv_Bump = TRANSFORM_TEX(v.texcoord, _Bump);
                o.worldNormal  = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                TRANSFER_SHADOW(o);

                //求出沿着发梢到发根方向的切线
                half4 p_tangent = mul(unity_ObjectToWorld, v.tangent);
                o.tangent = normalize(p_tangent).xyz;
                o.tangent = cross(o.tangent, o.worldNormal);
                return o;
            }
            
            float3 ShiftTangent(float3 T, float3 N, float shift)
            {
                float3 shiftedT = T + (shift * N);
                return normalize(shiftedT);
            }

            float StrandSpecular(float3 T, float3 V, float3 L, float exponent)
            {
                float3 halfDir = normalize(L + V);
                float dotTH = dot(T, halfDir);
                float sinTH = max(0.01,sqrt(1 - pow(dotTH, 2)));
                float dirAtten = smoothstep(-1, 0, dotTH);
                return dirAtten * pow(sinTH, exponent);
            }
            
            float3 LightMapColor(fixed3 worldLightDir,fixed3 worldNormalDir, fixed2 uv)
            {
                float LdotN = max(0, dot(worldLightDir, worldNormalDir));
                float3 lightColor = LdotN * tex2D(_LightMapMask, uv);
                return lightColor;
            }
            
            float4 frag(v2f i) : SV_Target { 
                fixed3 worldNormal = normalize(i.worldNormal);
                // fixed3 tangentNormal = UnpackNormal(tex2D(_Bump, i.uv_Bump)); // 暂时没用 法线图
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                fixed3 worldHalfDir = normalize(worldLightDir + worldViewDir);
                
                //漫反射贴图采样
                fixed4 c = tex2D (_MainTex, i.uv);
                fixed3 albedo = c.rgb * _Color.rgb;
                
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
                
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos); //阴影值计算
                
                fixed diff =  dot(worldNormal, worldLightDir); //世界空间的法线坐标和光照方向点乘得到漫反射颜色
                diff = (diff * 0.5 + 0.5) * atten; //暗部提亮  当然这里也可以不提亮
                
                //将光线颜色和环境光颜色以及梯度图采样相乘得到最终的漫反射颜色
                fixed3 diffuse = _LightColor0.rgb * albedo * tex2D(_Ramp, float2(diff, diff)).rgb;
                
                //头发高光图采样, 使用一张扰动图沿着物体切线方向扰动法线来达到效果
                float3 speTex = tex2D(_HairLightRamp, i.hairLightUV);
                //头发 主高光 偏移				
                float3 Ts = ShiftTangent(i.tangent, worldNormal, _MainSpecularOff * speTex);
                //头发 副高光 偏移
                float3 Tf = ShiftTangent(i.tangent, worldNormal, _FuSpecularOff * speTex);
                
                //头发 主副 高光值
                float specMain = StrandSpecular(Ts, worldViewDir, worldLightDir, _MainSpecularSmooth);
                float specFu = StrandSpecular(Tf, worldViewDir, worldLightDir, _FuSpecularSmooth);
                
                float specFinal = specMain;
                // 这里可以添加一个副高光
                // float specMask = tex2D(tSpecMask, uv); 
                // specFinal +=  specMask * specFu;
                // specFinal += specFu;

                specFinal *= _SpecularScale ;
                
                fixed3 specular = _Specular.rgb * specFinal * atten;
                
                //rim light term
                half rim = 1.0 - saturate(dot(worldViewDir, worldNormal));
                rim = pow(rim, _RimPower);
                
                fixed3 ao = LightMapColor(worldLightDir, worldNormal,i.uv).rgb;
                
                return fixed4((ambient + diffuse + specular + rim) /* * ao */, 1.0 );
            }
            
            ENDCG
        }
    }
    FallBack "Diffuse"
}
