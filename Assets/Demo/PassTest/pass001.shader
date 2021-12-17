Shader "AAA/pass/Pass" {  
    Properties {  
    }  
    SubShader {     
        Tags { "RenderType"="Opaque" }  
        LOD 200  
        Pass {  
            CGPROGRAM  
            #pragma vertex vert_img  
            #pragma fragment frag  
            #pragma fragmentoption ARB_precision_hint_fastest  
            #include "UnityCG.cginc"  
            fixed4 frag(v2f_img i) : COLOR  
            {  
                return fixed4(1,0,0,1);  
            }  
            ENDCG  
        }  
        Pass {  
            Blend One One  //颜色叠加
            CGPROGRAM  
            #pragma vertex vert_img  
            #pragma fragment frag  
            #pragma fragmentoption ARB_precision_hint_fastest  
            #include "UnityCG.cginc"  
            fixed4 frag(v2f_img i) : COLOR  
            {  
                return fixed4(0,1,0,1);  
            }  
            ENDCG  
        }  
    }  
    FallBack "Diffuse"  
}  