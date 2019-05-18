#ifndef __PROEDURAL_NOISE__
    #define __PROEDURAL_NOISE__

    #include "Math.cginc"
    #include "Noise.cginc"

    float3 CausticTriTwist(float2 uv,float time ) {
        const int MAX_ITER = 5;
        float2 p = fmod(uv*PI2,PI2 )-250.0;

        float2 i = float2(p);
        float c = 1.0;
        float inten = .005;

        for (int n = 0; n < MAX_ITER; n++) 
        {
            float t = time * (1.0 - (3.5 / float(n+1)));
            i = p + float2(cos(t - i.x) + sin(t + i.y), sin(t - i.y) + cos(t + i.x));
            c += 1.0/length(float2(p.x / (sin(i.x+t)/inten),p.y / (cos(i.y+t)/inten)));
        }
        
        c /= float(MAX_ITER);
        c = 1.17-pow(c, 1.4);
        float val = pow(abs(c), 8.0);
        return val;
    }

    // 
    float CausticVoronoi(float2 p,float time) {
        float v = 0.0;
        float a = 0.4;
        for (int i = 0;i<3;i++) {
            v+= WNoise(p,time)*a;
            p*=2.0;
            a*=0.5;
        }
        v = pow(v,2.)*5.;
        return v;
    }

    // unity 官方 Voronoi 节点: https://docs.unity3d.com/Packages/com.unity.shadergraph@6.5/manual/Voronoi-Node.html
    inline float2 unity_voronoi_noise_randomVector (float2 UV, float offset)
    {
        float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
        UV = frac(sin(mul(UV, m)) * 46839.32);
        return float2(sin(UV.y*+offset)*0.5+0.5, cos(UV.x*offset)*0.5+0.5);
    }

    void Unity_Voronoi_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
    {
        float2 g = floor(UV * CellDensity);
        float2 f = frac(UV * CellDensity);
        float t = 8.0;
        float3 res = float3(8.0, 0.0, 0.0);

        for(int y=-1; y<=1; y++)
        {
            for(int x=-1; x<=1; x++)
            {
                float2 lattice = float2(x,y);
                float2 offset = unity_voronoi_noise_randomVector(lattice + g, AngleOffset);
                float d = distance(lattice + offset, f);
                if(d < res.x)
                {
                    res = float3(d, offset.x, offset.y);
                    Out = res.x;
                    Cells = res.y;
                }
            }
        }
    }

    // unity 官方 SimpleNoise 节点: https://docs.unity3d.com/Packages/com.unity.shadergraph@6.5/manual/Voronoi-Node.html
    inline float unity_noise_randomValue (float2 uv)
    {
        return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453);
    }

    inline float unity_noise_interpolate (float a, float b, float t)
    {
        return (1.0-t)*a + (t*b);
    }

    inline float unity_valueNoise (float2 uv)
    {
        float2 i = floor(uv);
        float2 f = frac(uv);
        f = f * f * (3.0 - 2.0 * f);

        uv = abs(frac(uv) - 0.5);
        float2 c0 = i + float2(0.0, 0.0);
        float2 c1 = i + float2(1.0, 0.0);
        float2 c2 = i + float2(0.0, 1.0);
        float2 c3 = i + float2(1.0, 1.0);
        float r0 = unity_noise_randomValue(c0);
        float r1 = unity_noise_randomValue(c1);
        float r2 = unity_noise_randomValue(c2);
        float r3 = unity_noise_randomValue(c3);

        float bottomOfGrid = unity_noise_interpolate(r0, r1, f.x);
        float topOfGrid = unity_noise_interpolate(r2, r3, f.x);
        float t = unity_noise_interpolate(bottomOfGrid, topOfGrid, f.y);
        return t;
    }

    void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
    {
        float t = 0.0;

        float freq = pow(2.0, float(0));
        float amp = pow(0.5, float(3-0));
        t += unity_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

        freq = pow(2.0, float(1));
        amp = pow(0.5, float(3-1));
        t += unity_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

        freq = pow(2.0, float(2));
        amp = pow(0.5, float(3-2));
        t += unity_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

        Out = t;
    }
    
#endif // __PROEDURAL_NOISE__
