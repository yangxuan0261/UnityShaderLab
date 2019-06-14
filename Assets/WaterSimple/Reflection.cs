using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Reflection : MonoBehaviour {

	private Transform Panel;
	[HideInInspector][SerializeField]
	private Camera RefCamera;
	private Material RefMat;
	private RenderTexture ReflTex;
	public float clipPlaneOffset=0;
	private string ReflectTextureName = "_ReflectTex";
	// Use this for initialization
	void Start () {
		if(null == RefCamera)
		{
			GameObject go = new GameObject();
			go.name = "reflCamera";
			RefCamera = go.AddComponent<Camera>();
			RefCamera.CopyFrom(Camera.main);
			//RefCamera.fieldOfView *= 1.1f;
			RefCamera.enabled = false;
			RefCamera.clearFlags = CameraClearFlags.SolidColor;
			RefCamera.backgroundColor = new Color(0,0,0,0);
			RefCamera.cullingMask =  ~(1 << LayerMask.NameToLayer("Water"));
		}
		if(null == RefMat)
		{
			RefMat = this.GetComponent<Renderer>().sharedMaterial;
		}
		if(null == ReflTex)
		ReflTex = new RenderTexture(Mathf.FloorToInt(Camera.main.pixelWidth*0.5f),
		                                             Mathf.FloorToInt(Camera.main.pixelHeight*0.5f), 24);
		ReflTex.hideFlags = HideFlags.DontSave;
		RefCamera.targetTexture = ReflTex;
		if(null == Panel)
		{
			Panel = transform;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void OnWillRenderObject()
	{
		RenderRefection();
	}

	void RenderRefection()
	{
		Vector3 normal = Panel.up;

		float d = -Vector3.Dot (normal, Panel.position);
		Matrix4x4 refMatrix = new Matrix4x4();
		refMatrix.m00 = 1-2*normal.x*normal.x;
		refMatrix.m01 = -2*normal.x*normal.y;
		refMatrix.m02 = -2*normal.x*normal.z;
		refMatrix.m03 = -2*d*normal.x;

		refMatrix.m10 = -2*normal.x*normal.y;
		refMatrix.m11 = 1-2*normal.y*normal.y;
		refMatrix.m12 = -2*normal.y*normal.z;
		refMatrix.m13 = -2*d*normal.y;

		refMatrix.m20 = -2*normal.x*normal.z;
		refMatrix.m21 = -2*normal.y*normal.z;
		refMatrix.m22 = 1-2*normal.z*normal.z;
		refMatrix.m23 = -2*d*normal.z;

		refMatrix.m30 = 0;
		refMatrix.m31 = 0;
		refMatrix.m32 = 0;
		refMatrix.m33 = 1;

		RefCamera.worldToCameraMatrix = Camera.main.worldToCameraMatrix * refMatrix;
		RefCamera.transform.position = refMatrix.MultiplyPoint(Camera.main.transform.position);

		Vector3 forward = Camera.main.transform.forward;
//		Vector3 up = Camera.main.transform.up;
		forward = refMatrix.MultiplyVector (forward);
//		up = refMatrix.MultiplyVector (up);
//		Quaternion refQ = Quaternion.LookRotation (forward, up);
//		RefCamera.transform.rotation = refQ;
		RefCamera.transform.forward = forward;

		//
		Matrix4x4 projM = RefCamera.projectionMatrix;
		Vector4 clipPlane = CameraSpacePlane (RefCamera, Panel.position, Panel.up, 1);
		RefCamera.projectionMatrix = CalculateObliqueMatrix(projM, clipPlane);

		GL.invertCulling = true;
		RefCamera.Render();
		GL.invertCulling = false;
		
		RefCamera.targetTexture.wrapMode = TextureWrapMode.Repeat;
		RefMat.SetTexture(ReflectTextureName, RefCamera.targetTexture);
	}
	Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		Vector3 offsetPos = pos + normal * clipPlaneOffset;
		Matrix4x4 m = cam.worldToCameraMatrix;
		Vector3 cpos = m.MultiplyPoint(offsetPos);
		Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
		
		return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
	}
	public static float Sgn(float a)
	{
		if (a > 0.0F)
		{
			return 1.0F;
		}
		if (a < 0.0F)
		{
			return -1.0F;
		}
		return 0.0F;
	}
	public static Matrix4x4 CalculateObliqueMatrix(Matrix4x4 projection, Vector4 clipPlane)
	{
		Vector4 q = projection.inverse * new Vector4(
			Sgn(clipPlane.x),
			Sgn(clipPlane.y),
			1.0F,
			1.0F
			);
		Vector4 c = clipPlane * (2.0F / (Vector4.Dot(clipPlane, q)));
		// third row = clip plane - fourth row
		projection[2] = c.x - projection[3];
		projection[6] = c.y - projection[7];
		projection[10] = c.z - projection[11];
		projection[14] = c.w - projection[15];
		
		return projection;
	}
}
