/********************************************************************
 FileName: VolumeRayMarchingPostEffect.cs
 Description:体积光后处理脚本，用于模糊+叠加
 Created: 2018/04/29
 history: 29:4:2018 1:47 by puppet_master
*********************************************************************/
using UnityEngine;

public class VolumeRayMarchingPostEffect : PostEffectBase {
    //分辨率
    public int downSample = 1;
    //采样率
    public int samplerScale = 1;

    private RenderTexture volumeLightRT = null;

    public void RegistVolumeLightRT(RenderTexture rt) {
        volumeLightRT = rt;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (_Material && volumeLightRT) {
            Graphics.Blit(volumeLightRT, destination);
            //申请RT，并且分辨率按照downSameple降低
            RenderTexture tempRT = RenderTexture.GetTemporary(volumeLightRT.width >> downSample, volumeLightRT.height >> downSample, 0, source.format);

            //高斯模糊，两次模糊，横向纵向，使用pass1进行高斯模糊
            _Material.SetVector("_offsets", new Vector4(0, samplerScale, 0, 0));
            Graphics.Blit(volumeLightRT, tempRT, _Material, 0);
            _Material.SetVector("_offsets", new Vector4(samplerScale, 0, 0, 0));
            Graphics.Blit(tempRT, volumeLightRT, _Material, 0);

            _Material.SetTexture("_VolumeLightTex", volumeLightRT);
            Graphics.Blit(source, destination, _Material, 1);

            //释放申请的RT
            RenderTexture.ReleaseTemporary(tempRT);
        } else {
            Graphics.Blit(source, destination);
        }

    }
}