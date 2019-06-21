/********************************************************************
 FileName: VolumeRayMarchingLight.cs
 Description:
 Created: 2018/04/28
 history: 28:4:2018 20:33 by puppet_master
*********************************************************************/
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class VolumeRayMarchingLight : MonoBehaviour {

    private Material lightMaterial = null;

    private Light lightComponent = null;

    private Texture2D ditherMap = null;

    private CommandBuffer commandBuffer = null;

    public static RenderTexture volumeLightRT = null;

    private Renderer lightRenderer = null;

    //Mie-Scattering g 参数
    [Range(0.0f, 0.99f)]
    public float MieScatteringG = 0.0f;

    void OnEnable() {
        if (Camera.main != null)
            Camera.main.depthTextureMode = DepthTextureMode.Depth;

        Init();
        lightComponent.AddCommandBuffer(LightEvent.AfterShadowMap, commandBuffer);
    }

    void OnDisable() {
        lightComponent.RemoveCommandBuffer(LightEvent.AfterShadowMap, commandBuffer);
        if (Camera.main != null)
            Camera.main.depthTextureMode = DepthTextureMode.None;
    }

    private void Init() {
        InitVolumeLight();
        InitCommandBuffer();
        InitPostEffectComponent();
    }

    private void InitVolumeLight() {
        lightRenderer = GetComponent<Renderer>();
        lightMaterial = lightRenderer.sharedMaterial;
        lightComponent = GetComponent<Light>();
        if (lightComponent == null)
            lightComponent = gameObject.AddComponent<Light>();
        lightComponent.shadows = LightShadows.Hard;
        lightRenderer.enabled = false;
        if (ditherMap == null)
            ditherMap = GenerateDitherMap();
        if (volumeLightRT == null)
            volumeLightRT = new RenderTexture(512, 512, 16);
    }

    private void InitCommandBuffer() {
        if (commandBuffer == null)
            commandBuffer = new CommandBuffer();
        commandBuffer.Clear();
        commandBuffer.name = "RayMarchingVolumePointLight";
        commandBuffer.SetGlobalTexture("_ShadowMapTexture", BuiltinRenderTextureType.CurrentActive);
        commandBuffer.SetRenderTarget(volumeLightRT);
        commandBuffer.ClearRenderTarget(true, true, Color.black);
        commandBuffer.DrawRenderer(lightRenderer, lightMaterial);
    }

    private void InitPostEffectComponent() {
        if (Camera.main == null)
            return;
        var postEffect = Camera.main.gameObject.GetComponent<VolumeRayMarchingPostEffect>();
        if (postEffect == null)
            postEffect = Camera.main.gameObject.AddComponent<VolumeRayMarchingPostEffect>();
        postEffect.RegistVolumeLightRT(volumeLightRT);
        postEffect.shader = Shader.Find("GodRay/VolumeLightRayMarchingPostEffect");
    }

    void Update() {
        if (lightMaterial == null || lightComponent == null)
            return;
        //世界->光源矩阵
        Matrix4x4 lightMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one).inverse;
        transform.localScale = new Vector3(lightComponent.range * 2.0f, lightComponent.range * 2.0f, lightComponent.range * 2.0f);

        lightMaterial.EnableKeyword("POINT");
        if (lightComponent.shadows == LightShadows.None) {
            lightMaterial.DisableKeyword("SHADOWS_CUBE");
        } else {
            lightMaterial.EnableKeyword("SHADOWS_CUBE");
        }

        float g2 = MieScatteringG * MieScatteringG;
        float lightRange = lightComponent.range;
        lightMaterial.SetMatrix("_LightMatrix", lightMatrix);
        lightMaterial.SetVector("_VolumeLightPos", transform.position);
        lightMaterial.SetVector("_MieScatteringFactor", new Vector4((1 - g2) * 0.25f / Mathf.PI, 1 + g2, 2 * MieScatteringG, 1.0f / (lightRange * lightRange)));
        lightMaterial.SetTexture("_DitherMap", ditherMap);
        //自己计算MVP矩阵传给shader，用Camera.main可能导致编辑器Scene窗口显示有问题
        Matrix4x4 world = transform.localToWorldMatrix;
        Matrix4x4 proj = GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix, true);
        Matrix4x4 mat = proj * Camera.main.worldToCameraMatrix * world;
        lightMaterial.SetMatrix("_CustomMVP", mat);
    }

    private Texture2D GenerateDitherMap() {
        int texSize = 4;
        var ditherMap = new Texture2D(texSize, texSize, TextureFormat.Alpha8, false, true);
        ditherMap.filterMode = FilterMode.Point;
        Color32[] colors = new Color32[texSize * texSize];

        colors[0] = GetDitherColor(0.0f);
        colors[1] = GetDitherColor(8.0f);
        colors[2] = GetDitherColor(2.0f);
        colors[3] = GetDitherColor(10.0f);

        colors[4] = GetDitherColor(12.0f);
        colors[5] = GetDitherColor(4.0f);
        colors[6] = GetDitherColor(14.0f);
        colors[7] = GetDitherColor(6.0f);

        colors[8] = GetDitherColor(3.0f);
        colors[9] = GetDitherColor(11.0f);
        colors[10] = GetDitherColor(1.0f);
        colors[11] = GetDitherColor(9.0f);

        colors[12] = GetDitherColor(15.0f);
        colors[13] = GetDitherColor(7.0f);
        colors[14] = GetDitherColor(13.0f);
        colors[15] = GetDitherColor(5.0f);

        ditherMap.SetPixels32(colors);
        ditherMap.Apply();
        return ditherMap;
    }

    private Color32 GetDitherColor(float value) {
        byte byteValue = (byte) (value / 16.0f * 255);
        return new Color32(byteValue, byteValue, byteValue, byteValue);
    }
}