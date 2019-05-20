using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;

public class TexUtilMerge : TexUtilBase {

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

}