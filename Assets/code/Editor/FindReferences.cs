using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CubeSplitter : EditorWindow {
    Cubemap splitCube;
    Color[] CubeMapColors;
    int splitSize;

    [MenuItem("Tools/CubeSplitter", false, 500)]
    public static void OpenCubeSplitter() {
        var win = GetWindow<CubeSplitter>("CubeSplitter");
        win.Show();
    }

    void OnGUI () {

        GUILayout.Label ("Choose the Cube Map you want to save as 6 images and click EXPORT!", EditorStyles.boldLabel);
        splitCube = EditorGUILayout.ObjectField ("My Cubemap:", splitCube, typeof (Cubemap), false) as Cubemap;
        GUILayout.Label ("Make sure to set the Size to the same as the Cubemap you are using", EditorStyles.boldLabel);
        splitSize = EditorGUILayout.IntField ("CubeMap Size: ", splitSize);

        if (GUILayout.Button ("EXPORT!")) {
            if (splitCube) {
                Export ();
            }

            if (!splitCube) {
                Debug.Log ("Forget Something?");
            }
        }
    }

    void Export () {
        var filePath = AssetDatabase.GetAssetPath (splitCube);

        Texture2D tex = new Texture2D (splitSize, splitSize, TextureFormat.RGB24, false);

        CubeMapColors = splitCube.GetPixels (CubemapFace.PositiveY);
        tex.SetPixels (CubeMapColors, 0);
        tex.Apply ();
        byte[] bytes = tex.EncodeToPNG ();
        File.WriteAllBytes (filePath + "_top.png", bytes);

        CubeMapColors = splitCube.GetPixels (CubemapFace.NegativeY);
        tex.SetPixels (CubeMapColors, 0);
        tex.Apply ();
        bytes = tex.EncodeToPNG ();
        File.WriteAllBytes (filePath + "_bottom.png", bytes);

        CubeMapColors = splitCube.GetPixels (CubemapFace.PositiveX);
        tex.SetPixels (CubeMapColors, 0);
        tex.Apply ();
        bytes = tex.EncodeToPNG ();
        File.WriteAllBytes (filePath + "_right.png", bytes);

        CubeMapColors = splitCube.GetPixels (CubemapFace.NegativeX);
        tex.SetPixels (CubeMapColors, 0);
        tex.Apply ();
        bytes = tex.EncodeToPNG ();
        File.WriteAllBytes (filePath + "_left.png", bytes);

        CubeMapColors = splitCube.GetPixels (CubemapFace.PositiveZ);
        tex.SetPixels (CubeMapColors, 0);
        tex.Apply ();
        bytes = tex.EncodeToPNG ();
        File.WriteAllBytes (filePath + "_front.png", bytes);

        CubeMapColors = splitCube.GetPixels (CubemapFace.NegativeZ);
        tex.SetPixels (CubeMapColors, 0);
        tex.Apply ();
        bytes = tex.EncodeToPNG ();
        File.WriteAllBytes (filePath + "_back.png", bytes);

        this.Close ();
    }
}