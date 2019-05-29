using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class OtherScriptableObject : UtilBase<EOtherUtil> {

    public OtherScriptableObject() {
        mMod = EOtherUtil.ScriptableObject;
        mDescr = "ScriptableObject";
    }

    public override void Draw() {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("CreateScriptableObject", GUILayout.Width(200))) {
            CreateScriptableObject();
        }
        EditorGUILayout.LabelField("序列化资源");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("LoadScriptableObject", GUILayout.Width(200))) {
            LoadScriptableObject();
        }
        EditorGUILayout.LabelField("反序列化资源");
        EditorGUILayout.EndHorizontal();
    }

    public void CreateScriptableObject() {
        MyScriptableObject ins = ScriptableObject.CreateInstance<MyScriptableObject>();
        ins.myname = "hello";
        ins.age = 111;
        ins.raduis = 4.2f;
        string path = "Assets/Z_Other/ScriptableObject/MySO2.asset";

        string texpath1 = "Assets/Common/tex/build_sh_jb.png";
        string texpath2 = "Assets/Common/tex/Plasma_1.png";
        Texture tex1 = AssetDatabase.LoadAssetAtPath<Texture>(texpath1);
        Texture tex2 = AssetDatabase.LoadAssetAtPath<Texture>(texpath2);

        ins.texList.Add(tex1);
        ins.texList.Add(tex2);

        AssetDatabase.CreateAsset(ins, path);
        Debug.LogFormat("--- CreateAsset success, path:{0}", path);
    }

    public void LoadScriptableObject() {
        string path = "Assets/Z_Other/ScriptableObject/MySO2.asset";
        MyScriptableObject ins = AssetDatabase.LoadAssetAtPath<MyScriptableObject>(path);
        Debug.LogFormat("--- LoadAsset success, {0}, {1}, {2}, {3}", ins.myname, ins.age, ins.raduis, ins.texList.Count);
    }
}