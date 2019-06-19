Shader "test/PassNameTest01"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
   	SubShader
	{
		Tags { "RenderType"="Opaque" "IgnoreProjector"="True"}
		LOD 100
		UsePass "test/PassNameBase/PassNameBase01"
		// UsePass "test/PassNameBase/PassNameBase02" // 支持多 UsePass
	}
}
