using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class OtherTestWrap : UtilBase<EOtherUtil> {

    public OtherTestWrap() {
        mMod = EOtherUtil.Wrap;
        mDescr = "OtherTestWrap";
    }

    public override void Draw() {
        GUIStyle backgroundStyle = new GUIStyle(GUI.skin.GetStyle("box"));
        RectOffset br = backgroundStyle.margin;
        br.top = 10;
        br.bottom = 6;
        br.left = 4;
        br.right = 4;
        backgroundStyle.margin = br;

        br = backgroundStyle.padding;
        br.top = 8;
        br.bottom = 8;
        br.left = 8;
        br.right = 8;
        backgroundStyle.padding = br;
        using(var hs = new EditorGUILayout.VerticalScope(backgroundStyle)) {
            EditorGUILayout.Separator();

            DrawHeaderSection();

            EditorGUI.BeginChangeCheck();
            if (EditorGUI.EndChangeCheck()) {

            }
        }

    }

    void DrawHeaderSection() {
        GUI.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
        Texture2D headerImage = Resources.Load("heu_hengine") as Texture2D;

        GuiUtil.BeginSection();
        GUILayout.Label(headerImage, GUILayout.MinWidth(100));
        GuiUtil.EndSection();

        GUI.backgroundColor = Color.white;

        GuiUtil.DrawSeparator();
    }

}