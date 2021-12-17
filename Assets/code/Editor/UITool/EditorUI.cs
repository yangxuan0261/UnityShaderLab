using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class CUIToggle {
    public string label = "";
    public bool check = false;
    public object data = null; // 透传参数
}

public class EditorUI {
    public delegate bool DrawDetailsDelegate();
    public delegate void ShowSectionDelegate(bool isShow);
    public delegate void IntDelegate(int value);
    public delegate void PlatIdDelegate(int platId);

    public static void DrawSeparator(int height = 1) {
        for (int i = 0; i < height; i++) {
            EditorGUILayout.Separator();
        }
    }

    public static void DrawHorizontalLine() {
        EditorGUILayout.Separator();
        EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);
        EditorGUILayout.Separator();
    }

    public static void NewLine(int height = 0) {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.Height(height));
        EditorGUILayout.EndHorizontal();
    }

    public static bool IsEditorDarkSkin() {
        return EditorGUIUtility.isProSkin;
    }

    public static Color GetUISafeTextColorYellow() {
        return IsEditorDarkSkin() ? Color.yellow : Color.blue;
    }

    public static Color GetUISafeTextColorRed() {
        return IsEditorDarkSkin() ? Color.magenta : Color.red;
    }

    public static Color GetUISafeTextColorGreen() {
        return IsEditorDarkSkin() ? Color.green : new Color(0.1f, 0.4f, 0.1f);
    }

    public static GUIStyle GetGUIStyle(string srcStyle, int padding, int margin) {
        GUIStyle style = new GUIStyle(GUI.skin.GetStyle(srcStyle));
        style.margin = new RectOffset(margin, margin, margin, margin);
        style.padding = new RectOffset(padding, padding, padding, padding);
        return style;
    }

    public static float GetPixelsPerPoint() {
        return EditorGUIUtility.pixelsPerPoint;
    }

    public static float GetHandleSize(Vector3 worldPos) {
        return HandleUtility.GetHandleSize(worldPos) * 0.2f;
    }

    public static Vector3 GetMousePosition(ref Event currentEvent, Camera camera) {
        Vector3 mousePosition = currentEvent.mousePosition;
        mousePosition *= GetPixelsPerPoint();

        // Unity mouse coordinate y is inverted
        mousePosition.y = camera.pixelHeight - mousePosition.y;

        return mousePosition;
    }

    public static Vector3 GetHandleWorldToScreenPosition(Vector3 worldPosition, Camera camera) {
        return camera.WorldToScreenPoint(Handles.matrix.MultiplyPoint(worldPosition));
    }

    /// <summary>
    /// Draw the specified property field (via its propertyName).
    /// </summary>
    public static void DrawPropertyField(SerializedObject assetObject, string propertyName, string labelName, string toolTip = "") {
        SerializedProperty property = assetObject.FindProperty(propertyName);

        GUIContent content = new GUIContent(labelName, toolTip);

        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(property, GUIContent.none, true, GUILayout.Width(50f));
        GUILayout.Label(content);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    public static bool DrawToggleLeft(bool toggleValue, string labelName, string toolTip = "") {
        GUIContent content = new GUIContent("  " + labelName, toolTip);
        bool newValue = EditorGUILayout.ToggleLeft(content, toggleValue);
        return newValue;
    }

    public static bool DrawFoldOut(bool foldoutState, string labelName, bool bBold = true) {
        GUIStyle foldStyle = new GUIStyle(GUI.skin.GetStyle("Foldout"));
        foldStyle.richText = true;
        foldStyle.fontSize = 12;
        foldStyle.fontStyle = bBold ? FontStyle.Bold : FontStyle.Normal;
        return EditorGUILayout.Foldout(foldoutState, labelName, true, foldStyle);
    }

    public static bool DrawFoldOutSerializedProperty(SerializedProperty property, string labelName, ref bool bChanged) {
        bool bValue = property != null ? property.boolValue : true;
        bool bNewValue = DrawFoldOut(bValue, labelName);
        if (property != null && bValue != bNewValue) {
            bChanged = true;
            property.boolValue = bNewValue;
        }
        return bNewValue;
    }

    private static GUIStyle _windowStyle;

    public static GUIStyle GetWindowStyle() {
        if (_windowStyle == null) {
            _windowStyle = new GUIStyle();
            _windowStyle.name = "heu_ui_window";

            GUIStyleState styleState = new GUIStyleState();
            styleState.textColor = new Color(0.705f, 0.705f, 0.705f, 1.0f);
            _windowStyle.active = styleState;

            _windowStyle.alignment = TextAnchor.UpperLeft;
            _windowStyle.border = new RectOffset(2, 30, 2, 30);
            _windowStyle.clipping = TextClipping.Clip;
            _windowStyle.contentOffset = new Vector2(0f, 0f);
            _windowStyle.imagePosition = ImagePosition.ImageLeft;

            string textureName = string.Format("heu_ui_window{0}", IsEditorDarkSkin() ? "_d" : "");
            Texture2D normalTexture = Resources.Load<Texture2D>(textureName);
            normalTexture.filterMode = FilterMode.Point;

            GUIStyleState normalState = new GUIStyleState();
            normalState.background = normalTexture;
            normalState.textColor = new Color(0.705f, 0.705f, 0.705f, 1.0f);
            _windowStyle.normal = normalState;

            GUIStyleState onNormalState = new GUIStyleState();
            onNormalState.background = normalTexture;
            normalState.textColor = new Color(0.705f, 0.705f, 0.705f, 1.0f);
            _windowStyle.onNormal = onNormalState;

            _windowStyle.overflow = new RectOffset(1, 1, 1, 1);
            _windowStyle.padding = new RectOffset(10, 10, 10, 10);

            _windowStyle.richText = true;
            _windowStyle.stretchWidth = true;
            _windowStyle.stretchHeight = true;
            _windowStyle.wordWrap = false;
        }
        return _windowStyle;
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

            string textureName = string.Format("heu_ui_section_box{0}", true ? "_d" : "");
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

    public static void BeginSection() {
        EditorGUILayout.BeginVertical(GetSectionStyle());
        EditorGUILayout.Space();
        EditorGUI.indentLevel++;
    }

    public static void EndSection() {
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
    }

    public static void DrawHeadingLabel(string labelText) {
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontStyle = FontStyle.Bold;
        //labelStyle.normal.textColor = HEU_EditorUI.IsEditorDarkSkin() ? Color.white : Color.black;
        EditorGUILayout.LabelField(labelText, labelStyle);
    }

    public static void DrawWarningLabel(string labelText) {
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.normal.textColor = IsEditorDarkSkin() ? Color.yellow : Color.red;
        EditorGUILayout.LabelField(labelText, labelStyle);
    }

    public static void DrawSphereCap(int controlID, Vector3 position, Quaternion rotation, float size) {
#if UNITY_2017_1_OR_NEWER
        Handles.SphereHandleCap(controlID, position, rotation, size, EventType.Repaint);
#else
        Handles.SphereCap(controlID, position, rotation, size);
#endif
    }

    public static void DrawCircleCap(int controlID, Vector3 position, Quaternion rotation, float size) {
#if UNITY_2017_1_OR_NEWER
        Handles.CircleHandleCap(controlID, position, rotation, size, EventType.Repaint);
#else
        Handles.CircleCap(controlID, position, rotation, size);
#endif
    }

    public static bool DrawSphereCapButton(Vector3 position, Quaternion rotation, float handleSize, float pickSize) {
#if UNITY_2017_1_OR_NEWER
        return Handles.Button(position, rotation, handleSize, pickSize, Handles.SphereHandleCap);
#else
        return Handles.Button(position, rotation, handleSize, pickSize, Handles.SphereCap);
#endif
    }

    public static void DrawCubeCap(int controlID, Vector3 position, Quaternion rotation, float size) {
#if UNITY_2017_1_OR_NEWER
        Handles.CubeHandleCap(controlID, position, rotation, size, EventType.Repaint);
#else
        Handles.CubeCap(controlID, position, rotation, size);
#endif
    }

    public static void DrawArrowCap(int controlID, Vector3 position, Quaternion rotation, float size) {
#if UNITY_2017_1_OR_NEWER
        Handles.ArrowHandleCap(controlID, position, rotation, size, EventType.Repaint);
#else
        Handles.ArrowCap(controlID, position, rotation, size);
#endif
    }

    public static void DrawLine(Vector3 start, Vector3 end) {
        Handles.DrawLine(start, end);
    }

    public static void DrawFilePathWithDialog(string labelName, SerializedProperty filePathProperty) {
        EditorGUILayout.BeginHorizontal();

        GUIContent labelContent = new GUIContent(labelName);
        EditorGUILayout.DelayedTextField(filePathProperty, labelContent, GUILayout.ExpandWidth(true));

        GUIStyle buttonStyle = GetNewButtonStyle_MarginPadding(0, 0);
        if (GUILayout.Button("...", buttonStyle, GUILayout.Width(30), GUILayout.Height(18))) {
            string filePattern = "*.*";
            string newPath = EditorUtility.OpenFilePanel("Select " + labelName, filePathProperty.stringValue, filePattern);
            if (newPath != null && !string.IsNullOrEmpty(newPath)) {
                filePathProperty.stringValue = newPath; // TODO: 为知之
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    public static GUIStyle GetNewButtonStyle(FontStyle fontStyle, int fontSize, TextAnchor textAlignment, float fixedHeight, int paddingLeft, int paddingRight, int paddingTop, int paddingBottom,
        int marginTop, int marginBottom, int marginLeft, int marginRight) {
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontStyle = fontStyle;
        buttonStyle.fontSize = fontSize > 0 ? fontSize : buttonStyle.fontSize;
        buttonStyle.alignment = textAlignment;
        buttonStyle.fixedHeight = fixedHeight > 0 ? fixedHeight : buttonStyle.fixedHeight;

        buttonStyle.padding.left = paddingLeft;
        buttonStyle.padding.right = paddingRight;
        buttonStyle.padding.top = paddingTop;
        buttonStyle.padding.bottom = paddingBottom;

        buttonStyle.margin.top = marginTop;
        buttonStyle.margin.bottom = marginBottom;
        buttonStyle.margin.left = marginLeft;
        buttonStyle.margin.right = marginRight;

        return buttonStyle;
    }

    public static GUIStyle GetNewButtonStyle_HEUDefaults() {
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.fontSize = 11;
        buttonStyle.alignment = TextAnchor.MiddleCenter;
        buttonStyle.fixedHeight = 22;
        buttonStyle.padding.left = 0;
        buttonStyle.padding.right = 0;
        buttonStyle.margin.top = 15;
        buttonStyle.margin.bottom = 6;
        buttonStyle.margin.left = 12;
        buttonStyle.margin.right = 12;
        return buttonStyle;
    }

    public static GUIStyle GetNewButtonStyle_MarginPadding(int margin, int padding) {
        return GetNewButtonStyle(FontStyle.Normal, -1, TextAnchor.MiddleCenter, -1, 0, 0, 0, 0, 0, 0, 0, 0);
    }

    public delegate bool DrawField<T>(ref T value);

    public static bool DrawFieldInt(ref int val) {
        int newValue = EditorGUILayout.DelayedIntField(val);
        if (newValue != val) {
            val = newValue;
            return true;
        }
        return false;
    }

    public static bool DrawFieldFloat(ref float val) {
        float newValue = EditorGUILayout.DelayedFloatField(val);
        if (newValue != val) {
            val = newValue;
            return true;
        }
        return false;
    }

    public static bool DrawFieldString(ref string val) {
        string newValue = EditorGUILayout.DelayedTextField(val);
        if (newValue != val) {
            val = newValue;
            return true;
        }
        return false;
    }

    public static bool DrawArray<T>(string labelString, ref T[] values, DrawField<T> drawFunc) {
        // Arrays are drawn with a label, and rows of values.

        bool bChanged = false;

        GUILayout.BeginHorizontal(); {
            EditorGUILayout.PrefixLabel(labelString);

            GUILayout.BeginVertical(EditorStyles.helpBox); {
                int numElements = values.Length;
                int maxElementsPerRow = 4;

                GUILayout.BeginHorizontal(); {
                    for (int i = 0; i < numElements; ++i) {
                        if (i > 0 && i % maxElementsPerRow == 0) {
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                        }

                        if (drawFunc(ref values[i])) {
                            bChanged = true;
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();

        return bChanged;
    }

    // -------------------- Section
    public static bool DrawSection(string sectionLabel, DrawDetailsDelegate drawDetailsDlg, ref bool foldoutState, ShowSectionDelegate showDlg = null) {
        bool bChanged = false;

        EditorUI.BeginSection();
        EditorGUI.BeginChangeCheck();
        foldoutState = EditorUI.DrawFoldOut(foldoutState, sectionLabel);
        if (EditorGUI.EndChangeCheck() && showDlg != null) {
            showDlg(foldoutState);
        }

        if (foldoutState) {
            EditorUI.DrawSeparator();
            EditorGUI.indentLevel++;
            using(var hs = new EditorGUILayout.HorizontalScope()) {
                using(var vs = new EditorGUILayout.VerticalScope()) {
                    if (drawDetailsDlg != null) {
                        bChanged |= drawDetailsDlg();
                    }
                }
            }
            EditorGUI.indentLevel--;
        }
        EditorUI.EndSection();
        EditorUI.DrawSeparator();
        return bChanged;
    }

    // -------------------- its
    public static Enum EnumPopupExt(string label, Enum selected, params GUILayoutOption[] options) {
        using(new EditorGUILayout.HorizontalScope()) {
            selected = EditorGUILayout.EnumPopup(label, selected, options);
            EditorGUILayout.LabelField(string.Format("{0}", Convert.ToInt32(selected)), GUILayout.Width(70));
            return selected;
        }
    }


    public static string TextAreaExt(string label, string txt, GUIStyle style, params GUILayoutOption[] options) {
        using(new EditorGUILayout.HorizontalScope()) {
            EditorGUILayout.LabelField(label, GUILayout.Width(116));
            return EditorGUILayout.TextArea(txt, style, options);
        }
    }

    public static float SliderExt(string label, float value, float leftValue, float rightValue, params GUILayoutOption[] options) {
        using(new EditorGUILayout.HorizontalScope()) {
            EditorGUILayout.LabelField(label, GUILayout.Width(116));
            return EditorGUILayout.Slider(value, leftValue, rightValue);
        }
    }

    public static void IntFieldExt(string label, ref int value, IntDelegate dlg, params GUILayoutOption[] options) {

    }

    public static int IntSliderExt(string label, int value, int leftValue, int rightValue, params GUILayoutOption[] options) {
        using(new EditorGUILayout.HorizontalScope()) {
            EditorGUILayout.LabelField(label, GUILayout.Width(116));
            return EditorGUILayout.IntSlider(value, leftValue, rightValue);
        }
    }

    public static void SelectFile(string label, ref string selPath, string title, string extName) {
        using(new EditorGUILayout.HorizontalScope()) {
            string newPath = EditorGUILayout.TextField(new GUIContent(label), selPath, GUILayout.Width(300));
            GUIStyle buttonStyle = EditorUI.GetNewButtonStyle_MarginPadding(0, 0);
            if (GUILayout.Button("...", buttonStyle, GUILayout.Width(30), GUILayout.Height(18))) {
                string selFile = UnityEditor.EditorUtility.OpenFilePanel(title, newPath, extName);
                selPath = selFile;
            }
        }
    }

    public static void SelectFolder(string label, ref string selPath, string title, string defaultName) {
        using(new EditorGUILayout.HorizontalScope()) {
            string newPath = EditorGUILayout.TextField(new GUIContent(label), selPath, GUILayout.Width(300));
            GUIStyle buttonStyle = EditorUI.GetNewButtonStyle_MarginPadding(0, 0);
            if (GUILayout.Button("...", buttonStyle, GUILayout.Width(30), GUILayout.Height(18))) {
                string selFile = UnityEditor.EditorUtility.OpenFolderPanel(title, newPath, defaultName);
                selPath = selFile;
            }
        }
    }

    public static void DrawBtn(string btnName, string msg, Action fn, int space = 10, int width = 130) {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(space);
        if (GUILayout.Button(btnName, GUILayout.Width(width))) {
            fn();
        }
        EditorGUILayout.LabelField(msg);
        EditorGUILayout.EndHorizontal();
    }

    public static void DrawIndent(Action fn, int ident = 1) {
        EditorGUI.indentLevel += ident;
        fn();
        EditorGUI.indentLevel -= ident;
    }

    public static void DrawToggleList(List<CUIToggle> toggleArr, int ident = 0) {
        EditorGUI.indentLevel += ident;
        foreach (CUIToggle item in toggleArr) {
            item.check = EditorUI.DrawToggleLeft(item.check, item.label);
        }
        EditorGUI.indentLevel -= ident;
    }

    public static void ProgressBarExt(float value, string label, int height = 20, int width = 20) {
        Rect rect = GUILayoutUtility.GetRect(height, width); 
        EditorGUI.ProgressBar(rect, value, label); 
    }

    private static Texture2D _fireIcon = null;

    public static void DrawLabel01(string label) {
        if (_fireIcon == null) {
            _fireIcon = Resources.Load("heu_recookhdaIcon") as Texture2D;
        }
        GUIContent gct = new GUIContent(" " + label, _fireIcon);
        EditorGUILayout.LabelField(gct);
    }
}