using UnityEngine;
using System.Collections;

// 参考文章: https://blog.csdn.net/puppet_master/article/details/52819874

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
            Graphics.Blit(source, destination, _Material);
        }
    }
}