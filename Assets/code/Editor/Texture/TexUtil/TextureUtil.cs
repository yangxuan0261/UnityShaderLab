using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// TODO: 待整合: ScreenShot.cs

public enum ETexUtil {
	Merge,
	Split,
	ScreenShot,
}

public class TextureUtil : EditorWindow {

	// ---------------------- GUI begin ----------------------
	[MenuItem("Tools/TextureUtil", false, 500)]
	public static void OpenChunkGUI() {
		var win = GetWindow<TextureUtil>("TextureUtil");
		win.Show();
	}

	private ETexUtil mMod = ETexUtil.ScreenShot;
	private Dictionary<ETexUtil, TexUtilBase> mTexUtil = new Dictionary<ETexUtil, TexUtilBase>();

	public TextureUtil() {
		RegTexUtil();
	}

	void RegTexUtil() {
		mTexUtil.Add(ETexUtil.Merge, new TexUtilMerge());
		mTexUtil.Add(ETexUtil.Split, new TexUtilSplit());
		mTexUtil.Add(ETexUtil.ScreenShot, new TexUtilScreenShot());
	}

	TexUtilBase GetTexUtil(ETexUtil mod) {
		if (!mTexUtil.ContainsKey(mMod)) {
			Debug.LogErrorFormat("--- 木有注册工具, ETexUtil:{0}", mod);
			return null;
		}
		return mTexUtil[mod];
	}

	Vector2 scroll;
	void OnGUI() {
		scroll = EditorGUILayout.BeginScrollView(scroll);
		mMod = (ETexUtil) EditorGUILayout.EnumPopup("模式", mMod);
		TexUtilBase tub = GetTexUtil(mMod);
		EditorGUILayout.LabelField(tub.mDescr);
	
		GuiUtil.NewLine();

		tub.Draw();
		EditorGUILayout.EndScrollView();
	}

	/*
		string GetTexPath(string title) {
			EditorGUILayout.BeginHorizontal();
			mTex = (Texture) EditorGUILayout.ObjectField("其中一张贴图", mTex, typeof(Texture), allowSceneObjects : false);
			EditorGUILayout.EndHorizontal();
			return mTex != null ? AssetDatabase.GetAssetPath(mTex) : "";
		}
	 */
}