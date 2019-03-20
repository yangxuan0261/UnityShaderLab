using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;

public class HelperGUI: EditorWindow {

	// ---------------------- GUI begin ----------------------
    [MenuItem("Tools/HelperGUI", false, 500)]
    public static void OpenChunkGUI() {
        var win = GetWindow<HelperGUI>("HelperGUI");
        win.Show();
    }

	void RegSceneBtn(string btnName, string msg, Action fn, int tab = 1) {
		EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10 * tab);
        if (GUILayout.Button(btnName, GUILayout.Width(100))) {
			fn();
        }
		EditorGUILayout.LabelField(msg);
        GUILayout.EndHorizontal();
	}

	Vector2 scroll;
	void OnGUI() {
		scroll = EditorGUILayout.BeginScrollView(scroll);
		RegSceneBtn("生成go", "生成go", () =>{
			GenGo();
		});

		EditorGUILayout.EndScrollView();
	}

	void GenGo() {
		int xCnt = 50;
		int zCnt = 50;
		float xSpace = 0.3f;
		float zSpace = 0.45f;

		GameObject rootGo = GameObject.Find("root");
		GameObject tempGo = GameObject.Find("capsule");
		for (int i = 0; i < zCnt; i++) {
			for (int j = 0; j < xCnt; j++) {
				GameObject go = GameObject.Instantiate(tempGo);
				go.transform.parent = rootGo.transform;
				go.transform.localPosition = new Vector3(j * xSpace, 0, i * zSpace);
				go.name = "" + i + j;
			}
		}

	}
}
