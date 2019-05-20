using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class GuiUtil {

    public static void NewLine() {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("");
        EditorGUILayout.EndHorizontal();
    }

}