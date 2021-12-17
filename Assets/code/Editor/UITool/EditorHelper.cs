using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EditorHelper {
    public static void SelectObject(GameObject gameObject) {
        Selection.objects = new GameObject[] { gameObject };
    }

    public static void SelectObjects(GameObject[] gameObjects) {
        Selection.objects = gameObjects;
    }

    public static SerializedProperty GetSerializedProperty(SerializedObject serializedObject, string propertyName) {
        SerializedProperty foundProperty = serializedObject.FindProperty(propertyName);
        if (foundProperty == null) {
            string errorMsg = string.Format("Property {0} not found on Object {1}. Make sure class {1} has member variable {0}.", propertyName, serializedObject.targetObject.GetType().ToString());
            Debug.LogAssertion(errorMsg);
        }
        return foundProperty;
    }

    public static bool EditorDrawSerializedProperty(SerializedObject serializedObject, string propertyName, string label = null, string tooltip = null, bool bIncludeChildren = true) {
        SerializedProperty serializedProperty = GetSerializedProperty(serializedObject, propertyName);
        if (serializedProperty != null) {
            if (label == null) {
                EditorGUILayout.PropertyField(serializedProperty, bIncludeChildren);
            } else if (label.Length == 0) {
                EditorGUILayout.PropertyField(serializedProperty, GUIContent.none, bIncludeChildren);
            } else {
                EditorGUILayout.PropertyField(serializedProperty, new GUIContent(label, tooltip), bIncludeChildren);
            }
            return true;
        }
        return false;
    }

    public static GameObject SaveAsPrefabAsset(GameObject instanceRoot, string assetPath, out bool success) {
        return PrefabUtility.SaveAsPrefabAsset(instanceRoot, assetPath, out success);
    }

    public static bool EditorDrawFloatProperty(SerializedObject serializedObject, string propertyName, string label = null, string tooltip = null) {
        SerializedProperty serializedProperty = GetSerializedProperty(serializedObject, propertyName);
        if (serializedProperty != null) {
            if (label == null) {
                EditorGUILayout.DelayedFloatField(serializedProperty);
            } else if (label.Length == 0) {
                EditorGUILayout.DelayedFloatField(serializedProperty, GUIContent.none);
            } else {
                EditorGUILayout.DelayedFloatField(serializedProperty, new GUIContent(label, tooltip));
            }
            return true;
        }
        return false;
    }

    public static bool EditorDrawFloatSliderProperty(SerializedProperty parentProperty, string propertyName, string label, string tooltip) {
        bool bChanged = false;
        SerializedProperty itemProperty = parentProperty.FindPropertyRelative(propertyName);
        if (itemProperty != null) {
            using(new EditorGUILayout.HorizontalScope()) {
                GUIContent labelContent = new GUIContent(label, tooltip);

                float value = itemProperty.floatValue;
                float newValue = EditorGUILayout.DelayedFloatField(labelContent, value, GUILayout.ExpandWidth(false));
                if (newValue == value) {
                    newValue = GUILayout.HorizontalSlider(itemProperty.floatValue, 0f, 1f);
                }
                if (newValue != value) {
                    itemProperty.floatValue = newValue;
                    bChanged = true;
                }
            }
        }
        return bChanged;
    }

    public static bool EditorDrawIntProperty(SerializedObject serializedObject, string propertyName, string label = null, string tooltip = null) {
        SerializedProperty serializedProperty = GetSerializedProperty(serializedObject, propertyName);
        if (serializedProperty != null) {
            if (label == null) {
                EditorGUILayout.DelayedIntField(serializedProperty);
            } else if (label.Length == 0) {
                EditorGUILayout.DelayedIntField(serializedProperty, GUIContent.none);
            } else {
                EditorGUILayout.DelayedIntField(serializedProperty, new GUIContent(label, tooltip));
            }
            return true;
        }
        return false;
    }

    public static bool EditorDrawTextProperty(SerializedObject serializedObject, string propertyName, string label = null) {
        SerializedProperty serializedProperty = GetSerializedProperty(serializedObject, propertyName);
        if (serializedProperty != null) {
            if (label == null) {
                EditorGUILayout.DelayedTextField(serializedProperty);
            } else if (label.Length == 0) {
                EditorGUILayout.DelayedTextField(serializedProperty, GUIContent.none);
            } else {
                EditorGUILayout.DelayedTextField(serializedProperty, new GUIContent(label));
            }
            return true;
        }
        return false;
    }

    public static bool EditorDrawBoolProperty(SerializedObject serializedObject, string propertyName, string label = null, string tooltip = null) {
        SerializedProperty serializedProperty = GetSerializedProperty(serializedObject, propertyName);
        if (serializedProperty != null) {
            if (label == null) {
                EditorGUILayout.PropertyField(serializedProperty);
            } else if (label.Length == 0) {
                EditorGUILayout.PropertyField(serializedProperty, GUIContent.none);
            } else {
                EditorGUILayout.PropertyField(serializedProperty, new GUIContent(label, tooltip));
            }
            return true;
        }
        return false;
    }

    public static void EditorDrawIntArray(ref int[] intValues, string label = null) {
        // Arrays are drawn with a label, and rows of values.

        GUILayout.BeginHorizontal(); {
            if (!string.IsNullOrEmpty(label)) {
                EditorGUILayout.PrefixLabel(label);
            }

            GUILayout.BeginVertical(EditorStyles.helpBox); {
                int numElements = intValues.Length;
                int maxElementsPerRow = 4;

                GUILayout.BeginHorizontal(); {
                    for (int i = 0; i < numElements; ++i) {
                        if (i > 0 && i % maxElementsPerRow == 0) {
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                        }

                        intValues[i] = EditorGUILayout.DelayedIntField(intValues[i]);
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
    }

    public static void EditorDrawFloatArray(ref float[] floatFields, string label = null) {
        // Arrays are drawn with a label, and rows of values.

        GUILayout.BeginHorizontal(); {
            if (!string.IsNullOrEmpty(label)) {
                EditorGUILayout.PrefixLabel(label);
            }

            GUILayout.BeginVertical(EditorStyles.helpBox); {
                int numElements = floatFields.Length;
                int maxElementsPerRow = 4;

                GUILayout.BeginHorizontal(); {
                    for (int i = 0; i < numElements; ++i) {
                        if (i > 0 && i % maxElementsPerRow == 0) {
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                        }

                        floatFields[i] = EditorGUILayout.DelayedFloatField(floatFields[i]);
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
    }

    public static void EditorDrawTextArray(ref string[] stringFields, string label = null) {
        // Arrays are drawn with a label, and rows of values.

        GUILayout.BeginHorizontal(); {
            if (!string.IsNullOrEmpty(label)) {
                EditorGUILayout.PrefixLabel(label);
            }

            GUILayout.BeginVertical(EditorStyles.helpBox); {
                int numElements = stringFields.Length;
                int maxElementsPerRow = 4;

                GUILayout.BeginHorizontal(); {
                    for (int i = 0; i < numElements; ++i) {
                        if (i > 0 && i % maxElementsPerRow == 0) {
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                        }

                        stringFields[i] = EditorGUILayout.DelayedTextField(stringFields[i]);
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
    }

    public static bool EditorDrawVector2RelativeProperty(SerializedProperty parentProperty, string parameterName, string label, string tooltip) {
        bool bChanged = false;
        SerializedProperty valueProperty = parentProperty.FindPropertyRelative(parameterName);
        if (valueProperty != null) {
            Vector2 vec2Value = valueProperty.vector2Value;
            using(new EditorGUILayout.HorizontalScope()) {
                GUIContent labelContent = new GUIContent(label, tooltip);
                EditorGUILayout.PrefixLabel(labelContent);

                // To align with rest of UI due to prefix
                GUILayout.Space(-30);

                vec2Value.x = EditorGUILayout.DelayedFloatField(vec2Value.x);
                GUILayout.Space(-20);
                vec2Value.y = EditorGUILayout.DelayedFloatField(vec2Value.y);
            }

            if (vec2Value != valueProperty.vector2Value) {
                valueProperty.vector2Value = vec2Value;
                bChanged = true;
            }
        }
        return bChanged;
    }

    public static void EditorDrawArrayProperty(SerializedProperty arrayProperty, EditorDrawPropertyDelegate drawDelegate, string label = null) {
        // Arrays are drawn with a label, and rows of values.

        GUILayout.BeginHorizontal(); {
            if (!string.IsNullOrEmpty(label)) {
                EditorGUILayout.PrefixLabel(label);
            }

            GUILayout.BeginVertical(EditorStyles.helpBox); {
                int numElements = arrayProperty.arraySize;
                int maxElementsPerRow = 4;

                GUILayout.BeginHorizontal(); {
                    for (int i = 0; i < numElements; ++i) {
                        if (i > 0 && i % maxElementsPerRow == 0) {
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                        }

                        SerializedProperty elementProperty = arrayProperty.GetArrayElementAtIndex(i);
                        drawDelegate(elementProperty);
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
    }

    public delegate void EditorDrawPropertyDelegate(SerializedProperty property);

    public static void EditorDrawIntProperty(SerializedProperty property) {
        property.intValue = EditorGUILayout.DelayedIntField(property.intValue);
    }

    public static void EditorDrawFloatProperty(SerializedProperty property) {
        property.floatValue = EditorGUILayout.DelayedFloatField(property.floatValue);
    }

    public static void EditorDrawTextProperty(SerializedProperty property) {
        property.stringValue = EditorGUILayout.DelayedTextField(property.stringValue);
    }

    public static int[] GetSerializedPropertyArrayValuesInt(SerializedProperty property) {
        int[] array = null;
        if (property.isArray) {
            array = new int[property.arraySize];
            for (int i = 0; i < array.Length; ++i) {
                array[i] = property.GetArrayElementAtIndex(i).intValue;
            }
        }
        return array;
    }

    public static float[] GetSerializedPropertyArrayValuesFloat(SerializedProperty property) {
        float[] array = null;
        if (property.isArray) {
            array = new float[property.arraySize];
            for (int i = 0; i < array.Length; ++i) {
                array[i] = property.GetArrayElementAtIndex(i).floatValue;
            }
        }
        return array;
    }

    public static string[] GetSerializedPropertyArrayValuesString(SerializedProperty property) {
        string[] array = null;
        if (property.isArray) {
            array = new string[property.arraySize];
            for (int i = 0; i < array.Length; ++i) {
                array[i] = property.GetArrayElementAtIndex(i).stringValue;
            }
        }
        return array;
    }

    // -------------------- its
    public static bool IsAssetExist(string path) {
        return AssetDatabase.GetMainAssetTypeAtPath(path) != null;
    }
}