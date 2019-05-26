using System.Collections;
using UnityEngine;

// 参考文章: https://blog.csdn.net/puppet_master/article/details/52819874

[ExecuteInEditMode]
public class DepthTextureTest : PostEffectBase {
    void OnEnable() {
        // GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.DepthNormals;
    }

    void OnDisable() {
        // GetComponent<Camera>().depthTextureMode &= ~DepthTextureMode.Depth;
        GetComponent<Camera>().depthTextureMode &= ~DepthTextureMode.DepthNormals;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (_Material) {
            Graphics.Blit(source, destination, _Material);
        } else {
            Graphics.Blit(source, destination);
        }
    }
}