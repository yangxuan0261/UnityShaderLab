using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class GuiUtil {

    public static bool IsEditorDarkSkin() {
        return EditorGUIUtility.isProSkin;
    }

    // 换行
    public static void NewLine() {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("");
        EditorGUILayout.EndHorizontal();
    }

    public static void DrawSeparator() {
        EditorGUILayout.Separator();
    }

    private static GUIStyle _sectionStyle;

    public static GUIStyle GetSectionStyle() {
        if (_sectionStyle == null) {
            _sectionStyle = new GUIStyle();
            _sectionStyle.name = "heu_ui_section";

            GUIStyleState styleState = new GUIStyleState();
            styleState.textColor = new Color(0.705f, 0.705f, 0.705f, 1.0f);
            _sectionStyle.active = styleState;

            _sectionStyle.alignment = TextAnchor.UpperCenter;
            _sectionStyle.border = new RectOffset(9, 9, 4, 14);
            _sectionStyle.clipping = TextClipping.Clip;
            _sectionStyle.contentOffset = new Vector2(0f, 3f);
            _sectionStyle.imagePosition = ImagePosition.ImageLeft;

            string textureName = string.Format("heu_ui_section_box{0}", GuiUtil.IsEditorDarkSkin() ? "_d" : "");
            Texture2D normalTexture = Resources.Load<Texture2D>(textureName);

            GUIStyleState normalState = new GUIStyleState();
            normalState.background = normalTexture;
            _sectionStyle.normal = normalState;

            _sectionStyle.overflow = new RectOffset(4, 4, 0, 9);
            _sectionStyle.padding = new RectOffset(4, 4, 4, 4);

            _sectionStyle.richText = false;
            _sectionStyle.stretchWidth = false;
        }
        return _sectionStyle;
    }

    /// <summary>
    /// Start a UI section.
    /// </summary>
    public static void BeginSection() {
        EditorGUILayout.BeginVertical(GetSectionStyle());
        EditorGUILayout.Space();
        EditorGUI.indentLevel++;
    }

    /// <summary>
    /// End a UI section.
    /// </summary>
    public static void EndSection() {
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
    }

}