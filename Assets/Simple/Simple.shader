// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Custom/Simple Shader" {
	SubShader {
        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            
			struct a2v {
                float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
            };
            
            //使用一个结构体来定义顶点着色器的输出和片元着色器的输入
            struct v2f {
		        //SV_POSITION语义：pos里包含了顶点在裁剪空间中的位置
                float4 pos : SV_POSITION;
		        //COLOR0语义：可以用于存储颜色信息
                fixed3 color : COLOR0;
            };
            
            v2f vert(a2v v) {
		        //声明输出结构v2f
            	v2f o;
            	o.pos = UnityObjectToClipPos(v.vertex);
		        //这是把v.normal（法线数据）转换成颜色信息存在o.color中
            	o.color = v.normal * 0.5 + fixed3(0.5, 0.5, 0.5);
                return o;
            }
	    
   	        //v2f结构作为参数传递给片元着色器
            fixed4 frag(v2f i) : SV_Target {
		        //将插值后的i.color显示到屏幕上
                return fixed4(i.color, 1.0);
            }

            ENDCG
        }
    }
}