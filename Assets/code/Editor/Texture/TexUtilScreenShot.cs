using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TexUtilScreenShot : UtilBase<ETexUtil> {

    public TexUtilScreenShot() {
        mMod = ETexUtil.ScreenShot;
        mDescr = "TexUtilScreenShot";
    }

    // ---------------------- gui begin ----------------------
    private string mFSPath = "";

    public override void Draw() {
        DrawFullScreen();
        DrawPartScreen();
        DrawCam();

        EditorUI.DrawLabel01("导出选中对象的贴图");
        EditorUI.DrawBtn("导出", "", () => {
            SaveImage();
        }, 35, 100);
    }

    private void DrawFullScreen() {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("全屏", GUILayout.Width(100))) {
            DoFullScreen();
        }
        EditorGUILayout.EndHorizontal();
        GuiUtil.NewLine();
    }

    private void DrawPartScreen() {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("区域", GUILayout.Width(100))) {
            DoPartScreen();
            EditorCoroutineRunner.StartEditorCoroutine(DoPartScreen());
        }
        EditorGUILayout.EndHorizontal();
        GuiUtil.NewLine();
    }

    private void DrawCam() {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("指定相机", GUILayout.Width(100))) {
            DoCam();
        }
        EditorGUILayout.EndHorizontal();
        GuiUtil.NewLine();
    }
    // ---------------------- gui end ----------------------

    private void DoFullScreen() {
        string file = EditorUtils.GetDesktop("full.png");
        ScreenCapture.CaptureScreenshot(file, 4);
        Debug.LogFormat("--- DoFullScreen ok, path: {0}", file);
    }

    IEnumerator DoPartScreen() {
        Rect mRect = new Rect(0, 0, 700, 700);
        string file = EditorUtils.GetDesktop("part.png");

        yield return new WaitForEndOfFrame();
        Texture2D mTexture = new Texture2D((int) mRect.width, (int) mRect.height,
            TextureFormat.RGB24, false);
        //读取屏幕像素信息并存储为纹理数据
        mTexture.ReadPixels(mRect, 0, 0);
        mTexture.Apply();
        byte[] bytes = mTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(file, bytes);
        yield return null;
        Debug.LogFormat("--- DoPartScreen ok, path: {0}", file);

    }

    private void DoCam() {

    }

    private void SaveImage() {
        GameObject go = Selection.activeGameObject;
        EditorUtils.Assert(go != null, "--- 未选中场景对象");
        Debug.LogFormat("--- name: {0}", go.name);

        RawImage ri = go.GetComponent<RawImage>();
        EditorUtils.Assert(ri != null, "--- go: {0} 获取不到 RawImage 组件", go.name);
        Texture tex = ri.texture;
        EditorUtils.Assert(tex != null, "--- 获取不到 Texture 组件");

        string file = EditorUtils.GetDesktop("noise.png");
        Utils.SaveTexToPng(tex, file, tex.width, tex.height);
        Debug.LogFormat("--- save ok, path: {0}", file);
    }

}