using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class testMatCap2 : MonoBehaviour {

    Material mMat;

    void Start() {
        mMat = GetComponent<Renderer>().sharedMaterial;
    }


    void LateUpdate() {
        if (mMat != null) {
            // Debug.LogFormat("--- aaa");
            Matrix4x4 _o2w = transform.localToWorldMatrix;
            mMat.SetMatrix("_o2w", _o2w);
        }
    }
}