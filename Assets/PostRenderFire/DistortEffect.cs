/********************************************************************
 FileName: DistortEffect.cs
 Description: 屏幕扭曲效果
 Created: 2017/04/27
 by： puppet_master
*********************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistortEffect : PostEffectBase {

    //扭曲的时间系数
    [Range(0.0f, 1.0f)]
    public float DistortTimeFactor = 0.15f;
    //扭曲的强度
    [Range(0.0f, 0.2f)]
    public float DistortStrength = 0.01f;
    //噪声图
    public Texture NoiseTexture = null;
    //渲染Mask图所用的shader
    public Shader maskObjShader = null;
    //降采样系数
    public int downSample = 4;

    private Camera mainCam = null;
    private Camera additionalCam = null;
    private RenderTexture renderTexture = null;

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_Material)
        {
            _Material.SetTexture("_NoiseTex", NoiseTexture);
            _Material.SetFloat("_DistortTimeFactor", DistortTimeFactor);
            _Material.SetFloat("_DistortStrength", DistortStrength);
            _Material.SetTexture("_MaskTex", renderTexture);
            Graphics.Blit(source, destination, _Material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

    void Awake()
    {
        //创建一个和当前相机一致的相机
        InitAdditionalCam();
    }

    private void InitAdditionalCam()
    {
        mainCam = GetComponent<Camera>();
        if (mainCam == null)
            return;

        Transform addCamTransform = transform.Find("additionalDistortCam");
        if (addCamTransform != null)
            DestroyImmediate(addCamTransform.gameObject);

        GameObject additionalCamObj = new GameObject("additionalDistortCam");
        additionalCam = additionalCamObj.AddComponent<Camera>();

        SetAdditionalCam();
    }

    private void SetAdditionalCam()
    {
        if (additionalCam)
        {
            additionalCam.transform.parent = mainCam.transform;
            additionalCam.transform.localPosition = Vector3.zero;
            additionalCam.transform.localRotation = Quaternion.identity;
            additionalCam.transform.localScale = Vector3.one;
            additionalCam.farClipPlane = mainCam.farClipPlane;
            additionalCam.nearClipPlane = mainCam.nearClipPlane;
            additionalCam.fieldOfView = mainCam.fieldOfView;
            additionalCam.backgroundColor = Color.clear;
            additionalCam.clearFlags = CameraClearFlags.Color;
            additionalCam.cullingMask = 1 << LayerMask.NameToLayer("Distort");
            additionalCam.depth = -999;
            //分辨率可以低一些
            if (renderTexture == null)
                renderTexture = RenderTexture.GetTemporary(Screen.width >> downSample, Screen.height >> downSample, 0);
        }
    }

    void OnEnable()
    {
        SetAdditionalCam();
        additionalCam.enabled = true;
    }

    void OnDisable()
    {
        additionalCam.enabled = false;
    }

    void OnDestroy()
    {
        if (renderTexture)
        {
            RenderTexture.ReleaseTemporary(renderTexture);
        }
        DestroyImmediate(additionalCam.gameObject);
    }

    //在真正渲染前的回调，此处渲染Mask遮罩图
    void OnPreRender()
    {
        //maskObjShader进行渲染
        if (additionalCam.enabled)
        {
            additionalCam.targetTexture = renderTexture;
            additionalCam.RenderWithShader(maskObjShader, "");
        }
    }
}
