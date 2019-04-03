
// 参考: https://blog.csdn.net/mengfanye1991/article/details/51744259

Shader "color/ColorCorrect_HSV" {
    Properties{
        _MainTex("MainTex (RGB)", 2D) = "white" {}
        //Hue负责调整色相偏移. _Saturation ,_Value 负责调整画面整体的饱和度和色调
        _Hue("Hue", Range(0,359)) = 0
        _Saturation("Saturation", Range(0,3.0)) = 1.0
        _Value("Value", Range(0,3.0)) = 1.0
    }

    SubShader{
        Pass{
            Tags{ "RenderType" = "Opaque" }
            LOD 200

            Lighting Off

            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"


            sampler2D _MainTex;
            half _Hue;
            half _Saturation;
            half _Value;

            struct Input {
                float2 uv_MainTex;
            };

            //RGB to HSV
            float3 RGB2HSV(float3 rgb)
            {
                float3 hsv;
                float _max=max(rgb.x,max(rgb.y,rgb.z));
                float _min=min(rgb.x,min(rgb.y,rgb.z));

                if (rgb.x == _max)
                {
                    hsv.x = (rgb.y - rgb.z) / (_max - _min);
                }
                if (rgb.y == _max)
                {
                    hsv.x = 2 + (rgb.z - rgb.x) / (_max - _min);
                }
                if (rgb.z == _max)
                {
                    hsv.x = 4 + (rgb.x - rgb.y) / (_max - _min);
                }
                hsv.x = hsv.x * 60.0;
                if (hsv.x < 0)
                hsv.x = hsv.x + 360;
                hsv.z = _max;
                hsv.y = (_max - _min) / _max;
                return hsv;
            }

            //HSV to RGB
            float3 HSV2RGB(float3 hsv)
            {
                float R,G,B;
                if (hsv.y == 0)
                {
                    R = G = B = hsv.z;
                }
                else
                {
                    hsv.x = hsv.x / 60.0;
                    int i = (int)hsv.x;
                    float f = hsv.x - (float)i;
                    float a = hsv.z * (1 - hsv.y);
                    float b = hsv.z * (1 - hsv.y * f);
                    float c = hsv.z * (1 - hsv.y * (1 - f));
                    switch (i)
                    {
                        case 0: R = hsv.z; G = c; B = a;
                        break;
                        case 1: R = b; G = hsv.z; B = a;
                        break;
                        case 2: R = a; G = hsv.z; B = c;
                        break;
                        case 3: R = a; G = b; B = hsv.z;
                        break;
                        case 4: R = c; G = a; B = hsv.z;
                        break;
                        case 5: R = hsv.z; G = a; B = b;
                        break;
                    }
                }
                return float3(R,G,B);
            }

            fixed4 frag(v2f_img i) : SV_Target
            {
                fixed4 original = tex2D(_MainTex, i.uv); //获取原始贴图颜色

                float3 colorHSV;
                colorHSV.xyz = RGB2HSV(original.xyz); //转换为HSV
                colorHSV.x += _Hue; //调整 色相 ，实现色相偏移
                colorHSV.x = colorHSV.x % 360; //超过360的值从0开始

                colorHSV.y *= _Saturation; //调整饱和度
                colorHSV.z *= _Value;//调整色调

                original.xyz = HSV2RGB(colorHSV.xyz); //将调整后的HSV，转换为RGB颜色

                return original;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}