using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class GuiUtil {

    // 换行
    public static void NewLine() {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("");
        EditorGUILayout.EndHorizontal();
    }

}