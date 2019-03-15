Shader "AAA/pass/Grab" {  
    Properties {  
    }  
    SubShader {  
        Tags { "RenderType"="Opaque" }  
        LOD 200  
        GrabPass  
        {  
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