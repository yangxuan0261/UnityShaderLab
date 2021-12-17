Shader "test/testShaderLod" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {} 
    }

    SubShader 
    {       
        Tags { "RenderType"="Opaque" "IgnoreProjector"="True"}
        LOD 600
        UsePass "test/PassNameBase/PassNameBase02" // green
    } 

    SubShader 
    {       
        Tags { "RenderType"="Opaque" "IgnoreProjector"="True"}
        LOD 500
        UsePass "test/PassNameBase/PassNameBase01" // red
    } 
}
