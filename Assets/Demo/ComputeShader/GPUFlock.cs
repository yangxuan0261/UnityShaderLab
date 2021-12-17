using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 这个结构体字段要与 cs 中的完全一致, cpu 与 gpu 数据交互的载体
public struct GPUBoid {
    public Vector3 pos;
    public Vector3 rot;
    public Vector3 flockPos;
    public float speed;
    public float nearbyDis;
    public float boidsCount;
}

public class GPUFlock : MonoBehaviour {

    public ComputeShader cshader;

    public GameObject boidPrefab;
    public int boidsCount;
    public float spawnRadius;
    public float flockSpeed;
    public float nearbyDis;

    private Vector3 targetPos = Vector3.zero;
    private int kernelHandle;

    GameObject[] boidsGo;
    GPUBoid[] boidsData;

    void Start() {
        this.boidsGo = new GameObject[this.boidsCount];
        this.boidsData = new GPUBoid[this.boidsCount];
        this.kernelHandle = cshader.FindKernel("MyCSMain"); // 找到句柄

        for (int i = 0; i < this.boidsCount; i++) {
            this.boidsData[i] = this.CreateBoidData();
            this.boidsGo[i] = Instantiate(boidPrefab, this.boidsData[i].pos, Quaternion.Euler(this.boidsData[i].rot)) as GameObject;
            this.boidsData[i].rot = this.boidsGo[i].transform.forward;
        }
    }

    GPUBoid CreateBoidData() {
        GPUBoid boidData = new GPUBoid();
        Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
        Quaternion rot = Quaternion.Slerp(transform.rotation, Random.rotation, 0.3f);
        boidData.pos = pos;
        boidData.flockPos = transform.position;
        boidData.boidsCount = this.boidsCount;
        boidData.nearbyDis = this.nearbyDis;
        boidData.speed = this.flockSpeed + Random.Range(-0.5f, 0.5f);

        return boidData;
    }

    void Update() {

        this.targetPos += new Vector3(2f, 5f, 3f);
        this.transform.localPosition += new Vector3(
            (Mathf.Sin(Mathf.Deg2Rad * this.targetPos.x) * -0.2f),
            (Mathf.Sin(Mathf.Deg2Rad * this.targetPos.y) * 0.2f),
            (Mathf.Sin(Mathf.Deg2Rad * this.targetPos.z) * 0.2f)
        );

        ComputeBuffer buffer = new ComputeBuffer(boidsCount, 48);

        for (int i = 0; i < this.boidsData.Length; i++) {
            this.boidsData[i].flockPos = this.transform.position;
        }

        buffer.SetData(this.boidsData); // 设置需要计算的数据数组 this.boidsData
        cshader.SetBuffer(this.kernelHandle, "boidBuffer", buffer); // 上传一个 buffer
        cshader.SetFloat("deltaTime", Time.deltaTime); // 上传一个基础的uniform变量
        cshader.Dispatch(this.kernelHandle, this.boidsCount, 1, 1); // 执行 cs, gpu 并行计算
        buffer.GetData(this.boidsData); // 将数据从 GPU 传回到 CPU 中

        buffer.Release();

        for (int i = 0; i < this.boidsData.Length; i++) {
            this.boidsGo[i].transform.localPosition = this.boidsData[i].pos;
            if (!this.boidsData[i].rot.Equals(Vector3.zero)) {
                this.boidsGo[i].transform.rotation = Quaternion.LookRotation(this.boidsData[i].rot);
            }
        }
    }

}