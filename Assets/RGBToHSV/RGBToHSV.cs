using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RGBToHSV : MonoBehaviour {

    public Material mMat;
    private Color mHsv = new Color(0, 1, 1, 1);

    void Update() {
        if (mMat != null) {

            float val = Mathf.Abs(Mathf.Sin(UnityEngine.Time.time));
            // Color oldClr = mMat.GetColor("_MainColor");
            Color newClr = Color.HSVToRGB(val, mHsv.g, mHsv.b, true);
            mMat.SetColor("_MainColor", newClr);
        }
    }
}