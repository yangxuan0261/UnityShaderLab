using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShowName : MonoBehaviour {

	void Start () {
		TextMesh tm = GetComponent<TextMesh>();
		tm.text = transform.parent.name;
	}

	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update()
	{
		Start();
	}
	
}
