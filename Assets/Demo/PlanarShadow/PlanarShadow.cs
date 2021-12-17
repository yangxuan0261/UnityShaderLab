using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlanarShadow : MonoBehaviour {

	public Renderer target;
	public Vector3 projdir = Vector3.forward; // 如果超过一个角色有阴影, 应该统一一个方向参考点

	private Material mat;
	// Use this for initialization
	private void Awake() {
#if UNITY_EDITOR
		mat = target.sharedMaterial;
#else
		mat = target.material;
#endif
	}

	void Update() {
		if (mat != null) {
			mat.SetVector("_WorldPos", transform.position);
			mat.SetVector("_ShadowProjDir", projdir);
			// mat.SetVector("_ShadowPlane", new Vector4(0.0f, 1.0f, 0.0f));
			// mat.SetVector("_ShadowFadeParams", new Vector3(0.0f, 1.5f, 0.7f));
		}
	}
}