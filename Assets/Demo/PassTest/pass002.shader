Shader "AAA/pass/Grab" {  
    Properties {  
    }  
    SubShader {  
        Tags { "RenderType"="Opaque" }  
        LOD 200  
        
        GrabPass
        {
            //此处给出一个抓屏贴图的名称，抓屏的贴图就可以通过这张贴图来获取，而且每一帧不管有多个物体使用了该shader，只会有一个进行抓屏操作
            //如果此处为空，则默认抓屏到_GrabTexture中，但是据说每个用了这个shader的都会进行一次抓屏！
            "_GrabTempTex"
        }    

        Pass {  
            CGPROGRAM  
            #pragma vertex vert_img  
            #pragma fragment frag  
            #pragma fragmentoption ARB_precision_hint_fastest  
            #include "UnityCG.cginc"  
            sampler2D _GrabTexture;  
            fixed4 frag(v2f_img i) : COLOR  
            {  
                return tex2D(_GrabTexture, i.uv);  
            }  
            ENDCG  
        }  
    }  
    FallBack "Diffuse"  
}  