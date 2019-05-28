using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;

public class TexUtilSplit : UtilBase<ETexUtil> {

    public TexUtilSplit() {
        mMod = ETexUtil.Split;
        mDescr = "TexUtilSplit";
    }

    public override void Draw() {
        EditorGUILayout.BeginHorizontal();
		GUILayout.Space(10);
		if (GUILayout.Button("TexUtilSplit", GUILayout.Width(100))) {

		}
		EditorGUILayout.LabelField("TexUtilSplitaaaaaaaaaaa");
		EditorGUILayout.EndHorizontal();

        /*
        string path = GetTexPath("需要分离的贴图");
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("行:");
		mXCnt = EditorGUILayout.IntSlider(mXCnt, 1, 100);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("列:");
		mYCnt = EditorGUILayout.IntSlider(mYCnt, 1, 100);
		EditorGUILayout.EndHorizontal();

		RegSceneBtn("分离", "", () => {
			if (mXCnt == 1 || mYCnt == 1 || path == "") {
				return;
			}

		});
         */
    }

}