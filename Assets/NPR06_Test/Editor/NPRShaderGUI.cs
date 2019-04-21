using System;
using UnityEditor;
using UnityEngine;

public class NPRShaderGUI : ShaderGUI {
	public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] properties) {
		base.OnGUI (materialEditor, properties); // 显示默认面板

		Material targetMat = materialEditor.target as Material;

		bool isEnable = false;

		// cubemap
		MaterialProperty cubeMap = ShaderGUI.FindProperty ("_Cubemap", properties);
		isEnable = cubeMap.textureValue != null;
		if (isEnable) {
			targetMat.EnableKeyword ("_CUBEMAP_ON");
		} else {
			targetMat.DisableKeyword ("_CUBEMAP_ON");
		}

		// emission
		MaterialProperty emissionMap = ShaderGUI.FindProperty ("_EmissionTex", properties);
		isEnable = emissionMap.textureValue != null;
		if (isEnable) {
			targetMat.EnableKeyword ("_EMISSION_ON");
		} else {
			targetMat.DisableKeyword ("_EMISSION_ON");
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

		// receive shadow
		bool isOtherLightOff = Array.IndexOf (targetMat.shaderKeywords, "_OHTER_LIGHT_OFF") != -1;
		EditorGUI.BeginChangeCheck ();
		isOtherLightOff = EditorGUILayout.Toggle ("OtherLightOff", isOtherLightOff);

		if (EditorGUI.EndChangeCheck ()) {
			if (isOtherLightOff)
				targetMat.EnableKeyword ("_OHTER_LIGHT_OFF");
			else
				targetMat.DisableKeyword ("_OHTER_LIGHT_OFF");
		}

	}
}