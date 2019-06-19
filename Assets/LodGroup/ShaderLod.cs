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
            Debug.LogFormat("--- maximumLOD = 500");
            mat.shader.maximumLOD = 500;
        }

        if (Input.GetKeyDown(KeyCode.B)) {
            Debug.LogFormat("--- maximumLOD = 600");
            mat.shader.maximumLOD = 600;
        }

        // globalMaximumLOD 貌似没什么乱用
        if (Input.GetKeyDown(KeyCode.C)) {
            Debug.LogFormat("--- globalMaximumLOD = 500");
            Shader.globalMaximumLOD = 500;
        }

        if (Input.GetKeyDown(KeyCode.D)) {
            Debug.LogFormat("--- globalMaximumLOD = 600");
            Shader.globalMaximumLOD = 600;
        }
    }
}