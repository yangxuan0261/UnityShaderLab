using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RndRG : MonoBehaviour {

	Material _mat;	

	// Use this for initialization
	void Start () {
		_mat = GetComponent<Renderer>().sharedMaterial;
	}
	
	[ContextMenu("genClr")]
	void genClr() {
		if (_mat != null) {
			float r = Random.Range(0.5f, 1);
			float g = Random.Range(0.5f, 1);
			_mat.SetVector("_Color", new Vector4(r, g, 1, 1));
		} else {
			Debug.LogError("--- 没有找到材质球");
		}
	}
}
