
// 参考: https://blog.csdn.net/u012941657/article/details/47660969

Shader "color/ColorCorrect_BSC" {
    Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BrightnessAmount("Brightess Amount",Range(0.0,5)) = 1.0
        _satAmount("Saturation Amount",Range(0.0,5)) = 1.0
        _conAmount("Contrast Amount",Range(0.0,5)) = 1.0
    }
    SubShader {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"
            uniform sampler2D _MainTex;
            fixed _BrightnessAmount;
            fixed _satAmount;
            fixed _conAmount;

            float3 ContrastSaturationBrightness(float3 color,float brt,float sat,float con)
            {
                float AvgLumR = 0.5;
                float AvgLumG =0.5;
                float AvgLumB = 0.5;
                float3 LuminanceCoeff = float3(0.2125,0.7154,0.0721);
                float3 AvgLumin = float3(AvgLumR,AvgLumG,AvgLumB);
                float3 brtColor = color * brt;
                float intensityf = dot(brtColor,LuminanceCoeff);
                float3 intensity = float3(intensityf,intensityf,intensityf);

                float3 satColor = lerp(intensity,brtColor,sat);

                float3 conColor = lerp(AvgLumin,satColor,con);
                return conColor;
            }

            fixed4 frag(v2f_img i) : COLOR
            {
                fixed4 renderTex=tex2D(_MainTex,i.uv);
                renderTex.rgb = ContrastSaturationBrightness(renderTex.rgb,_BrightnessAmount,_satAmount,_conAmount);
                return renderTex;
            }
            ENDCG
        } 


    }
}