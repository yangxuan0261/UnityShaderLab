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

        EditorUI.DrawBtn("添加所有场景", "", () => {
            HashSet<string> nameSet = new HashSet<string>();

            string txt = "";
            string[] resFiles = AssetDatabase.FindAssets("t:Scene", new string[] { "Assets" });
            EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[resFiles.Length]; // 加入 Build In Scene
            for (int i = 0; i < resFiles.Length; ++i) {
                string path = AssetDatabase.GUIDToAssetPath(resFiles[i]);
                Debug.LogFormat("--- path: {0}", path);
                scenes[i] = new EditorBuildSettingsScene(path, true);

                // 写入名字
                string fileName = Path.GetFileNameWithoutExtension(path);
                EditorUtils.Assert(!nameSet.Contains(fileName), "--- 包含重复场景名: {0}", fileName);
                nameSet.Add(fileName);
                txt += fileName + "\n";
            }
            EditorBuildSettings.scenes = scenes;
            Utils.WriteFileUTF8(EditorUtils.GetDesktop("all_scene.txt"), txt);

            Debug.LogFormat("--- EditorBuildSettings ok");
        }, 35, 100);
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