using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class DepthOfFiled : PostEffectBase {

    [Range(0.0f, 1.0f)]
    public float focalDistance = 1.0f;
    [Range(0.0f, 0.5f)]
    public float focalWidth = 0.02f;
    //分辨率
    public int downSample = 1;
    //采样率
    public int samplerScale = 1;

    private Camera _mainCam = null;
    public Camera MainCam {
        get {
            if (_mainCam == null)
                _mainCam = GetComponent<Camera>();
            return _mainCam;
        }
    }

    void OnEnable() {
        //maincam的depthTextureMode是通过位运算开启与关闭的
        MainCam.depthTextureMode |= DepthTextureMode.Depth;
    }

    void OnDisable() {
        MainCam.depthTextureMode &= ~DepthTextureMode.Depth;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (_Material) {
            //首先将我们设置的焦点限制在远近裁剪面之间
            Mathf.Clamp(focalDistance, MainCam.nearClipPlane, MainCam.farClipPlane);

            //申请两块RT，并且分辨率按照downSameple降低
            RenderTexture temp1 = RenderTexture.GetTemporary(source.width >> downSample, source.height >> downSample, 0, source.format);
            RenderTexture temp2 = RenderTexture.GetTemporary(source.width >> downSample, source.height >> downSample, 0, source.format);

            //直接将场景图拷贝到低分辨率的RT上达到降分辨率的效果
            Graphics.Blit(source, temp1);

            //高斯模糊，两次模糊，横向纵向，使用pass0进行高斯模糊
            _Material.SetVector("_offsets", new Vector4(0, samplerScale, 0, 0));
            Graphics.Blit(temp1, temp2, _Material, 0);
            _Material.SetVector("_offsets", new Vector4(samplerScale, 0, 0, 0));
            Graphics.Blit(temp2, temp1, _Material, 0);

            //景深操作，景深需要两的模糊效果图我们通过_BlurTex变量传入shader
            _Material.SetTexture("_BlurTex", temp1);
            //设置shader的参数，主要是焦点和远近模糊的权重，权重可以控制插值时使用模糊图片的权重
            _Material.SetFloat("_focalDistance", focalDistance);
            _Material.SetFloat("_focalWidth", focalWidth);

            //使用pass1进行景深效果计算，清晰场景图直接从source输入到shader的_MainTex中
            Graphics.Blit(source, destination, _Material, 1);

            //释放申请的RT
            RenderTexture.ReleaseTemporary(temp1);
            RenderTexture.ReleaseTemporary(temp2);
        }
    }

    //计算设置的焦点被转换到01空间中的距离，以便shader中通过这个01空间的焦点距离与depth比较
    // private float FocalDistance01(float distance) {
    //     return MainCam.WorldToViewportPoint((distance - MainCam.nearClipPlane) * MainCam.transform.forward + MainCam.transform.position).z / (MainCam.farClipPlane - MainCam.nearClipPlane);
    // }

}