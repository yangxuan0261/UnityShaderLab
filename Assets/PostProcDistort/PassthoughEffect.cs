/********************************************************************
 FileName: PassthoughEffect.cs
 Description: "传说中的穿越"效果
 Created: 2017/05/07
 by ：puppet_master
*********************************************************************/
using UnityEngine;
 
public class PassthoughEffect : PostEffectBase
{
 
    //扭曲强度
    [Range(0, 0.15f)]
    public float distortFactor = 1.0f;
    //扭曲中心（0-1）屏幕空间，默认为中心点
    public Vector2 distortCenter = new Vector2(0.5f, 0.5f);
 
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_Material)
        {
            _Material.SetFloat("_DistortFactor", distortFactor);
            _Material.SetVector("_DistortCenter", distortCenter);
            Graphics.Blit(source, destination, _Material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
 
    }
}
