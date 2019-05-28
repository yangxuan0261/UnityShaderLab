using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;

public class OtherOne : UtilBase<EOtherUtil> {

    public OtherOne() {
        mMod = EOtherUtil.One;
        mDescr = "OtherOne";
    }

    public override void Draw() {
        EditorGUILayout.BeginHorizontal();
		GUILayout.Space(10);
		if (GUILayout.Button("OtherOne", GUILayout.Width(100))) {

		}
		EditorGUILayout.LabelField("OtherOneccccccccccc");
		EditorGUILayout.EndHorizontal();
    }

}