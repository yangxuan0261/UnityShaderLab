using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

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
        string path = "E:\\temp_save\\aabb.png";
        ScreenCapture.CaptureScreenshot(path, 4);
        Debug.LogFormat("--- DoFullScreen ok!");
    }

    IEnumerator DoPartScreen() {
        Rect mRect = new Rect(0, 0, 700, 700);
        string mFileName = "E:\\temp_save\\aacc.png";

        yield return new WaitForEndOfFrame();
        Texture2D mTexture = new Texture2D((int) mRect.width, (int) mRect.height,
            TextureFormat.RGB24, false);
        //读取屏幕像素信息并存储为纹理数据
        mTexture.ReadPixels(mRect, 0, 0);
        mTexture.Apply();
        byte[] bytes = mTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(mFileName, bytes);
        yield return null;
        Debug.LogFormat("--- DoPartScreen ok!");

    }

    private void DoCam() {

    }
}