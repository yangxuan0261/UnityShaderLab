using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SortAllBuilding : MonoBehaviour {
    public int row = 7;
    public int xWidth = 3;
    public int zWidth = 3;
    public Vector3 pos = Vector3.zero;
    public bool refresh = false;

    private bool _isDirty = true;

    void Start () {
        _isDirty = true;   
    }

    void OnValidate() {
        _isDirty = true;   
    }

    void Sort() {
        int xCnt = 1;
        int zCnt = 0;
        int total = transform.childCount;
        Debug.LogFormat("--- 总数量:{0}", total);
        for (int i = 0; i < total; i++) {
            Transform tf = transform.GetChild(i);
            tf.position = new Vector3(xWidth * xCnt, 0f, zWidth * zCnt);

            if (xCnt == row) {
                zCnt += 1;
                xCnt = 1;
            } else {
                xCnt += 1;
            }

        }
    }

    void Update() {
        if (_isDirty) {
            _isDirty = false;
            Sort();
        }
    }
}
