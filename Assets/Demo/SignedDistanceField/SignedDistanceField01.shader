Shader "test/sdf/SignedDistanceField01"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _BackgroundColor ("BackgroundColor", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            // #pragma fragment frag
            #pragma fragment frag2

            #include "UnityCG.cginc"
            #define fwidth(x) ( abs(ddx(x)) + abs(ddy(x)) )

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 scrPos : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            // color from the material
            fixed4 _Color;
            fixed4 _BackgroundColor;

            float4 render(float d, float3 color, float stroke) 
            {
                float anti = fwidth(d) * 1.0;
                float4 colorLayer = float4(color, 1.0 - smoothstep(-anti, anti, d));
                if (stroke < 0.000001) {
                    return colorLayer;
                }

                float4 strokeLayer = float4(float3(0.05, 0.05, 0.05), 1.0 - smoothstep(-anti, anti, d - stroke));
                return float4(lerp(strokeLayer.rgb, colorLayer.rgb, colorLayer.a), strokeLayer.a);
            }

            // ------------- 有符号距离区域
            float sdfCircle(float2 coord, float2 center, float radius)
            {
                float2 offset = coord - center;
                return sqrt((offset.x * offset.x) + (offset.y * offset.y)) - radius;
            }

            float sdfTorus(float2 coord, float2 center, float radius1, float radius2)
            {
                float2 offset = coord - center;
                return abs(sqrt((offset.x * offset.x) + (offset.y * offset.y)) - radius1) - radius2;
            }

            float sdfEclipse(float2 coord, float2 center, float a, float b)
            {
                float a2 = a * a;
                float b2 = b * b;
                return (b2 * (coord.x - center.x) * (coord.x - center.x) +
                a2 * (coord.y - center.y) * (coord.y - center.y) - a2 * b2) / (a2 * b2);
            }

            float sdfBox(float2 coord,  float2 center, float width, float height)
            {
                float2 d = abs(coord - center) - float2(width, height);
                return min(max(d.x,d.y),0.0) + length(max(d,0.0));
            }

            float sdfRoundBox(float2 coord,  float2 center, float width, float height, float r)
            {
                float2 d = abs(coord - center) - float2(width, height);
                return min(max(d.x,d.y),0.0) + length(max(d,0.0)) - r;
            }

            // ------------- 布尔运算
            // 并集
            float sdfUnion(const float a,const float b) {
                return min(a, b);
            }

            // 差集
            float sdfDifference(const float a, const float b) {
                return max(a, -b);
            }

            // 交集
            float sdfIntersection(const float a, const float b) {
                return max(a, b);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.scrPos = ComputeScreenPos(o.pos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 pixelPos = (i.scrPos.xy / i.scrPos.w)*_ScreenParams.xy;
                
                float circle = sdfCircle(pixelPos, float2(0.5, 0.5)* _ScreenParams.xy, 100);
                float torus = sdfTorus(pixelPos, float2(0.2, 0.5)* _ScreenParams.xy, 50, 20);
                float eclipse = sdfEclipse(pixelPos, float2(0.4, 0.5)* _ScreenParams.xy, 100, 50);
                float box = sdfBox(pixelPos, float2(0.6, 0.5)* _ScreenParams.xy, 70, 40);
                float roundBox = sdfRoundBox(pixelPos, float2(0.8, 0.5)* _ScreenParams.xy, 70, 40, 10);

                float4 circleLayer = render(circle, _Color, fwidth(circle) * 2.0);
                float4 torusLayer = render(torus, _Color, fwidth(torus) * 2.0);
                float4 eclipseLayer = render(eclipse, float3(0.91, 0.12, 0.39), fwidth(eclipse)* 2.0);
                float4 boxLayer = render(box, float3(0.3, 0.69, 0.31), fwidth(box)* 2.0);
                float4 roundBoxLayer = render(roundBox, float3(1, 0.76, 0.03), fwidth(roundBox)* 2.0);




                float4 col = lerp(_BackgroundColor, circleLayer, circleLayer.a);
                // col *= lerp(_BackgroundColor, torusLayer, torusLayer.a);
                // col *= lerp(_BackgroundColor, eclipseLayer, eclipseLayer.a);
                // col *= lerp(_BackgroundColor, boxLayer, boxLayer.a);
                // col *= lerp(_BackgroundColor, roundBoxLayer, roundBoxLayer.a);
                return col;
            }

            fixed4 frag2 (v2f i) : SV_Target 
            {
                float2 pixelPos = (i.scrPos.xy / i.scrPos.w)*_ScreenParams.xy;

                float circle = sdfCircle(pixelPos, float2(0.2, 0.5)* _ScreenParams.xy, 100);
                float circle2 = sdfCircle(pixelPos, float2(0.5, 0.5)* _ScreenParams.xy, 100);
                float circle3 = sdfCircle(pixelPos, float2(0.8, 0.5)* _ScreenParams.xy, 100);

                float box = sdfBox(pixelPos, float2(0.2, 0.5)* _ScreenParams.xy, 120, 70);
                float box2 = sdfBox(pixelPos, float2(0.5, 0.5)* _ScreenParams.xy, 120, 70);
                float box3 = sdfBox(pixelPos, float2(0.8, 0.5)* _ScreenParams.xy, 120, 70);

                float unionResult = sdfUnion(circle, box);
                float diffResult = sdfDifference(circle2, box2);
                float intersectResult = sdfIntersection(circle3, box3);

                float4 unionLayer = render(unionResult, float3(0.91, 0.12, 0.39), fwidth(unionResult)* 2.0);
                float4 diffLayer = render(diffResult, float3(0.3, 0.69, 0.31), fwidth(diffResult)* 2.0);
                float4 intersectLayer = render(intersectResult, float3(1, 0.76, 0.03), fwidth(intersectResult)* 2.0);

                float4 col = lerp(_BackgroundColor, unionLayer, unionLayer.a);
                col *= lerp(_BackgroundColor, diffLayer, diffLayer.a);
                col *= lerp(_BackgroundColor, intersectLayer, intersectLayer.a);
                return col;
            }
            ENDCG
        }
    }
}
