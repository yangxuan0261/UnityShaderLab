using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class testMatCap2 : MonoBehaviour {

    Material mMat;

    void Start() {
        mMat = GetComponent<Renderer>().sharedMaterial;
    }

    void Update() {
        if (mMat != null) {
            // Debug.LogFormat("--- aaa");
            Matrix4x4 _o2wIT = transform.localToWorldMatrix;
            mMat.SetMatrix("_o2wIT", _o2wIT);
        }
    }
}