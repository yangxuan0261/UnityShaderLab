/*
 *Author       *Jens
 */
using UnityEngine;
using System.Collections;
using System.IO;

//using UnityEditor;
using UnityEngine.UI;

public class ScreenShot : MonoBehaviour
{
	//定义图片保存路径
	private string m_FullShotPath;
	private string m_partShotPath;
	private string m_OtherCameraPath;
	//这个不是Main相机，其他摄像机，默认不激活它
	public Camera CameraTrans;
	//显示图片
	public RawImage image;

	void Start ()
	{
		//初始化路径，实际使用应该用Application.persistentDataPath，
		//因为使用dataPath就是Asset文件不能读写操作
		// m_FullShotPath = Application.dataPath + "/Resources/FullScreenShot.png";
		m_FullShotPath = "E:\\temp_save\\aaa.png";
		m_partShotPath = Application.dataPath + "/Resources/PartScreenShot.png";
		m_OtherCameraPath = Application.dataPath + "/Resources/OtherCameraScreenShot.png";
	}

	//在Unity回调中初始化按钮
	void OnGUI ()
	{
		if (GUILayout.Button ("全屏截图", GUILayout.Height (50))) {
			print ("全屏截图OK");
			CaptureByUnity (m_FullShotPath);
//			AssetDatabase.Refresh ();
		}
		if (GUILayout.Button ("局部截图", GUILayout.Height (50))) {
			print ("局部截图OK");
			StartCoroutine (CaptureByRect (new Rect (0, 0, 1024, 768), m_partShotPath));
		}
		if (GUILayout.Button ("非MainCamera截图", GUILayout.Height (50))) {
			//启用顶视图相机
			CameraTrans.gameObject.SetActive (true);
			//禁用主相机
			Camera.main.enabled = false;
			//这里一定要先指定一个分辨率。不然截图就会卡顿，现在先写死值，以后再设置活
			StartCoroutine (CaptureByCamera (CameraTrans, new Rect (0, 0, 1024, 768),
				m_OtherCameraPath));
			print ("非MainCamera截图OK");
		}

		if (GUILayout.Button ("加载图片", GUILayout.Height (50))) {
			Debug.Log ("加载图片"); 
			Texture2D _tex = (Texture2D)Resources.Load ("FullScreenShot");
			image.texture = _tex;
		}
	}



	/// <summary>
	/// 使用Application类下的CaptureScreenshot()方法实现截图
	/// 优点：简单，可以快速地截取某一帧的画面、全屏截图
	/// 缺点：不能针对摄像机截图，无法进行局部截图
	/// </summary>
	/// <param name="mFileName">M file name.</param>
	private void CaptureByUnity (string mFileName)
	{
		ScreenCapture.CaptureScreenshot (mFileName, 4);
	}

	/// <summary>
	/// 根据一个Rect类型来截取指定范围的屏幕, 左下角为(0,0)
	/// 读取屏幕像素存储为纹理图片
	/// </summary>
	/// <param name="mRect">M rect.截屏的大小</param>
	/// <param name="mFileName">M file name.保存路径</param>
	private IEnumerator CaptureByRect (Rect mRect, string mFileName)
	{
		//等待渲染线程结束
		yield return new WaitForEndOfFrame ();
		//初始化Texture2D, 大小可以根据需求更改
		Texture2D mTexture = new Texture2D ((int)mRect.width, (int)mRect.height,
			                     TextureFormat.RGB24, false);
		//读取屏幕像素信息并存储为纹理数据
		mTexture.ReadPixels (mRect, 0, 0);
		//应用
		mTexture.Apply ();
		//将图片信息编码为字节信息
		byte[] bytes = mTexture.EncodeToPNG ();  
		//保存
		System.IO.File.WriteAllBytes (mFileName, bytes);
		//需要展示次截图，可以返回截图,或者使用out修饰参数也可以带出去
		//return mTexture;
	}

	/// <summary>
	/// 指定相机截图
	/// </summary>
	/// <returns>The by camera.</returns>
	/// <param name="mCamera">M camera.要被截屏的相机</param>
	/// <param name="mRect">M rect. 截屏的区域</param>
	/// <param name="mFileName">M file name.</param>
	private IEnumerator  CaptureByCamera (Camera mCamera, Rect mRect, string mFileName)
	{
		//等待渲染线程结束
		yield return new WaitForEndOfFrame ();
		//初始化RenderTexture   深度只能是【0、16、24】截不全图请修改
		RenderTexture mRender = new RenderTexture ((int)mRect.width, (int)mRect.height, 16);
		//设置相机的渲染目标
		mCamera.targetTexture = mRender;
		//开始渲染
		mCamera.Render ();
		//激活渲染贴图读取信息
		RenderTexture.active = mRender;
		Texture2D mTexture = new Texture2D ((int)mRect.width, (int)mRect.height, TextureFormat.RGB24, false);
		//读取屏幕像素信息并存储为纹理数据
		mTexture.ReadPixels (mRect, 0, 0);
		//应用
		mTexture.Apply ();
		//释放相机，销毁渲染贴图
		mCamera.targetTexture = null;   
		RenderTexture.active = null; 
		GameObject.Destroy (mRender);  
		//将图片信息编码为字节信息
		byte[] bytes = mTexture.EncodeToPNG ();  
		//保存
		System.IO.File.WriteAllBytes (mFileName, bytes);
		//需要展示次截图，可以返回截图
		//return mTexture;
	}


}
