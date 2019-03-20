using UnityEngine;
using UnityEditor;
using System;

public class TestShaderGUI : ShaderGUI
{
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        base.OnGUI(materialEditor, properties); // 显示默认面板

        Material targetMat = materialEditor.target as Material;

		// test1, shader 中增加一个 Toggle, 来决定编辑器是否显示 _Color2 属性
		MaterialProperty _UseTwoColors = ShaderGUI.FindProperty("_UseTwoColors", properties);
		if (_UseTwoColors.floatValue == 1)
		{
			MaterialProperty _Color2 = ShaderGUI.FindProperty("_Color2", properties);
			materialEditor.ShaderProperty(_Color2, _Color2.displayName);
		}

		// test2, 检测贴图是否有被用到, 来决定是否启用 _BLENDMAP 宏
		MaterialProperty blendMap = ShaderGUI.FindProperty("_BlendTex", properties);
		bool blendEnabled = blendMap.textureValue != null;
		if (blendEnabled) {
      		targetMat.EnableKeyword("_BLENDMAP");
		}
		else {
			targetMat.DisableKeyword("_BLENDMAP");
		}
		
		// test3, 编辑器扩展一个 Toggle, 来决定是否启用 CS_BOOL 宏
        bool CS_BOOL = Array.IndexOf(targetMat.shaderKeywords, "CS_BOOL") != -1;
        EditorGUI.BeginChangeCheck();
        CS_BOOL = EditorGUILayout.Toggle("CS_BOOL", CS_BOOL);

        if (EditorGUI.EndChangeCheck())
        {
            if (CS_BOOL)
                targetMat.EnableKeyword("CS_BOOL");
            else
                targetMat.DisableKeyword("CS_BOOL");
        }
    }
}