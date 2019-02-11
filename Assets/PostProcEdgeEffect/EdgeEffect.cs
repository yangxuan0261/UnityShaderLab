/********************************************************************
 FileName: EdgeEffect.cs
 Description: 后处理描边效果，使用Roberts和Sobel算子，可调强度&检测距离
 history: 11:11:2018 by puppet_master
 https://blog.csdn.net/puppet_master
*********************************************************************/
using UnityEngine;
 
[ExecuteInEditMode]
public class EdgeEffect : MonoBehaviour
{
    public enum EdgeOperator
    {
        Sobel = 0,
        Roberts = 1,
    }
 
    private Material edgeEffectMaterial = null;
    public Color edgeColor = Color.black;
    public Color nonEdgeColor = Color.white;
    [Range(1.0f, 10.0f)]
    public float edgePower = 1.0f;
    [Range(1, 5)]
    public int sampleRange = 1;
 
    public EdgeOperator edgeOperator = EdgeOperator.Sobel;
 
    private void Awake()
    {
        var shader = Shader.Find("Edge/EdgeEffect");
        edgeEffectMaterial = new Material(shader);
    }
 
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        edgeEffectMaterial.SetColor("_EdgeColor", edgeColor);
        edgeEffectMaterial.SetColor("_NonEdgeColor", nonEdgeColor);
        edgeEffectMaterial.SetFloat("_EdgePower", edgePower);
        edgeEffectMaterial.SetFloat("_SampleRange", sampleRange);
        Graphics.Blit(source, destination, edgeEffectMaterial, (int)edgeOperator);
    }
}
