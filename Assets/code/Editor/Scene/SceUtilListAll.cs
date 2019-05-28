using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;

public class SceUtilListAll : UtilBase<ESceneUtil> {

    public SceUtilListAll() {
        mMod = ESceneUtil.AllScene;
        mDescr = "SceUtilListAll";
    }

    public override void Draw() {
        EditorGUILayout.BeginHorizontal();
		GUILayout.Space(10);
		if (GUILayout.Button("SceUtilListAll", GUILayout.Width(100))) {

		}
		EditorGUILayout.LabelField("SceUtilListAllccccccccccc");
		EditorGUILayout.EndHorizontal();
    }

}