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

public class TextureUtil : UtilTemplate<ETexUtil, UtilBase<ETexUtil>> {

	// ---------------------- GUI begin ----------------------
	[MenuItem("Tools/TextureUtil", false, 500)]
	public static void OpenChunkGUI() {
		var win = GetWindow<TextureUtil>("TextureUtil");
		win.Show();
	}

	public TextureUtil() {
		mNewMod = ETexUtil.ScreenShot;
		mUtilMap.Add(ETexUtil.Merge, new TexUtilMerge());
		mUtilMap.Add(ETexUtil.Split, new TexUtilSplit());
		mUtilMap.Add(ETexUtil.ScreenShot, new TexUtilScreenShot());
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