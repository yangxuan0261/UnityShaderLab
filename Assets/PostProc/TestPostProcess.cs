using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TestPostProcess : PostEffectBase
{
    //分辨率
    public int downSample = 1;
    //采样率
    public int samplerScale = 1;
    //高亮部分提取阈值
    public Color colorThreshold = Color.gray;
    //Bloom泛光颜色
    public Color bloomColor = Color.white;


    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_Material)
        {
            //申请两块RT，并且分辨率按照downSameple降低
            RenderTexture temp1 = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);

            //直接将场景图拷贝到低分辨率的RT上达到降分辨率的效果
            // Graphics.Blit(source, destination, _Material, 0); // pass 的索引从 0 开始
			temp1 = null; //如果为null，直接渲染source到屏幕上
			Graphics.Blit(source, temp1);
			// destination = temp1;


            //使用pass2进行景深效果计算，清晰场景图直接从source输入到shader的_MainTex中
            // Graphics.Blit(source, destination, _Material, 2);

            //释放申请的RT
            RenderTexture.ReleaseTemporary(temp1);
        }
    }
}
