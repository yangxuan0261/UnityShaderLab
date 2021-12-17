using UnityEngine;


[ExecuteInEditMode]
public class PostRenderNoise : MonoBehaviour
{
    public Material _material;
 
    void Awake()
    {
        // _material = new Material(Shader.Find("Custom/PostRenderNoise"));
        // _material.SetTexture("_SecondaryTex", Resources.Load("Textures/Hatch") as Texture);
    }
 
	void Start() {
		if (_material == null) {
			Debug.LogError("--- _material is null");
		}
	}

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        _material.SetFloat("_OffsetX", Random.Range(0f, 1.1f));
        _material.SetFloat("_OffsetY", Random.Range(0, 1.1f));
        Graphics.Blit(source, destination, _material);
    }
}