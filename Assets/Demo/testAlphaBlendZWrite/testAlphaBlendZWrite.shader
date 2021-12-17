Shader "test/Alpha Blend ZWrite" {
    Properties {
        _Color ("Main Tint", Color) = (1,1,1,1)
        _MainTex ("Main Tex", 2D) = "white" {}
        // 用于在透明纹理的基础上控制整体的透明度
        _AlphaScale ("Alpha Scale", Range(0,1 )) = 1
    }
    SubShader {

        // RenderType标签可以让Unity
        // 把这个Shader归入到提前定义的组(Transparent)
        // 用于指明该Shader是一个使用了透明度混合的Shader

        // IgnoreProjector=True这意味着该Shader
        // 不会受到投影器(Projectors)的影响

        // 为了使用透明度混合的Shader一般都应该在SubShader
        // 中设置这三个标签

        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}

        // 仅仅是把模型的深度信息写入深度缓冲中
        // 从而剔除模型中被自身遮挡的片元
        Pass {
            // 开启深度写入
            ZWrite On
            // 用于设置颜色通道的写掩码(Wirte Mask)
            // ColorMask RGB|A|0|(R/G/B/A组合)
            // 当为0时意味着该Pass不写入任何颜色通道，也就不会输出任何颜色
            ColorMask 0
        }

        Pass {

            // 向前渲染路径的方式
            Tags {"LightMode"="ForwardBase"}

            // 深度写入设置为关闭状态
            ZWrite Off

            // 这是混合模式
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Lighting.cginc"

            fixed4  _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _AlphaScale;

            struct a2v {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

             v2f vert(a2v v) {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);

                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target {

                fixed3 worldNormal = normalize(i.worldNormal);

                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));

                fixed4 texColor = tex2D(_MainTex, i.uv);

                fixed3 albedo = texColor.rgb * _Color.rgb;

                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;

                fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(worldNormal, worldLightDir));

                // 设置透明度通道的值
                return fixed4 (ambient + diffuse ,texColor.a * _AlphaScale);
            }

            ENDCG
        }

    }
    FallBack "Transparent/VertexLit"
}
