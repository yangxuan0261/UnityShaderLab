using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Matrix : MonoBehaviour {

    public Material mMat;

    void Update() {
        if (mMat != null) {
            // 把 局部坐标系 三个方向转到 世界空间
            Vector3 right = transform.TransformDirection(transform.right);
            Vector3 up = transform.TransformDirection(transform.up);
            Vector3 forward = transform.TransformDirection(transform.forward);

            mMat.SetVector("_right", right);
            mMat.SetVector("_up", up);
            mMat.SetVector("_forward", forward);
        }
    }
}