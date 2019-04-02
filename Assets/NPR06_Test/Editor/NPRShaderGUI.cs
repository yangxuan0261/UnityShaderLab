using System;
using UnityEditor;
using UnityEngine;

public class NPRShaderGUI : ShaderGUI {
	public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] properties) {
		base.OnGUI (materialEditor, properties); // 显示默认面板

		Material targetMat = materialEditor.target as Material;

		// cubemap
		MaterialProperty cubeMap = ShaderGUI.FindProperty ("_Cubemap", properties);
		bool blendEnabled = cubeMap.textureValue != null;
		if (blendEnabled) {
			targetMat.EnableKeyword ("_CUBEMAP_ON");
		} else {
			targetMat.DisableKeyword ("_CUBEMAP_ON");
		}

		// receive shadow
		bool isRecvShadow = Array.IndexOf (targetMat.shaderKeywords, "_SHADOW_ON") != -1;
		EditorGUI.BeginChangeCheck ();
		isRecvShadow = EditorGUILayout.Toggle ("RecevieShadow", isRecvShadow);

		if (EditorGUI.EndChangeCheck ()) {
			if (isRecvShadow)
				targetMat.EnableKeyword ("_SHADOW_ON");
			else
				targetMat.DisableKeyword ("_SHADOW_ON");
		}

	}
}