using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TexUtilMerge : UtilBase<ETexUtil> {

    public TexUtilMerge() {
        mMod = ETexUtil.Merge;
        mDescr = "TexUtilMerge";
    }

    public override void Draw() {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10);
        if (GUILayout.Button("TexUtilMerge", GUILayout.Width(100))) {

        }
        EditorGUILayout.LabelField("TexUtilMergeccccccccccc");
        EditorGUILayout.EndHorizontal();
    }

    // public override void OnUtilEnter(){
    //     Debug.LogFormat("--- OnUtilEnter");
    // }

    // public override void OnUtilExit(){
    //     Debug.LogFormat("--- OnUtilExit");
    // }
}