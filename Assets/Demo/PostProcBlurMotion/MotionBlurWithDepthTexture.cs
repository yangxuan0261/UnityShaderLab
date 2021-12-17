using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class MotionBlurWithDepthTexture : PostEffectBase
{
 
    //定义运动模糊时模糊图像使用的大小
    [Range(0.0f, 1.0f)] public float BlurSize = 0.5f;
    //定义一个Camera变量，获取该脚本所在的摄像机组建，得到摄像机的视角和投影矩阵
    private Camera _myCamera;
 
    public Camera Camera
    {
        get
        {
            if (_myCamera == null)
            {
                _myCamera = GetComponent<Camera>();
            }
            return _myCamera;
        }
    }
 
    //定义一个变量保存 上一帧摄像机的视角 * 投影矩阵
    private Matrix4x4 _previousViewProjectionMatrix;
 
    //在OnEnable中设置摄像机的状态，以获得深度纹理
    void OnEnable()
    {
        Camera.depthTextureMode = DepthTextureMode.Depth;
    }
 
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (_Material != null)
        {
            //将模糊大小传给Shader
            _Material.SetFloat("_BlurSize", BlurSize);
 
            //使用 视角 * 投影矩阵 对NDC（归一化的设备坐标）下的顶点坐标进行变换，得到该像素在世界空间下的位置
            //计算前一帧与当前帧的位置差，生成该像素的速度
 
            //将 前一帧视角 * 投影矩阵 传给Shader
            _Material.SetMatrix("_PreviousViewProjectionMatrix", _previousViewProjectionMatrix);
            //计算 当前帧视角 * 投影矩阵
            //Camera.projectionMatrix获得当前摄像机投影矩阵
            //Camera.worldToCameraMatrix获得当前摄像机视角矩阵
            Matrix4x4 currentViewProjectionMartix = Camera.projectionMatrix * Camera.worldToCameraMatrix;
            //计算 当前帧视角 * 投影矩阵 的逆矩阵
            Matrix4x4 currentViewProjectionInverseMartix = currentViewProjectionMartix.inverse;
            //将当前帧视角 * 投影矩阵 的逆矩阵 传递给Shader
            _Material.SetMatrix("_CurrentViewProjectionInverseMartix", currentViewProjectionInverseMartix);
            //将 当前帧视角 * 投影矩阵 保存为 前一帧视角 * 投影矩阵
            _previousViewProjectionMatrix = currentViewProjectionMartix;
 
            Graphics.Blit(src, dest, _Material);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
