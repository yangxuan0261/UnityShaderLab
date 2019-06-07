using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class Translating : MonoBehaviour {

    public float speed = 10.0f;
    public bool pingpong = true;
    public Transform startTF;
    public Transform endTf;

    private Vector3 curEndPoint = Vector3.zero;

    private Vector3 startPoint = Vector3.zero;
    private Vector3 endPoint = Vector3.zero;

    void Start() {
        startPoint = startTF.position;
        endPoint = endTf.position;

        transform.position = startPoint;
        curEndPoint = endPoint;
    }

    void Update() {
        transform.position = Vector3.Slerp(transform.position, curEndPoint, Time.deltaTime * speed);
        if (pingpong) {
            if (Vector3.Distance(transform.position, curEndPoint) < 0.001f) {
                curEndPoint = Vector3.Distance(curEndPoint, endPoint) < Vector3.Distance(curEndPoint, startPoint) ? startPoint : endPoint;
            }
        }
    }
}