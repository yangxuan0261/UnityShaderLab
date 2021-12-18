using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

public class Utils {

    // 组合目录
    public static string CombinePath(string p1, string p2) {
        bool isEndsWith = p1.EndsWith("/");
        bool isStartsWith = p2.StartsWith("/");
        if (isEndsWith && isStartsWith)
            return p1 + FixPath(p2, false);
        else if (!isEndsWith && !isStartsWith)
            return p1 + "/" + p2;
        else
            return p1 + p2;
    }

    // 修正路径
    // 为了方便路径操作, 对路径做修正
    // 约定:路径内都是"/", 没有"\\"
    // 约定:拼接目录前半段以"/"结尾
    // 约定:拼接目录的后半段不带"/"开头
    public static string FixPath(string path, bool isFirstPart = true) {
        path = path.Replace("\\", "/");
        if (isFirstPart)
            if (!path.EndsWith("/")) path = path + "/";
            else
        if (path.StartsWith("/")) path = path.Substring(1, path.Length - 1);
        return path;
    }

    public static string LuaPath(string relaPath = "") {
        if (!Const.LuaBundleMode) {
            return CombinePath(CombinePath(Application.dataPath, "code/lua"), relaPath);
        } else {
            return CombinePath(CombinePath(Application.persistentDataPath, Const.kResluaDir), relaPath);
        }
    }

    public static string ResourcePath(string relaPath = "") {
        if (!Const.UseAssetBundle) {
            return CombinePath(CombinePath(Application.dataPath, "res"), relaPath);
        } else {
            return CombinePath(CombinePath(Application.persistentDataPath, Const.kResArtDir), relaPath);
        }
    }

    public static string PersistentDataPath(string relaPath = "") {
        return CombinePath(Application.persistentDataPath, relaPath);
    }

    public static byte[] GetLuaAssetsByteData(string relaPath, string fileType) {
        string RootPath = GetLuaAssetsPath(relaPath);
        byte[] fileContent = null;
        if (Application.platform == RuntimePlatform.Android) {
            if ((!File.Exists(PersistentDataPath(CombinePath(fileType, relaPath)))) && IsFileInStreamingAssetsPath(RootPath)) {
                //目前只对lua获取操作,初始化tolua最小环境
                fileContent = ReadAllByteFromStreamingAssets(RootPath);
                WriteFile(PersistentDataPath(CombinePath(fileType, relaPath)), fileContent);
                return fileContent;
            }
        }
        if (!File.Exists(RootPath)) {
            return fileContent;
        }
        try {
            fileContent = System.IO.File.ReadAllBytes(RootPath);
        } catch (Exception e) {
            LogUtil.E("文件读取错误:" + RootPath);
            return null;
        }
        return fileContent;
    }

    public static string GetLuaAssetsPath(string relaPath, string fileType = "") {
        string filePath = CombinePath(Const.kResluaDir, relaPath);
        string RootPath = "";
#if UNITY_EDITOR
        RootPath = LuaPath(relaPath);
#else
        RootPath = PersistentDataPath(filePath);
        if (!File.Exists(RootPath)) {
            RootPath = GetAssetsPath(filePath);
        }
#endif
        return RootPath;
    }

    public static byte[] ReadAllByteFromStreamingAssets(string url) {
        WWW w = new WWW(url);
        while (!w.isDone) { Thread.Sleep(0); }
        return w.bytes;
    }

    public static string GetAssetsPath(string relaPath = "") {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            return string.Format("{0}/Raw/{1}", Application.dataPath, relaPath);
        } else if (Application.platform == RuntimePlatform.Android) {
            return CombinePath(Application.streamingAssetsPath, relaPath);
        } else if (Application.platform == RuntimePlatform.WindowsPlayer ||
            Application.platform == RuntimePlatform.WindowsEditor) {
            return CombinePath(Application.streamingAssetsPath, relaPath);
        } else {
            return string.Format("{0}/{1}", Application.dataPath, relaPath);
        }
    }

    // todo: 先不做加密偏移
    public static AssetBundle CreateBundleFromFile(string path) {
        if ((Application.platform == RuntimePlatform.Android) && IsFileInStreamingAssetsPath(path)) {
            path = path.Substring(11).Replace("!/", "!"); // 去掉jar:file:// 去掉!/assets中间的/
        }
#if ENCRPYT_AB
        string[] splitedPath = path.Split("/\\".ToCharArray());
        ulong offset = (ulong) splitedPath[splitedPath.Length - 1].Length % 4 + 7;
        return AssetBundle.LoadFromFile(path, 0, offset);
#else
        return AssetBundle.LoadFromFile(path);
#endif
    }

    public static AssetBundleCreateRequest CreateBundleFromFileAsync(string path) {
        if ((Application.platform == RuntimePlatform.Android) && IsFileInStreamingAssetsPath(path)) {
            path = path.Substring(11).Replace("!/", "!"); // 去掉jar:file:// 去掉!/assets中间的/
        }
#if ENCRPYT_AB
        string[] splitedPath = path.Split("/\\".ToCharArray());
        ulong offset = (ulong) splitedPath[splitedPath.Length - 1].Length % 4 + 7;
        return AssetBundle.LoadFromFileAsync(path, 0, offset);
#else
        return AssetBundle.LoadFromFileAsync(path);
#endif
    }

    public static AssetBundle CreateEncryptAb(string path) {
        string[] splitedPath = path.Split("/\\".ToCharArray());
        ulong offset = (ulong) splitedPath[splitedPath.Length - 1].Length % 4 + 7;
        return AssetBundle.LoadFromFile(path, 0, offset);
    }

    public static bool IsFileExist(string path) {
        return System.IO.File.Exists(path);
    }

    public static Exception WriteFile(string path, byte[] content) {
        string dirPath = System.IO.Path.GetDirectoryName(path);
        if (!IsDirectoryExist(dirPath)) {
            CreateDirectory(dirPath);
        }

        FileStream fs = null;
        try {
            fs = new FileStream(path, FileMode.Create);
            fs.Write(content, 0, content.Length);
        } catch (System.Exception ex) {
            return ex;
        } finally {
            if (fs != null) {
                fs.Close();
            }
        }
        return null;
    }

    public static Exception WriteFileUTF8(string path, string content) {
        byte[] cbytes = System.Text.UTF8Encoding.UTF8.GetBytes(content);
        return WriteFile(path, cbytes);
    }

    public static bool IsDirectoryExist(string path) {
        return System.IO.Directory.Exists(path);
    }

    public static void MoveFile(string src, string dst) {
        if (File.Exists(dst))
            File.Delete(dst);

        string dirPath = System.IO.Path.GetDirectoryName(dst);
        if (!Directory.Exists(dirPath))
            CreateDirectory(dirPath);

        File.Move(src, dst);
    }

    public static void CopyFile(string src, string dst, bool isWWWRead = false) {
        if (File.Exists(dst))
            File.Delete(dst);

        string dirPath = System.IO.Path.GetDirectoryName(dst);
        if (!Directory.Exists(dirPath))
            CreateDirectory(dirPath);
        if (isWWWRead) {
            byte[] content = ReadAllBytesFromWWW(WWWPrefix(src));
            WriteFile(dst, content);
        } else {
            File.Copy(src, dst, true);
        }
    }

    // justSub 代表只拷贝目录下的所有东西, 不包括目录本身
    public static void CopyDir(string src, string dst, string searchPattern, bool justSub = true) {
        if (!Directory.Exists(src)) {
            return;
        }

        src = src.Replace("\\", "/");
        dst = dst.Replace("\\", "/");

        if (!justSub) {
            string name = Path.GetFileName(src);
            dst = Path.Combine(dst, name).Replace("\\", "/");
        }

        string[] files = Directory.GetFiles(src, searchPattern, SearchOption.AllDirectories);
        foreach (string file in files) {
            string dstFilePath = file.Replace("\\", "/").Replace(src, dst);

            string dir = Path.GetDirectoryName(dstFilePath);
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }

            File.Copy(file, dstFilePath, true);
        }
    }

    public static string JsonBeauty(string str) {
        object obj = LitJson.JsonMapper.ToObject<object>(str);
        LitJson.JsonWriter writer = new LitJson.JsonWriter();
        writer.PrettyPrint = true;
        LitJson.JsonMapper.ToJson(obj, writer);
        return writer.TextWriter.ToString();
    }

    public static bool CreateDirectory(string path) {
        bool result = true;
        try {
            System.IO.Directory.CreateDirectory(path);
        } catch (Exception exp) {
            LogUtil.E(string.Format("create directory fail {0}", exp.ToString()));
            result = false;
        }
        return result;
    }

    public static string StreamingAssetsPath(string relaPath = "") {
        return CombinePath(Application.streamingAssetsPath, relaPath);
    }

    public static string ProjectLuaPath(string relaPath = "") {
        // TODO: 测试重启虚拟机, 111
        // string luaPath = Path.Combine(Application.persistentDataPath, relaPath);
        // if (File.Exists(luaPath)) {
        //     return luaPath;
        // }
        // return CombinePath(CombinePath("C:/TS/rummy_itc/Assets", "Code"), relaPath);
        return CombinePath(CombinePath(Application.dataPath, "Code"), relaPath);
    }

    public static bool IsFileInStreamingAssetsPath(string filePath) {
        string streamingAssetsPath = StreamingAssetsPath();
        return filePath.StartsWith(streamingAssetsPath);
    }

    // 从左至右, 只要有一个 v2 > v1 则判断为 v2 为新版本
    public static bool VersionCompare(string v1, string v2) {
        LogUtil.D("--- VersionCompare, v1:{0}, v2:{1}", v1, v2);
        bool is1Less2 = false;
        try {
            string[] v1Arr = v1.Split('.');
            string[] v2Arr = v2.Split('.');
            if (v1Arr.Length == v2Arr.Length) {
                List<int> v1List = new List<int>();
                List<int> v2List = new List<int>();
                Array.ForEach(v1Arr, ele => { v1List.Add(int.Parse(ele)); });
                Array.ForEach(v2Arr, ele => { v2List.Add(int.Parse(ele)); });

                for (int i = 0; i < v2List.Count; i++) {
                    if (v2List[i] != v1List[i]) { // 不相等的情况下才能进行判断
                        is1Less2 = v2List[i] > v1List[i];
                        break;
                    }
                }
            }
        } catch (System.Exception e) {
            LogUtil.E("--- 版本比较出错, v1:{0}, v2:{1}, msg:{2}", v1, v2, e.Message);
            is1Less2 = true;
        }
        return is1Less2;
    }

    // 获取版本号前3位
    public static string Version3flag(string v1) {
        string[] v1Arr = v1.Split('.');
        if (v1Arr != null && v1Arr.Length == 4) {
            List<string> v1List = v1Arr.ToList();
            v1List.RemoveAt(3);
            return String.Join(".", v1List.ToArray());
        } else {
            return "";
        }
    }

    public static string md5file(string file) {
        try {
            FileStream fs = new FileStream(file, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++) {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        } catch (Exception ex) {
            throw new Exception("md5file() fail, error:" + ex.Message);
        }
    }

    public static string md5str(string content) {
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] data = System.Text.Encoding.UTF8.GetBytes(content);
        byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
        md5.Clear();

        string destString = "";
        for (int i = 0; i < md5Data.Length; i++) {
            destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
        }
        destString = destString.PadLeft(32, '0');
        return destString;
    }

    public static string GetResourceDataPath(string relaPath) {
        string filePath = CombinePath(Const.kResArtDir, relaPath);
        string RootPath = "";
        RootPath = ResourcePath(relaPath);
        if (Const.UseAssetBundle) {
            RootPath = PersistentDataPath(filePath); //先读热更资源
            if (!File.Exists(RootPath)) {
                RootPath = GetAssetsPath(filePath); //再读包内资源
            }
        }
        return RootPath;
    }

    public static void DeleteFolder(string path) {
        if (Directory.Exists(path))
            Directory.Delete(path, true);
    }

    public static void DeleteFile(string path) {
        if (File.Exists(path))
            File.Delete(path);
    }

    public static bool IsOverUI() {
        if (EventSystem.current) {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
        return false;
    }

    public static void SetDebugDrawDirty() {
        // DebugDraw.Ins.SetDirty();
    }

    public static float GetAngleVec2(Vector2 pos1, Vector2 pos2) {
        // float value = (float)((Mathf.Atan2(pos2.x - pos1.x, pos2.y - pos1.y) / Math.PI) * 180f);
        // if(value < 0) value += 360f;
        // return value;

        Vector2 direction = pos2 - pos1;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0f) angle += 360f;
        return angle;
    }

    // ---------- bytes to any begin ----------
    public static string BytesToUTF8(byte[] bytes) {
        return Encoding.UTF8.GetString(bytes);
    }

    public static byte[] UTF8ToBytes(string str) {
        return Encoding.UTF8.GetBytes(str);
    }

    public static string ReadAllTextFromFile(string path) {
        return System.IO.File.ReadAllText(path);
    }

    public static byte[] ReadAllBytesFromFile(string path) {
        return System.IO.File.ReadAllBytes(path);
    }

    public static string Base64Encode(string plainText) {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes).Replace("\n", "");
    }

    public static string Base64Decode(string base64EncodedData) {
        var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public static string ReadFileEncrypt(string path) {
        try {
            byte[] bts = ReadAllBytesFromFile(path);
            bts = AESUtil.aesDecryptBase64(bts, Const.kAesKey);
            return Utils.BytesToUTF8(bts);
        } catch (System.Exception e) {
            return "";
        }
    }

    public static Exception WriteFileEncrypt(string path, string content) {
        byte[] bts = Utils.UTF8ToBytes(content);
        bts = AESUtil.aesEncryptBase64(bts, Const.kAesKey);
        return WriteFile(path, bts);
    }

    public static string ReadAllTextFromWWW(string url) {
        WWW w = new WWW(url);
        while (!w.isDone) { Thread.Sleep(10); }
        return w.text;
    }

    public static byte[] ReadAllBytesFromWWW(string url) {
        WWW w = new WWW(url);
        while (!w.isDone) { Thread.Sleep(10); }
        return w.bytes;
    }

    public static IEnumerator ReadByteOfFileByWWW(string url, string outfile) {
        WWW www = new WWW(url);
        yield return www;
        if (www.isDone) {
            int index = outfile.LastIndexOf('/');
            string dirPath = outfile.Substring(0, index);
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            File.WriteAllBytes(outfile, www.bytes);
        }
        yield return 0;
    }

    public static Texture ReadLocalTexFromWWW(string url) {
        WWW w = new WWW(WWWPrefix(url));
        while (!w.isDone) { Thread.Sleep(10); }
        return w.texture;
    }

    // 加载本地图片生成 Sprite
    public static Sprite GetLocalSprite(string path) {
        if (!File.Exists(path)) {
            return null;
        }
        byte[] byteArray;
        try {
            byteArray = File.ReadAllBytes(path);
        } catch (System.Exception e) {
            Debug.LogException(e);
            DeleteFile(path);
            return null;
        }
        Texture2D texture = Byte2Tex2D(byteArray, 0, 0, TextureFormat.RGB24);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 1f);
        return sprite;
    }

    public static Texture2D Byte2Tex2D(byte[] data, int width, int height, TextureFormat format) {
        Texture2D texture = new Texture2D(width, height, format, false);
        texture.LoadImage(data);
        return texture;
    }

    public static Texture2D CreateTexture2D(string path, int width, int height) {
        if (!File.Exists(path)) {
            return null;
        }

        Texture2D texture = null;
        try {
            byte[] byteArr = File.ReadAllBytes(path);
            texture = new Texture2D(width, height);
            texture.LoadImage(byteArr);
        } catch (System.Exception e) {
            // GameMgr.ReportException(e);
            return null;
        }
        return texture;
    }

    public static Sprite CreateSprite(Texture2D texture) {
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero, 1f);
        return sprite;
    }

    public static byte[] GetLuaByteData(string relaPath, bool isAb = false) {
        byte[] content = null;
        if (!Const.UseAssetBundle && !Const.LuaBundleMode) {
            string projPath = ProjectLuaPath(relaPath);
            if (File.Exists(projPath)) {
                content = ReadAllBytesFromFile(projPath);
            }
        } else {
            content = GetBytes(relaPath);
        }
        return content;
    }

    public static byte[] GetBytes(string relaPath) {
        byte[] content = null;
        string perPath = PersistentDataPath(relaPath);
        if (!File.Exists(perPath)) {
            string packPath = GetAssetsPath(relaPath);
            if (Application.platform == RuntimePlatform.Android) {
                content = ReadAllBytesFromWWW(WWWPrefix(packPath));
            } else {
                if (!File.Exists(packPath)) {
                    return null;
                } else {
                    content = ReadAllBytesFromFile(packPath);
                }
            }
        } else {
            content = ReadAllBytesFromFile(perPath);
        }
        return content;
    }

    // WWW类使用路径的前缀
    public static string WWWPrefix(string path = "") {
        switch (Application.platform) {
            case RuntimePlatform.Android:
                return path;
                break;

            case RuntimePlatform.IPhonePlayer:
            case RuntimePlatform.OSXEditor:
                return "file://" + path;
                break;

            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
                return "file:///" + path;
                break;

            default:
                LogUtil.E("[WWWPrefix] 不支持的平台:" + Application.platform.ToString());
                return "";
        }
    }
    // ---------- bytes to any end ----------

    // ---------- Camera begin ----------
    public static Vector2 ScreenPointToLocalPointInRectangle(RectTransform rect, Vector2 screenPoint, Camera cam) {
        Vector2 tempWorldPos = new Vector3();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPoint, cam, out tempWorldPos);
        return tempWorldPos;
    }

    public static Vector3 ScreenPointToWorldPointInRectangle(RectTransform rect, Vector2 screenPoint, Camera cam) {
        Vector3 tempWorldPos = new Vector3();
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, screenPoint, cam, out tempWorldPos);
        return tempWorldPos;
    }

    public static Vector3 WorldToScreenPoint(Camera cam, Vector3 pos) {
        return cam.WorldToScreenPoint(pos);
    }

    public static string SystemLanguage() {
        String langStr = Application.systemLanguage.ToString();
        if (langStr.Contains("Chinese") || langStr.Contains("chinese")) {
            langStr = "Chinese";
        }
        return langStr;
    }

    public static string StrFormat(string fmt, params object[] args) {
        return string.Format(fmt, args);
    }
    // ---------- Camera begin ----------

    // ---------- Component begin ----------
    public static Component AddComponent(GameObject go, string className) {
        if (Application.isEditor) {
            System.Reflection.Assembly[] assemblys = System.AppDomain.CurrentDomain.GetAssemblies();
        }
        Type t = Type.GetType(className);
        return AddComponent(go, t);
    }

    public static Component AddComponent(GameObject go, Type type) {
        return go.AddComponent(type);
    }

    public static Component AddComponent<T>(GameObject go) where T : Component {
        return go.AddComponent<T>();
    }

    public static Component GetComponent(GameObject go, string className) {
        return go.GetComponent(className);
    }

    public static Component GetComponent(GameObject go, Type t) {
        return go.GetComponent(t);
    }
    // ---------- Component end ----------

    // ---------- 位操作 begin ----------
    public static int BitLshift(int n, int c) {
        return n << c;
    }

    public static int StrBitLshift(int n, string c) {
        int cInt = Convert.ToInt32(c, 2);
        return n << cInt;
    }

    public static int BitRshift(int n, int c) {
        return n >> c;
    }

    public static int StrBitRshift(int n, string c) {
        int cInt = Convert.ToInt32(c, 2);
        return n >> cInt;
    }

    public static int Inv(int n) {
        return ~n;
    }

    public static int StrBitToInt32(string l) {
        int lInt = Convert.ToInt32(l, 2);
        return lInt;
    }

    public static int BitAnd(int l, int r) {
        return l & r;
    }

    public static int StrBitAnd(string l, string r) {
        int lInt = Convert.ToInt32(l, 2);
        int rInt = Convert.ToInt32(r, 2);
        return lInt & rInt;
    }

    public static int BitOr(int l, int r) {
        return l | r;
    }

    public static int StrBitOr(string l, string r) {
        int lInt = Convert.ToInt32(l, 2);
        int rInt = Convert.ToInt32(r, 2);
        return lInt | rInt;
    }

    public static int BitXor(int l, int r) {
        return l ^ r;
    }

    public static int StrBitXor(string l, string r) {
        int lInt = Convert.ToInt32(l, 2);
        int rInt = Convert.ToInt32(r, 2);
        return lInt ^ rInt;
    }
    // ---------- 位操作 end ----------

    // ---------- 转化 table 为数组 end----------

    public static GameObject GetChildRecursive(GameObject targetGo, string name) {
        Transform targetTrans = targetGo.transform;
        for (int i = 0; i < targetTrans.childCount; i++) {
            Transform child = targetTrans.GetChild(i);
            if (child.name == name) {
                return child.gameObject;
            } else {
                GameObject go = GetChildRecursive(child.gameObject, name);
                if (go != null) {
                    return go;
                }
            }
        }
        return null;
    }

    public static void ExecutePointerClick(GameObject go) {
        ExecuteEvents.Execute<IPointerClickHandler>(go, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
    }

    /// <summary>
    /// 获取设备 id 接口
    /// </summary>
    /// <returns></returns>
    public static string GetDeviceID() {
        string mDeviceUniqueIdentifier;
#if UNITY_IOS && !UNITY_EDITOR
        if (UnityEngine.iOS.Device.advertisingTrackingEnabled && !string.IsNullOrEmpty(UnityEngine.iOS.Device.advertisingIdentifier)) {
            mDeviceUniqueIdentifier = UnityEngine.iOS.Device.advertisingIdentifier;
        } else if (!string.IsNullOrEmpty(UnityEngine.iOS.Device.vendorIdentifier)) {
            mDeviceUniqueIdentifier = UnityEngine.iOS.Device.vendorIdentifier;
        } else {
            mDeviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
        }
#else
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer) {
            // mDeviceUniqueIdentifier = SystemInfo.deviceName; // PC 上会出现不唯一的情况
            string deviceKey = "PC_DeviceID";
            mDeviceUniqueIdentifier = GetLocalCacheStr(deviceKey);
            if (mDeviceUniqueIdentifier.Length == 0 || mDeviceUniqueIdentifier.Length != 32) {
                mDeviceUniqueIdentifier = md5str(GetRandomString(10));
                SetLocalCacheStr(deviceKey, mDeviceUniqueIdentifier);
            }
        } else {
            mDeviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
        }
#endif
        return mDeviceUniqueIdentifier;
        /*if (Application.platform == RuntimePlatform.WindowsEditor)
            return SystemInfo.deviceName;
        return SystemInfo.deviceUniqueIdentifier;*/
    }

    public static string GetRandomString(int iLength) {
        string RandomString = string.Empty;
        char Randomchar = 'a';
        for (int i = 0; i < iLength; i++) {
            Randomchar = (char) UnityEngine.Random.Range(97, 123);
            RandomString += Randomchar.ToString();
        }
        return RandomString;
    }

    public static int LongToInt(long num) {
        return (int) num;
    }

    public static string GetApiVersion() {
        return Const.ApiVersion;
    }

    // 获取Transform在场景中的层级路径
    public static string GetTransformPath(Transform transform, bool editorOnly = true) {
        if (!editorOnly || Application.isEditor) {
            List<string> paths = new List<string>();
            while (transform != null) {
                paths.Insert(0, transform.name);
                transform = transform.parent;
            }
            return string.Join("/", paths.ToArray());
        } else {
            return transform.name;
        }
    }

    // 保存/修改本地持久缓存数据
    public static void SetLocalCacheStr(string key, string value) {
        PlayerPrefs.SetString(key, value);
    }
    public static void SetLocalCacheInit(string key, int value) {
        PlayerPrefs.SetInt(key, value);
    }
    // 读取本地持久缓存数据
    public static String GetLocalCacheStr(string key, string _default = "") {
        string res = PlayerPrefs.GetString(key, _default);
        return res;
    }
    public static int GetLocalCacheInit(string key, int _default = 0) {
        return PlayerPrefs.GetInt(key, _default);
    }

    public static bool CompareUInt(string num1, string num2) {
        ulong uint_1 = 0;
        ulong uint_2 = 0;
        if (UInt64.TryParse(num1, out uint_1) && UInt64.TryParse(num2, out uint_2)) {
            return uint_1 > uint_2;
        }
        return false;
    }

    // 获取字符串在 Text 中的绘制长度（用于做 Text 的自适应）
    public static int GetFontlen(string str, int fontSize, string fontName = "Arial") {
        int len = 0;
        Font font;
        font = Font.CreateDynamicFontFromOSFont(fontName, fontSize);
        font.RequestCharactersInTexture(str);
        CharacterInfo ch;
        for (int i = 0; i < str.Length; i++) {
            font.GetCharacterInfo(str[i], out ch);
            len += ch.advance;
        }
        // 不销毁会导致闪退
        GameObject.Destroy(font);
        return len;
    }

    public static int GetFontlenByFont(Font font, String str, int fontSize) {
        int len = 0;
        if (font == null) {
            Debug.LogError("GetFontlenByFont need a font");
            return len;
        }
        font.RequestCharactersInTexture(str, fontSize, FontStyle.Normal);
        CharacterInfo ch;
        for (int i = 0; i < str.Length; i++) {
            font.GetCharacterInfo(str[i], out ch, fontSize);
            len += ch.advance;
        }
        return len;
    }

    // 获取当前分辨率
    public static Vector2 GetScreenPixel() {
        Resolution[] resolutions = Screen.resolutions;
        return new Vector2(resolutions[0].width, resolutions[0].height);
    }

    // // 生成截屏
    // public static void MakeScreenShot(Camera camera, Vector2 distanceXY, Vector2 distanceWH, string path) {
    //     RenderTexture rt = new RenderTexture((int) distanceWH.x, (int) distanceWH.y, 1);
    //     camera.targetTexture = rt;
    //     camera.Render();
    //     camera.gameObject.SetActive(false);
    //     RenderTexture tempRT = RenderTexture.active;
    //     RenderTexture.active = rt;
    //     GameMgr.Instance.StartCoroutine(BuilderScreenShot(camera, distanceXY, distanceWH, path));
    //     camera.targetTexture = null;
    //     RenderTexture.active = tempRT;
    //     GameObject.Destroy(rt);
    //     GameObject.Destroy(tempRT);
    // }

    // 缩放一张 Texture2D
    public static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targHeight) {
        Texture2D scaledTex = new Texture2D(targetWidth, targHeight, source.format, false);
        Color color;
        try {
            for (int i = 0; i < targetWidth; i++) {
                for (int j = 0; j < targHeight; j++) {
                    // 通过纹理的 UV 值获取指定像素的颜色值
                    color = source.GetPixelBilinear((float) i / (float) targetWidth, (float) j / (float) targHeight);
                    scaledTex.SetPixel(i, j, color);
                }
            }
            scaledTex.Apply();
        } catch (Exception e) {
            LogUtil.E("scale texture2d fail:" + e.Message);
        }
        return scaledTex;
    }

    public static Texture2D TextureToTexture2D(Texture texture, TextureFormat fmt = TextureFormat.RGB24) {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = RenderTexture.GetTemporary(
            texture.width,
            texture.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);
        Graphics.Blit(texture, renderTexture);
        RenderTexture.active = renderTexture;

        Texture2D texture2D = new Texture2D(texture.width, texture.height, fmt, false);
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(renderTexture);
        return texture2D;
    }

    public static bool SaveTexToPng(Texture source, String filePath, int targetWidth, int targHeight) {
        if (source == null) return false;
        try {
            Texture2D readableText = Utils.TextureToTexture2D(source);
            if (readableText.width != targetWidth || readableText.height != targHeight) {
                readableText = ScaleTexture(readableText, targetWidth, targHeight);
            }
            return SaveTex2DAsPng(readableText, filePath);;
        } catch (Exception e) {
            LogUtil.E("write texture2d to file fail:" + e.Message);
            return false;
        }
    }

    public static bool SaveTex2DToPng(Texture2D source, String filePath, int targetWidth, int targHeight) {
        if (source == null) return false;
        try {
            Texture2D readableText = Utils.TextureToTexture2D(source, source.format);
            if (readableText.width != targetWidth || readableText.height != targHeight) {
                readableText = ScaleTexture(readableText, targetWidth, targHeight);
            }
            return SaveTex2DAsPng(readableText, filePath);;
        } catch (Exception e) {
            LogUtil.E("write texture2d to file fail:" + e.Message);
            return false;
        }
    }

    public static bool SaveTexture2DAsFile(Texture2D source, String filePath) {
        if (source == null) return false;
        return SaveTex2DToPng(source, filePath, source.width, source.height);
    }

    public static bool SaveTex2DAsPng(Texture2D source, string filePath) {
        try {
            //这里可以转 JPG PNG EXR  Unity都封装了固定的Api
            byte[] imagebytes = source.EncodeToPNG();
            WriteFile(filePath, imagebytes);
        } catch (Exception e) {
            LogUtil.E("write texture2d to file fail:" + e.Message);
            return false;
        }
        return true;
    }

    static IEnumerator BuilderScreenShot(Camera camera, Vector2 distanceXY, Vector2 distanceWH, string path) {
        yield return new WaitForEndOfFrame();
        Rect rect = new Rect(distanceXY.x, distanceXY.y, distanceWH.x, distanceWH.y);
        Texture2D screenShot = new Texture2D((int) (rect.width), (int) (rect.height), TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0, false);
        screenShot.Apply();
        //GameObject.Destroy(camera.transform.parent.gameObject);
        SaveTex2DAsPng(screenShot, path);
        yield return null;
    }

    // 震动
    public static void Vibrate() {
#if UNITY_ANDROID || UNITY_IPHONE
        Handheld.Vibrate();
#endif
    }

    //正则检测字符串格式
    public static bool CheckFormat(String str, String reg) {
        Regex regex = new Regex(@reg);
        if (regex.IsMatch(str)) {
            return true;
        } else {
            return false;
        }
    }

    // 获取当前的网络类型
    public static String CheckNetReach() {
        String net_type = "UNKNOWN";
        switch (Application.internetReachability) {
            case NetworkReachability.NotReachable: // 网络断开
                net_type = "NONE";
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork: // 移动网络
                net_type = "WWLAN";
                break;
            case NetworkReachability.ReachableViaLocalAreaNetwork: // wifi网络
                net_type = "WIFI";
                break;
        }
        return net_type;
    }

    // ---------- 筹码数运算 begin----------

    // 对筹码数进行运算
    public static string OperationChips(string operatorStr, string num1, string num2) {
        // Debug.Log("进入 Utils.cs文件 OperationChips方法，计算 " + num1 + " " + operatorStr + " " + num2);
        // 容错判断
        if (operatorStr == null || num1 == null || num2 == null) {
            Debug.Log("传递过来的计算参数不正确");
            return "-1";
        }
        if (num1.Contains(".") || num2.Contains(".")) {
            return OperationChipsOfDecimal(operatorStr, num1, num2);
        } else if (!num1.Contains(".") && !num2.Contains(".")) {
            return OperationChipsOfInt(operatorStr, num1, num2);
        }
        return "";
    }

    // 整型运算
    public static string OperationChipsOfInt(string operatorStr, string num1, string num2) {
        // 将数值转换成64位整型
        UInt64 num1_64 = 0;
        UInt64 num2_64 = 0;
        if (UInt64.TryParse(num1, out num1_64)) {
            // Debug.Log("成功，转换为UInt64类型，" + num1 + " To " + num1_64);
        } else {
            // Debug.Log("失败，转换为UInt64类型，" + num1);
            return "";
        }
        if (UInt64.TryParse(num2, out num2_64)) {
            // Debug.Log("成功，转换为UInt64类型，" + num2 + " To " + num2_64);
        } else {
            // Debug.Log("失败，转换为UInt64类型，" + num2);
            return "";
        }
        // 计算
        UInt64 result = 0;
        if (operatorStr.Equals("+")) {
            result = num1_64 + num2_64;
        } else if (operatorStr.Equals("-")) {
            result = num1_64 - num2_64;
        } else if (operatorStr.Equals("*")) {
            result = num1_64 * num2_64;
        } else if (operatorStr.Equals("/")) {
            result = num1_64 / num2_64;
        }
        // Debug.Log("运算结果: " + result.ToString());
        return result.ToString();
    }

    // 小数运算
    public static string OperationChipsOfDecimal(string operatorStr, string num1, string num2) {
        // 将数值转换成Double类型
        Double num1_double = 0;
        Double num2_double = 0;
        if (Double.TryParse(num1, out num1_double)) {
            // Debug.Log("成功，转换为Double类型，" + num1 + " To " + num1_double);
        } else {
            // Debug.Log("失败，转换为Double类型，" + num1);
            return "";
        }
        if (Double.TryParse(num2, out num2_double)) {
            // Debug.Log("成功，转换为Double类型，" + num2 + " To " + num2_double);
        } else {
            // Debug.Log("失败，转换为Double类型，" + num2);
            return "";
        }
        // 计算
        Double result = 0;
        if (operatorStr.Equals("+")) {
            result = num1_double + num2_double;
        } else if (operatorStr.Equals("-")) {
            result = num1_double - num2_double;
        } else if (operatorStr.Equals("*")) {
            result = num1_double * num2_double;
        } else if (operatorStr.Equals("/")) {
            result = num1_double / num2_double;
        }
        // Debug.Log("运算结果: " + result.ToString());
        return result.ToString();
    }

    // 比较筹码数
    public static int CompareChips(string num1, string num2) {
        // Debug.Log("进入 Utils.cs文件 CompareChips方法，比较大小 " + num1 + " 和 " + num2);
        // 容错判断
        if (num1 == null || num2 == null) {
            Debug.Log("传递过来的比较参数不正确");
            return 0;
        }
        int result = 0; // 0代表相等，小于0代表小于，大于0代表大于
        UInt64 num1_64 = 0;
        UInt64 num2_64 = 0;
        if (UInt64.TryParse(num1, out num1_64) && UInt64.TryParse(num2, out num2_64)) {
            // Debug.Log("成功，转换为UInt64类型，" + num1 + " To " + num1_64 + ", " + num2 + " To " + num2_64);
            result = num1_64.CompareTo(num2_64);
        } else if (num1.Contains(".") || num2.Contains(".")) {
            Double num1_double = 0;
            Double num2_double = 0;
            if (Double.TryParse(num1, out num1_double) && Double.TryParse(num2, out num2_double)) {
                // Debug.Log("成功，转换为Double类型，" + num1 + " To " + num1_double + ", " + num2 + " To " + num2_double);
                result = num1_double.CompareTo(num2_double);
            }
        }
        // Debug.Log("运算结果: " + result);
        return result;
    }

    // ---------- 筹码数运算 end ----------

}