using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPos : MonoBehaviour {

#if UNITY_EDITOR // 编辑器模式、且非游戏状态 才绘制
    void OnDrawGizmos() {
        if (!Application.isPlaying) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.2f);
        }
    }
#endif
}
