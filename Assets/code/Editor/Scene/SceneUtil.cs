using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ESceneUtil {
	AllScene = 0,
}

public class SceneUtil : UtilTemplate<ESceneUtil, UtilBase<ESceneUtil>> {

	// ---------------------- GUI begin ----------------------
	[MenuItem("Tools/SceneUtil", false, 2000)]
	public static void OpenChunkGUI() {
		var win = GetWindow<SceneUtil>("SceneUtil");
		win.Show();
	}

	public SceneUtil() {
		mNewMod = ESceneUtil.AllScene;
		mUtilMap.Add(ESceneUtil.AllScene, new SceUtilListAll());
	}
}