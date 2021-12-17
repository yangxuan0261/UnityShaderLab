using UnityEngine;
using UnityEditor;
using System;

public class CharacterStandard_PBR_ShaderGUI : ShaderGUI
{
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        Material targetMat = materialEditor.target as Material;

        bool isFancy = Array.IndexOf(targetMat.shaderKeywords, "_FANCY_STUFF") != -1;
        EditorGUI.BeginChangeCheck();
        isFancy = EditorGUILayout.Toggle("FancyStuff", isFancy); 
        if (EditorGUI.EndChangeCheck())
        {
            if (isFancy)
                targetMat.EnableKeyword("_FANCY_STUFF");
            else
                targetMat.DisableKeyword("_FANCY_STUFF");
        }

		bool isUseRoughness = Array.IndexOf(targetMat.shaderKeywords, "_USE_ROUGHNESS") != -1;
        EditorGUI.BeginChangeCheck();
        isUseRoughness = EditorGUILayout.Toggle("UseRoughness", isUseRoughness); 
        if (EditorGUI.EndChangeCheck())
        {
            if (isUseRoughness)
                targetMat.EnableKeyword("_USE_ROUGHNESS");
            else
                targetMat.DisableKeyword("_USE_ROUGHNESS");
        }

		// bool isAlphaTest = Array.IndexOf(targetMat.shaderKeywords, "_ALPHA_TEST_ON") != -1;
        // EditorGUI.BeginChangeCheck();
        // isAlphaTest = EditorGUILayout.Toggle("AlphaTest", isAlphaTest); 
        // if (EditorGUI.EndChangeCheck())
        // {
        //     if (isAlphaTest)
        //         targetMat.EnableKeyword("_ALPHA_TEST_ON");
        //     else
        //         targetMat.DisableKeyword("_ALPHA_TEST_ON");
        // }

		MaterialProperty cutOff = ShaderGUI.FindProperty("_CutOff", properties);
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("CutOff");
		cutOff.floatValue = EditorGUILayout.Slider(cutOff.floatValue, 0, 1.1f);
		targetMat.SetFloat("_CutOff", cutOff.floatValue);
		EditorGUILayout.EndHorizontal();
		bool isAlphaTest = cutOff.floatValue > 0.001 ? true : false;
		if (isAlphaTest)
			targetMat.EnableKeyword("_ALPHA_TEST_ON");
		else
			targetMat.DisableKeyword("_ALPHA_TEST_ON");


        base.OnGUI(materialEditor, properties); // 显示默认面板


		bool isEnabled = false;

		MaterialProperty normalMap = ShaderGUI.FindProperty("_NormalTex", properties);
		isEnabled = normalMap.textureValue != null;
		if (isEnabled) {
      		targetMat.EnableKeyword("_NORMAL_MAP");
		}
		else {
			targetMat.DisableKeyword("_NORMAL_MAP");
		}
		// Debug.LogFormat("--- _NormalTex:{0}", isEnabled);

		MaterialProperty relfectionMap = ShaderGUI.FindProperty("_RefectionTex", properties);
		isEnabled = relfectionMap.textureValue != null;
		if (isEnabled) {
      		targetMat.EnableKeyword("_REFLECTION_ON");
		}
		else {
			targetMat.DisableKeyword("_REFLECTION_ON");
		}
		// Debug.LogFormat("--- _RefectionTex:{0}", isEnabled);

		MaterialProperty dissolveMap = ShaderGUI.FindProperty("_DissolveTex", properties);
		isEnabled = dissolveMap.textureValue != null;
		if (isEnabled) {
      		targetMat.EnableKeyword("_DISSOLVE_ON");
		}
		else {
			targetMat.DisableKeyword("_DISSOLVE_ON");
		}
		// Debug.LogFormat("--- _DissolveTex:{0}", isEnabled);
		
		MaterialProperty flowLightMap = ShaderGUI.FindProperty("_FlowLightTex", properties);
		isEnabled = flowLightMap.textureValue != null;
		if (isEnabled) {
      		targetMat.EnableKeyword("_FLOW_LIGHT_ON");
		}
		else {
			targetMat.DisableKeyword("_FLOW_LIGHT_ON");
		}
		// Debug.LogFormat("--- _FlowLightTex:{0}", isEnabled);
		



    }
}