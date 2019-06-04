using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HelperGUI : EditorWindow {

	// ---------------------- GUI begin ----------------------
	[MenuItem ("Tools/HelperGUI", false, 500)]
	public static void OpenChunkGUI () {
		var win = GetWindow<HelperGUI> ("HelperGUI");
		win.Show ();
	}

	[SerializeField] //必须要加
	protected List<UnityEngine.Object> _assetList = new List<UnityEngine.Object> ();
	protected SerializedObject _serializedObject;
	protected SerializedProperty _assetLstProperty;

	protected void OnEnable () {
		//使用当前类初始化
		_serializedObject = new SerializedObject (this);
		//获取当前类中可序列话的属性
		_assetLstProperty = _serializedObject.FindProperty ("_assetList");
	}

	void RegSceneBtn (string btnName, string msg, Action fn, int tab = 1)
	 {
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Space (10 * tab);
		if (GUILayout.Button (btnName, GUILayout.Width (100))) {
			fn ();
		}
		EditorGUILayout.LabelField (msg);
		GUILayout.EndHorizontal ();
	}

	Vector2 scroll;
	void OnGUI () {
		scroll = EditorGUILayout.BeginScrollView (scroll);
		RegSceneBtn ("生成go", "生成go", () => {
			GenGo ();
		});

		DrawSelectObj ();

		EditorGUILayout.EndScrollView ();
	}

	void GenGo () {
		int xCnt = 50;
		int zCnt = 50;
		float xSpace = 0.3f;
		float zSpace = 0.45f;

		GameObject rootGo = GameObject.Find ("root");
		GameObject tempGo = GameObject.Find ("capsule");
		for (int i = 0; i < zCnt; i++) {
			for (int j = 0; j < xCnt; j++) {
				GameObject go = GameObject.Instantiate (tempGo);
				go.transform.parent = rootGo.transform;
				go.transform.localPosition = new Vector3 (j * xSpace, 0, i * zSpace);
				go.name = "" + i + j;
			}
		}
	}

	void DrawSelectObj () {
		_serializedObject.Update ();
		EditorGUI.BeginChangeCheck ();
		EditorGUILayout.PropertyField (_assetLstProperty, true);
		if (EditorGUI.EndChangeCheck ()) {
			_serializedObject.ApplyModifiedProperties ();
		}

	}
}