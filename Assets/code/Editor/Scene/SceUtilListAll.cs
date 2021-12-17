using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SceUtilListAll : UtilBase<ESceneUtil> {

    public SceUtilListAll() {
        mMod = ESceneUtil.AllScene;
        mDescr = "SceUtilListAll";
    }

    public override void Draw() {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10);
        if (GUILayout.Button("SceUtilListAll", GUILayout.Width(100))) {

            string txt = "";
            string[] resFiles = AssetDatabase.FindAssets("t:Scene", new string[] { "Assets" });
            for (int i = 0; i < resFiles.Length; i++) {
                resFiles[i] = AssetDatabase.GUIDToAssetPath(resFiles[i]);
                Debug.LogFormat(resFiles[i]);
                txt += resFiles[i] + "\n";
            }
            Utils.WriteFileUTF8(EditorUtils.GetDesktop("adasdasd.txt"), txt);
        }
        EditorGUILayout.LabelField("SceUtilListAllccccccccccc");
        EditorGUILayout.EndHorizontal();
    }

    // private static void ChangeShader(bool useSRP) {
    //     var guids = AssetDatabase.FindAssets("t:scene");
    //     foreach (var guid in guids) {
    //         string path = AssetDatabase.GUIDToAssetPath(guid);
    //         Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
    //         if (mat == null || mat.shader == null || mat.shader.name == null) { continue; }
    //         string newShader = mat.shader.name;
    //         if (useSRP) {
    //             newShader = newShader.Replace("App/NonSRP/", "App/SRP/");
    //         } else {
    //             newShader = newShader.Replace("App/SRP/", "App/NonSRP/");
    //         }
    //         mat.shader = Shader.Find(newShader);
    //         EditorUtility.SetDirty(mat);
    //     }
    //     AssetDatabase.SaveAssets();
    // }

}