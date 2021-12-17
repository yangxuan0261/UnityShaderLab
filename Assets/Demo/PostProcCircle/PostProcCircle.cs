using UnityEngine;

public class PostProcCircle : PostEffectBase {

    [Range(0, 1f)]
    public float radius = 0.2f;
    [Range(0, 0.2f)]
    public float width = 0.03f;
    public Vector2 center = new Vector2(0.5f, 0.5f);
    public Color mainColor = Color.red;

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (_Material) {
            _Material.SetFloat("_Radius", radius);
            _Material.SetFloat("_Width", width);
            _Material.SetVector("_Center", center);
            _Material.SetColor("_MainColor", mainColor);
            Graphics.Blit(source, destination, _Material);
        } else {
            Graphics.Blit(source, destination);
        }
    }
}