using UnityEngine;
using System.Collections;
 
//编辑状态下也运行
[ExecuteInEditMode]
//继承自PostEffectBase
public class BlurJunzhi : PostEffectBase
{
    //模糊半径
    public float BlurRadius = 1.0f;
 
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_Material)
        {
 
            //blur 
            _Material.SetFloat("_BlurRadius", BlurRadius);
            Graphics.Blit(source, destination, _Material);
 
        }
    }
}
