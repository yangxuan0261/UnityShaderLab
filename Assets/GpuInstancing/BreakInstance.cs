using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakInstance : MonoBehaviour {

    public MeshRenderer mMeshRenderer;

    void Update() {
        if (Input.GetKeyDown(KeyCode.V) && mMeshRenderer != null) {
            int idClr = Shader.PropertyToID("_Color");
            mMeshRenderer.material.SetColor(idClr, new Color(1, 0, 0, 1)); // .material 获取材质球时, 是获取到复制拷贝的实例, 所以不会 gpu instance.
        }
    }
}