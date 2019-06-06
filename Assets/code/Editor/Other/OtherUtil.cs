using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EOtherUtil {
	ScriptableObject = 0,
	Wrap,
}

public class OtherUtil : UtilTemplate<EOtherUtil, UtilBase<EOtherUtil>> {

	// ---------------------- GUI begin ----------------------
	[MenuItem("Tools/OtherUtil", false, 3000)]
	public static void OpenChunkGUI() {
		var win = GetWindow<OtherUtil>("OtherUtil");
		win.Show();
	}

	public OtherUtil() {
		mNewMod = EOtherUtil.ScriptableObject;
		mUtilMap.Add(EOtherUtil.ScriptableObject, new OtherScriptableObject());
		mUtilMap.Add(EOtherUtil.Wrap, new OtherTestWrap());
	}

	// protected override void OnGUI() {

	// }
}