using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;

public class TextureUtil: EditorWindow {

	enum EMode {
		Merge = 0,
		Split,
	}

	// ---------------------- GUI begin ----------------------
    [MenuItem("Tools/TextureUtil", false, 500)]
    public static void OpenChunkGUI() {
        var win = GetWindow<TextureUtil>("TextureUtil");
        win.Show();
    }

	private EMode mMod = EMode.Split;
	private Texture mTex;

	// split
	private int mXCnt = 1;
	private int mYCnt = 1;

	// merge

	void RegSceneBtn(string btnName, string msg, Action fn, int tab = 1) {
		EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10 * tab);
        if (GUILayout.Button(btnName, GUILayout.Width(100))) {
			fn();
        }
		EditorGUILayout.LabelField(msg);
        EditorGUILayout.EndHorizontal();
	}

	Vector2 scroll;
	void OnGUI() {
		scroll = EditorGUILayout.BeginScrollView(scroll);
		mMod = (EMode)EditorGUILayout.EnumPopup("模式", mMod);
		switch (mMod) {
			case EMode.Merge: DrawMerge(); break;
			case EMode.Split: DrawSplit(); break;
		}
		EditorGUILayout.EndScrollView();
	}

	string GetTexPath(string title) {
		EditorGUILayout.BeginHorizontal();
		mTex = (Texture)EditorGUILayout.ObjectField("其中一张贴图", mTex, typeof(Texture), allowSceneObjects: false);
        EditorGUILayout.EndHorizontal();
		return mTex != null ? AssetDatabase.GetAssetPath(mTex) : "";
	}

	void DrawMerge() {
		string path = GetTexPath("需要合并的其中一张贴图");
		RegSceneBtn("合并", "", ()=>{
			if (path == "") {
				return;
			}

			string dirPath = Path.GetDirectoryName(path);
			Debug.LogFormat("--- 目录路径:{0}", dirPath);
		});
	}

	void DrawSplit() {
		string path = GetTexPath("需要分离的贴图");
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("行:");
		mXCnt = EditorGUILayout.IntSlider(mXCnt, 1, 100);
        EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("列:");
		mYCnt = EditorGUILayout.IntSlider(mYCnt, 1, 100);
        EditorGUILayout.EndHorizontal();

		RegSceneBtn("分离", "", ()=>{
			if (mXCnt == 1 || mYCnt == 1 || path == "") {
				return;
			}


		});
	}
}
