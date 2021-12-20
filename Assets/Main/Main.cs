using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Demo {

    public class Main : MonoBehaviour, IEnhancedScrollerDelegate {

        public EnhancedScroller scroller;
        public CellView cellView;

        public GameObject canvasRoot;
        public GameObject sclRoot;
        public GameObject backBtn;

        private List<Data> mDataLst = new List<Data>();

        void Start() {
            GameObject.DontDestroyOnLoad(canvasRoot);
            OnBtnShowScroller();

            scroller.Delegate = this;
            scroller.cellViewInstantiated = (scl, cv) => {
                CellView mycv = (CellView) cv;
                mycv.gameObject.SetActive(true);
                mycv.onClickFn = OnBtnItem;
            };

            InitData();
        }

        public int GetNumberOfCells(EnhancedScroller scroller) {
            return mDataLst.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) {
            return 50;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex) {
            CellView cv = (CellView) scroller.GetCellView(cellView);
            cv.SetData(mDataLst[dataIndex], dataIndex);
            return cv;
        }

        // UI 处理
        public void OnBtnItem(CellView cv, Data data) {
            sclRoot.SetActive(false);
            backBtn.SetActive(true);

            SceneManager.LoadScene(data.name);
        }

        public void OnBtnShowScroller() {
            sclRoot.SetActive(true);
            backBtn.SetActive(false);
        }

        // 场景配置
        void InitData() {
            mDataLst.Add(new Data { title = "动态合批", name = "DynamicBatch" });
            mDataLst.Add(new Data { title = "静态合批", name = "StaticBatch" });
            mDataLst.Add(new Data { title = "GpuInstancing", name = "GpuInstancing" });
            mDataLst.Add(new Data { title = "GpuInstancing2", name = "GpuInstancing2" });
            mDataLst.Add(new Data { title = "颜色校正", name = "ColorCorrect" });
            mDataLst.Add(new Data { title = "CommandBuffer", name = "CommandBuffer" });
            mDataLst.Add(new Data { title = "CommandBuffer 后处理描边", name = "OutlinePostEffectCmdBuffer" });
            mDataLst.Add(new Data { title = "后处理渲染", name = "RenderAfterPostEffect" });
            mDataLst.Add(new Data { title = "计算屏幕坐标", name = "ComputeScreenPos" });
            mDataLst.Add(new Data { title = "ComputeShader", name = "ComputeShader" });
            mDataLst.Add(new Data { title = "Cubemap", name = "Cubemap" });
            mDataLst.Add(new Data { title = "ddxddy 偏导数, 求法线", name = "ddxddy" });
            mDataLst.Add(new Data { title = "延迟渲染", name = "DeferredRendering" });
            mDataLst.Add(new Data { title = "x - filling-the-g-buffers", name = "filling-the-g-buffers" });
            mDataLst.Add(new Data { title = "pbr", name = "pbr" });
            mDataLst.Add(new Data { title = "能量罩", name = "ForceFieldScene" });
            mDataLst.Add(new Data { title = "几何着色器", name = "Geometry02" });
            mDataLst.Add(new Data { title = "玻璃模糊效果", name = "Glass" });
            mDataLst.Add(new Data { title = "抓取显存内容-GrabPass", name = "GrabPass" });
            mDataLst.Add(new Data { title = "草", name = "Grass" });
            mDataLst.Add(new Data { title = "头发, 各向异性", name = "HairDemo" });
            mDataLst.Add(new Data { title = "HDR", name = "HDR" });
            mDataLst.Add(new Data { title = "光照图", name = "Lightmap" });
            mDataLst.Add(new Data { title = "光照模型", name = "LightModel" });
            mDataLst.Add(new Data { title = "光照探针-LightProbe", name = "LightProbe" });
            mDataLst.Add(new Data { title = "LodGroup", name = "LodGroup" });
            mDataLst.Add(new Data { title = "MatCap", name = "MatCap" });
            mDataLst.Add(new Data { title = "Matrix", name = "Matrix" });
            mDataLst.Add(new Data { title = "顶点中的多套 uv 设置", name = "MultiUVSets" });
            mDataLst.Add(new Data { title = "卡通渲染-NPR", name = "NPR" });
            mDataLst.Add(new Data { title = "卡通渲染-CelShading", name = "NPR02_CelShading" });
            mDataLst.Add(new Data { title = "素描效果-PencilSketchScene", name = "PencilSketchScene" });
            mDataLst.Add(new Data { title = "风格化效果-StylizedHighlightsScene", name = "StylizedHighlightsScene" });
            mDataLst.Add(new Data { title = "ToneBasedShadingScene", name = "ToneBasedShadingScene" });
            mDataLst.Add(new Data { title = "NPR06_Test", name = "NPR06_Test" });
            mDataLst.Add(new Data { title = "OcclusionCulling", name = "OcclusionCulling" });
            mDataLst.Add(new Data { title = "Oil", name = "Oil" });
            mDataLst.Add(new Data { title = "ShadowTest", name = "ShadowTest" });
            mDataLst.Add(new Data { title = "Outline and ScreenSpace texture", name = "Outline and ScreenSpace texture" });
            mDataLst.Add(new Data { title = "PassName", name = "PassName" });
            mDataLst.Add(new Data { title = "passtest", name = "passtest" });
            mDataLst.Add(new Data { title = "PBR01", name = "PBR01" });
            mDataLst.Add(new Data { title = "PBR_Custom", name = "PBR_Custom" });
            mDataLst.Add(new Data { title = "PBR_Fake", name = "PBR_Fake" });
            mDataLst.Add(new Data { title = "ChannelCopy", name = "ChannelCopy" });
            mDataLst.Add(new Data { title = "PhotoshopBlend", name = "PhotoshopBlend" });
            mDataLst.Add(new Data { title = "PlanarShadow", name = "PlanarShadow" });
            mDataLst.Add(new Data { title = "AlphaCaptureDemo", name = "AlphaCaptureDemo" });
            mDataLst.Add(new Data { title = "Camera360CaptureDemo", name = "Camera360CaptureDemo" });
            mDataLst.Add(new Data { title = "Camera360StereoCaptureDemo", name = "Camera360StereoCaptureDemo" });
            mDataLst.Add(new Data { title = "CameraCaptureDemo", name = "CameraCaptureDemo" });
            mDataLst.Add(new Data { title = "ScreenCaptureDemo", name = "ScreenCaptureDemo" });
            mDataLst.Add(new Data { title = "TextureCaptureDemo", name = "TextureCaptureDemo" });
            mDataLst.Add(new Data { title = "WebcamCaptureDemo", name = "WebcamCaptureDemo" });
            mDataLst.Add(new Data { title = "PostProc", name = "PostProc" });
            mDataLst.Add(new Data { title = "PostProcBloomtest", name = "PostProcBloomtest" });
            mDataLst.Add(new Data { title = "PostProcBlurGaosi", name = "PostProcBlurGaosi" });
            mDataLst.Add(new Data { title = "PostProcBlurJingxiang", name = "PostProcBlurJingxiang" });
            mDataLst.Add(new Data { title = "PostProcBlurJingxiang2", name = "PostProcBlurJingxiang2" });
            mDataLst.Add(new Data { title = "PostProcBlurJunzhi", name = "PostProcBlurJunzhi" });
            mDataLst.Add(new Data { title = "PostProcBlurMotion", name = "PostProcBlurMotion" });
            mDataLst.Add(new Data { title = "PostProcCircle", name = "PostProcCircle" });
            mDataLst.Add(new Data { title = "PostProcDepth", name = "PostProcDepth" });
            mDataLst.Add(new Data { title = "PostProcNormal", name = "PostProcNormal" });
            mDataLst.Add(new Data { title = "PostProcDepthOfField", name = "PostProcDepthOfField" });
            mDataLst.Add(new Data { title = "PostProcDepthBuffer", name = "PostProcDepthBuffer" });
            mDataLst.Add(new Data { title = "PostProcDistort01", name = "PostProcDistort01" });
            mDataLst.Add(new Data { title = "PostProcDistort02", name = "PostProcDistort02" });
            mDataLst.Add(new Data { title = "RotationDistortEffect", name = "RotationDistortEffect" });
            mDataLst.Add(new Data { title = "PostProcEdgeEffect", name = "PostProcEdgeEffect" });
            mDataLst.Add(new Data { title = "PostProcEdgeEffectDepthNormal", name = "PostProcEdgeEffectDepthNormal" });
            mDataLst.Add(new Data { title = "PostProcessing", name = "PostProcessing" });
            mDataLst.Add(new Data { title = "PostProcScreenDepthScan", name = "PostProcScreenDepthScan" });
            mDataLst.Add(new Data { title = "PostProcWaterWaveEffect", name = "PostProcWaterWaveEffect" });
            mDataLst.Add(new Data { title = "扭曲效果", name = "DistortEffect" });
            mDataLst.Add(new Data { title = "PostRenderFire", name = "PostRenderFire" });
            mDataLst.Add(new Data { title = "PostRenderingCheckerboard", name = "PostRenderingCheckerboard" });
            mDataLst.Add(new Data { title = "PostRenderNoise", name = "PostRenderNoise" });
            mDataLst.Add(new Data { title = "生成噪点图", name = "ProceduralNoise" });
            mDataLst.Add(new Data { title = "Projector 投影贴图到不规则平面上", name = "Projector" });
            mDataLst.Add(new Data { title = "反射探针-ReflectionProbe", name = "ReflectionProbe" });
            mDataLst.Add(new Data { title = "渲染顺序-RenderOrderOpaque", name = "RenderOrderOpaque" });
            mDataLst.Add(new Data { title = "渲染顺序-RenderOrderTransparent", name = "RenderOrderTransparent" });
            mDataLst.Add(new Data { title = "颜色码转换-RGBToHSV", name = "RGBToHSV" });
            mDataLst.Add(new Data { title = "涟漪效果", name = "RippleEffect" });
            mDataLst.Add(new Data { title = "SamplerState", name = "SamplerState" });
            mDataLst.Add(new Data { title = "屏幕坐标", name = "ScreenPos" });
            mDataLst.Add(new Data { title = "ShaderGUI", name = "ShaderGUI" });
            mDataLst.Add(new Data { title = "几何着色器-Geometry", name = "Geometry" });
            mDataLst.Add(new Data { title = "符号距离函数-SDF", name = "SignedDistanceField01" });
            mDataLst.Add(new Data { title = "Simple", name = "Simple" });
            mDataLst.Add(new Data { title = "SSR-Simple", name = "SSR" });
            mDataLst.Add(new Data { title = "模板测试-StencilBuffer", name = "StencilBuffer" });
            mDataLst.Add(new Data { title = "模板测试-StencilByMask", name = "StencilByMask" });
            mDataLst.Add(new Data { title = "曲面细分效果-Tessellation", name = "Tessellation" });
            mDataLst.Add(new Data { title = "testAlphaBlendZWrite", name = "testAlphaBlendZWrite" });
            mDataLst.Add(new Data { title = "混合测试", name = "testBlend" });
            mDataLst.Add(new Data { title = "溶解效果", name = "testDissolve" });
            mDataLst.Add(new Data { title = "噪声图流动效果-火焰", name = "testDistort" });
            mDataLst.Add(new Data { title = "扫光效果", name = "testFlash" });
            mDataLst.Add(new Data { title = "扫光效果-有法线", name = "testFlashLight" });
            mDataLst.Add(new Data { title = "翻页效果", name = "testFlipBook" });
            mDataLst.Add(new Data { title = "灰化", name = "testGrayTex" });
            mDataLst.Add(new Data { title = "IfFor语句", name = "testIfFor" });
            mDataLst.Add(new Data { title = "法线图", name = "testNormal" });
            mDataLst.Add(new Data { title = "阴影测试", name = "testShadow" });
            mDataLst.Add(new Data { title = "高管贴图", name = "testSpecularTex" });
            mDataLst.Add(new Data { title = "testTransform", name = "testTransform" });
            mDataLst.Add(new Data { title = "贴图边缘过滤-TextureFilter", name = "TextureFilter" });
            mDataLst.Add(new Data { title = "Transparent 渲染顺序测试", name = "Transparent" });
            mDataLst.Add(new Data { title = "描边-模板测试", name = "testOutline" });
            mDataLst.Add(new Data { title = "描边-法线", name = "testOutlineXRay" });
            mDataLst.Add(new Data { title = "顶点动画", name = "VertexAnim" });
            mDataLst.Add(new Data { title = "VL-ExtrudeVertex", name = "VL" });
            mDataLst.Add(new Data { title = "VL-Postprocessing", name = "VL" });
            mDataLst.Add(new Data { title = "x - VL-RayMarching", name = "VL" });
            mDataLst.Add(new Data { title = "无反射的水", name = "WaterSimple_reflect_off" });
            mDataLst.Add(new Data { title = "有反射的水", name = "WaterSimple_reflect_on" });
            mDataLst.Add(new Data { title = "卡通水", name = "WaterToon" });
            mDataLst.Add(new Data { title = "x - z buff 测试", name = "z_test" });
            mDataLst.Add(new Data { title = "Best HTTP (Pro)", name = "SampleSelector" });
        }
    }
}