/********************************************************************
 FileName: EdgeEffectDepthNormal.cs
 Description: 后处理描边效果，使用DepthNormalTexture进行检测
 history: 13:11:2018 by puppet_master
 https://blog.csdn.net/puppet_master
*********************************************************************/
using UnityEngine;
 
[ExecuteInEditMode]
public class EdgeEffectDepthNormal : MonoBehaviour
{
    private Material edgeEffectMaterial = null;
    public Color edgeColor = Color.black;
    public Color nonEdgeColor = Color.white;
    [Range(1, 5)]
    public int sampleRange = 1;
    [Range(0, 1.0f)]
    public float normalDiffThreshold = 0.2f;
    [Range(0, 5.0f)]
    public float depthDiffThreshold = 2.0f;
 
    private void Awake()
    {
        var shader = Shader.Find("Edge/EdgeEffectDepthNormal");
        edgeEffectMaterial = new Material(shader);
    }
 
    private void OnEnable()
    {
        var cam = GetComponent<Camera>();
        cam.depthTextureMode |= DepthTextureMode.DepthNormals;
    }
 
    private void OnDisable()
    {
        var cam = GetComponent<Camera>();
        cam.depthTextureMode = DepthTextureMode.None;
    }
 
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        edgeEffectMaterial.SetColor("_EdgeColor", edgeColor);
        edgeEffectMaterial.SetColor("_NonEdgeColor", nonEdgeColor);
        edgeEffectMaterial.SetFloat("_SampleRange", sampleRange);
        edgeEffectMaterial.SetFloat("_NormalDiffThreshold", normalDiffThreshold);
        edgeEffectMaterial.SetFloat("_DepthDiffThreshold", depthDiffThreshold);
        Graphics.Blit(source, destination, edgeEffectMaterial);
    }
}
