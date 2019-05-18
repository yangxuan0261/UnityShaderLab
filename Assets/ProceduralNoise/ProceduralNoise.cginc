#ifndef __PROEDURAL_NOISE__
    #define __PROEDURAL_NOISE__

    #define PI2 6.28318530718

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

    // unity 官方: https://docs.unity3d.com/Packages/com.unity.shadergraph@6.5/manual/Voronoi-Node.html

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

#endif // __PROEDURAL_NOISE__
