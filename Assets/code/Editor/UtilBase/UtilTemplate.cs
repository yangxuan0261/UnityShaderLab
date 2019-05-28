using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UtilTemplate<E, B> : EditorWindow where E : Enum where B : UtilBase<E> {

	protected E mNewMod;
	protected E mOldMod;
	protected Dictionary<E, B> mUtilMap = new Dictionary<E, B>();
	private B currIns = null;

	public UtilTemplate() { }

	B GetTexUtil(E mod) {
		if (!mUtilMap.ContainsKey(mNewMod)) {
			Debug.LogErrorFormat("--- 木有注册工具, 模式:{0}", mod);
			return default(B);
		}
		return mUtilMap[mod];
	}

	Vector2 scroll;
	protected virtual void OnGUI() {
		scroll = EditorGUILayout.BeginScrollView(scroll);
		mNewMod = (E) EditorGUILayout.EnumPopup("模式", mNewMod);
		B ins = GetTexUtil(mNewMod);
		if (ins == null) {
			Debug.LogFormat("Error: 为获取到实例");
			return;
		}

		if (!mNewMod.Equals(mOldMod)) {
			mOldMod = mNewMod;
			Debug.LogFormat("--- change");
			if (currIns != null) {
				currIns.OnUtilExit();
			}
			ins.OnUtilEnter();
			currIns = ins;
		}

		EditorGUILayout.LabelField(ins.mDescr);
		GuiUtil.NewLine();
		ins.Draw();
		EditorGUILayout.EndScrollView();
	}

}