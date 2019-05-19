using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenUrlComp : MonoBehaviour {

    public UnityEngine.UI.Text mText;

    public void open() {
        if (mText != null && !string.IsNullOrEmpty(mText.text)) {
            Debug.LogFormat("--- url:{0}", mText.text);
            Application.OpenURL(mText.text);
        }
    }
}