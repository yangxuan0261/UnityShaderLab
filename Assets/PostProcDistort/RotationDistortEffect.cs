/********************************************************************
 FileName: PassthoughEffect.cs
 Description: 漩涡扭曲效果
 Created: 2017/05/10
 by ：puppet_master
*********************************************************************/
using UnityEngine;
using System.Collections;
 
public class RotationDistortEffect : PostEffectBase
{
 
    //收缩强度
    [Range(0, 20.0f)]
    public float distortFactor = 1.0f;
    //扭曲中心（0-1）屏幕空间，默认为中心点
    public Vector2 distortCenter = new Vector2(0.5f, 0.5f);
    //噪声图
    public Texture NoiseTexture = null;
    //屏幕扰动强度
    [Range(0, 2.0f)]
    public float distortStrength = 1.0f;
 
    //屏幕扭曲时间
    public float passThoughTime = 3.0f;
    //当前时间
    private float currentTime = 0.0f;
    //曲线控制权重
    public float rotationCurveFactor = 10.0f;
    //屏幕全传效果曲线控制
    public AnimationCurve rotationCurve;
    //扰动曲线系数
    public float distortCurveFactor = 0.1f;
    //屏幕扰动效果曲线控制
    public AnimationCurve distortCurve;
 
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_Material)
        {
            _Material.SetTexture("_NoiseTex", NoiseTexture);
            _Material.SetFloat("_DistortFactor", distortFactor);
            _Material.SetVector("_DistortCenter", distortCenter);
            _Material.SetFloat("_DistortStrength", distortStrength);
            Graphics.Blit(source, destination, _Material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
 
    //ContexMenu，可以直接在Component上右键调用该函数，比较好用的小技巧哈
    [ContextMenu("Play")]
    public void StartPassThoughEffect()
    {
        currentTime = 0.0f;
        StartCoroutine(UpdatePassthoughEffect());
    }
 
    private IEnumerator UpdatePassthoughEffect()
    {
        while (currentTime < passThoughTime)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / passThoughTime;
            //根据时间占比在曲线（0，1）区间采样，再乘以权重作为收缩系数
            distortFactor = rotationCurve.Evaluate(t) * rotationCurveFactor;
            distortStrength = distortCurve.Evaluate(t) * distortCurveFactor;
            yield return null;
            //结束时强行设置为0
            distortFactor = 0.0f;
            distortStrength = 0.0f;
        }
    }
}
