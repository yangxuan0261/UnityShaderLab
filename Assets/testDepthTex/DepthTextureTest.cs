using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class DepthTextureTest : PostEffectBase
{
    void OnEnable()
    {
		Debug.Log("--- OnEnable");
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
    }

    void OnDisable()
    {
        GetComponent<Camera>().depthTextureMode &= ~DepthTextureMode.Depth;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_Material)
        {
			Debug.Log("--- has material");
            Graphics.Blit(source, destination, _Material);
        }
    }
}