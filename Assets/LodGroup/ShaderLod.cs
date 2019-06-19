using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderLod : MonoBehaviour {

    Material mat;

    void Start() {
        // Shader.globalMaximumLOD = 600;
        mat = transform.GetComponent<Renderer>().sharedMaterial;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            mat.shader.maximumLOD = 500;
            Debug.LogFormat("--- maximumLOD = 500");
        }

        if (Input.GetKeyDown(KeyCode.B)) {
            mat.shader.maximumLOD = 600;
            Debug.LogFormat("--- maximumLOD = 600");
        }
    }
}