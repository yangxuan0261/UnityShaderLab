using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class EditorUtils {

    public static void ClearConsole() {
        var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);
    }

    public static void CopyTransform(Transform src, Transform dst) {
        dst.localPosition = src.localPosition;
        dst.localScale = src.localScale;
        dst.localRotation = src.localRotation;
    }

    public static void ResetTransform(Transform trans) {
        trans.localPosition = Vector3.zero;
        trans.localScale = Vector3.one;
        trans.localRotation = Quaternion.Euler(0, 0, 0);
    }

    // 获取同目录下, 同文件名, 不同扩展名的 文件路径, (unity文件系统路径)
    public static string GetSameNameFile(string path, string extName) {
        string fileName = Path.GetFileNameWithoutExtension(path);
        string dirName = Path.GetDirectoryName(path);
        return Path.Combine(dirName, fileName + extName).Replace("\\", "/");
    }

    public static T GetSameNameFile<T>(string path, string extName) where T : UnityEngine.Object {
        string assetPath = GetSameNameFile(path, extName);
        return AssetDatabase.LoadAssetAtPath<T>(assetPath);;
    }

    public static void DrawChangeLine() {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(".");
        GUILayout.EndHorizontal();
    }

    public static void RevealInFinder(string path) {
        if (Directory.Exists(path) || File.Exists(path)) {
            EditorUtility.RevealInFinder(path);
        } else {
            UnityEngine.Debug.LogErrorFormat("--- no dir: {0}", path);
        }
    }

    // 获取项目路径
    public static string GetProjPath() {
        string assetPath = Application.dataPath.Replace("\\", "/");;
        return assetPath.Substring(0, assetPath.LastIndexOf("/Assets"));
    }

    public static string GetAssetPath(string fullPath) {
        int pos = fullPath.IndexOf("Assets");
        return pos > -1 ? fullPath.Substring(pos) : fullPath;
    }

    public static string GetFullPath(string assetPath) {
        if (assetPath.StartsWith("Assets")) {
            return Path.Combine(Directory.GetCurrentDirectory(), assetPath).Replace("\\", "/");
        }
        return assetPath;
    }

    public static void Assert(bool b, string format, params object[] args) {
        if (!b) {
            throw new Exception(string.Format(format, args));
        }
    }

    // 可以获取到 py 脚本 print 的值
    public static string ProcCmd(string command, string argument) {
        ProcessStartInfo psi = new ProcessStartInfo(command);
        psi.Arguments = argument;
        psi.CreateNoWindow = true;
        psi.ErrorDialog = true;
        psi.UseShellExecute = false; 

        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;
        psi.RedirectStandardInput = true;
        psi.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
        psi.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;

        Process p = Process.Start(psi); 

        StringBuilder sb1 = new StringBuilder();
        StreamReader reader = p.StandardOutput;
        while (!reader.EndOfStream) {
            string line = reader.ReadLine();
            if (!line.StartsWith("---")) { // 过滤掉自定义的日志, 方便 py 调试
                sb1.Append(line);
            }
            if (!reader.EndOfStream) {
                sb1.Append("\n");
            }
        }
        reader.Close();

        StringBuilder sb2 = new StringBuilder();
        StreamReader errorReader = p.StandardError;
        while (!errorReader.EndOfStream) {
            sb2.Append(errorReader.ReadLine()).Append("\n");
        }
        errorReader.Close();

        p.WaitForExit();
        p.Close();

        if (sb2.Length > 0) {
            throw new Exception(string.Format("--- Error, python error, msg:{0}", sb2.ToString()));
        }
        return sb1.ToString();
    }

    // 弹窗执行 py, 新开线程, 不阻塞 gui
    public static void ProcCmdOnWin(string command, string argument) {
        UnityEngine.Debug.LogFormat("--- ProcCmdOnWin: {0} {1}", command, argument);
#if UNITY_EDITOR_WIN
        ProcCmdOnWindows(command, argument);
#elif UNITY_EDITOR_OSX
        ProcCmdOnMacOs(command, argument);
#else
        EditorUtils.Assert(false, "--- do not support");
#endif
    }

    public static void ProcCmdOnWindows(string command, string argument) {
        new Thread(() => {
            ProcessStartInfo psi = new ProcessStartInfo(command);
            psi.Arguments = argument;
            psi.CreateNoWindow = true;
            psi.ErrorDialog = true;
            psi.UseShellExecute = true; 
            psi.RedirectStandardOutput = false;
            psi.RedirectStandardError = false;
            psi.RedirectStandardInput = false;
            Process p = Process.Start(psi); 
            p.WaitForExit();
            p.Close();
        }).Start();
    }

    public static string Join(params string[] paths) {
        return Path.GetFullPath(Path.Combine(paths)).Replace("\\", "/");
    }

    public static string Obj2Json(object obj, bool isPretty = true) {
        LitJson.JsonWriter writer = new LitJson.JsonWriter();
        writer.PrettyPrint = isPretty;
        LitJson.JsonMapper.ToJson(obj, writer);
        return writer.TextWriter.ToString();
    }

    public static T Json2Obj<T>(string content) {
        return LitJson.JsonMapper.ToObject<T>(content);
    }

    public static void WriteObj2Json(string path, object obj, bool isPretty = true) {
        LitJson.JsonWriter writer = new LitJson.JsonWriter();
        writer.PrettyPrint = isPretty;
        LitJson.JsonMapper.ToJson(obj, writer);
        Utils.WriteFileUTF8(path, Obj2Json(obj, isPretty));
    }

    public static void WriteObj2UnicodeJson(string path, object obj, bool isPretty = true) {
        LitJson.JsonWriter writer = new LitJson.JsonWriter();
        writer.PrettyPrint = isPretty;
        LitJson.JsonMapper.ToJson(obj, writer);
        Utils.WriteFileUTF8(path, ConvUnicode(Obj2Json(obj, isPretty)));
    }

    public static T ReadJson<T>(string path) {
        Assert(File.Exists(path), "--- no file path: " + path);
        byte[] content = Utils.ReadAllBytesFromFile(path);
        string str = System.Text.Encoding.UTF8.GetString(content);
        return EditorUtils.Json2Obj<T>(str);
    }

    public static T ReadJsonEncrypt<T>(string path) {
        Assert(File.Exists(path), "--- no file path: " + path);
        string str = Utils.ReadFileEncrypt(path);
        return EditorUtils.Json2Obj<T>(str);
    }

    public static void WriteJsonEncrypt(string path, object obj) {
        string json = EditorUtils.Obj2Json(obj);
        Utils.WriteFileEncrypt(path, json);
    }

    public static string beautyJson(object obj, bool isPretty = true) {
        LitJson.JsonWriter writer = new LitJson.JsonWriter();
        writer.PrettyPrint = isPretty;
        LitJson.JsonMapper.ToJson(obj, writer);
        return writer.TextWriter.ToString();
    }

    public static string beautyJson(string str, bool isPretty = true) {
        object obj = EditorUtils.Json2Obj<object>(str);
        return beautyJson(obj, isPretty);
    }

    public static string ConvSlashToUnicodeSlash(string txt) {
        return txt.Replace('/', '\u2215');
    }

    public static string ConvUnicodeSlashToSlash(string txt) {
        return txt.Replace('\u2215', '/');
    }

    public static int[] GetImagePixels(string path) {
        UnityEngine.Object obj = AssetImporter.GetAtPath(path);
        TextureImporter importer = obj as TextureImporter;
        EditorUtils.Assert(importer != null, string.Format("检测到非图片资源, path:{0}", path));
        return GetImagePixels(importer);
    }

    public static int[] GetImagePixels(TextureImporter importer) {
        object[] args = new object[2] { 0, 0 };
        MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
        mi.Invoke(importer, args);
        return new int[2] {
            (int) args[0], (int) args[1]
        };
    }

    public static Texture2D ReadTex2D(string path) {
        WWW w = new WWW(path);
        while (!w.isDone) { Thread.Sleep(10); }
        return w.texture;
    }

    // 将 Unicode 转成中文, 解决 litjson 中文问题
    public static string ConvUnicode(string txt) {
        Regex reg = new Regex(@"(?i)\\[uU]([0-9a-f]{4})");
        return reg.Replace(txt, delegate(Match m) { return ((char) Convert.ToInt32(m.Groups[1].Value, 16)).ToString(); });
    }

    public static string[] GetFiles(string dstDir, string pattern) {
        Regex reg = new Regex(pattern);
        var fileArr = Directory.GetFiles(dstDir, "*", SearchOption.AllDirectories).Where(path => reg.IsMatch(path)).ToList();
        int cnt = fileArr.Count;
        string[] retArr = new string[cnt];
        for (int i = 0; i < cnt; i++) {
            retArr[i] = fileArr[i].Replace("\\", "/");
        }
        return retArr;
    }

    private const string RND_NUM = "0123456789";
    private const string RND_CHAR_UP = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string RND_CHAR_LOW = "abcdefghijklmnopqrstuvwxyz";
    private const string RND_SPEC = "~!@#$%^&*()+=?";
    public static string RandomStr(int dstLen = 15, bool hasNum = true, bool hasCharUp = true, bool hasCharLow = true, bool hasSpec = true) {
        string combine = "";
        if (hasNum) combine += RND_NUM;
        if (hasCharUp) combine += RND_CHAR_UP;
        if (hasCharLow) combine += RND_CHAR_LOW;
        if (hasSpec) combine += RND_SPEC;
        string rdnStr = combine.Shuffle();
        return rdnStr.Substring(0, Math.Min(dstLen, rdnStr.Length));
    }

    public static string Now(string fmt = "yyyyMMddTHHmmss") {
        return System.DateTime.Now.ToString(fmt);
    }

    public static string GetDesktop(params string[] paths) {
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        if (paths == null || paths.Length == 0) {
            return Join(desktop);
        } else {
            string relaPath = Path.Combine(paths);
            return Join(desktop, relaPath);
        }
    }

    // 多线程执行, 并等到返回
    public static void MultiThreadExec(ParameterizedThreadStart[] fnArr) {
        Thread[] thArr = new Thread[fnArr.Length];
        for (int i = 0; i < fnArr.Length; i++) {
            Thread t = new Thread(fnArr[i]);
            thArr[i] = t;
            t.Start();
        }
        foreach (var t in thArr) {
            t.Join();
        }
    }

    // 文件锁
    public static void FileLockFunc(string lockFile, Action fn, int msTimeout = 1000) {
        while (Utils.IsFileExist(lockFile)) {
            UnityEngine.Debug.LogFormat("--- locing, file: {0}", lockFile);
            Thread.Sleep(msTimeout);
        }

        Utils.WriteFileUTF8(lockFile, "");
        Exception ex = null;
        try {
            fn();
        } catch (Exception e) {
            ex = e;
        } finally {
            Utils.DeleteFile(lockFile);
            if (ex != null)
                throw ex;
        }
    }

    // 批量替换
    public static string ReplaceMulti(string txt, Dictionary<string, string> replaceDict) {
        Regex regex = new Regex(String.Join("|", replaceDict.Keys));
        return regex.Replace(txt, m => replaceDict[m.Value]);
    }

    // 删除匹配文件
    public static void RemoveFile(string dir, string matchRegex, string keepRegex) {
        Regex rgx = new Regex(keepRegex);
        string[] fileArr = EditorUtils.GetFiles(dir, matchRegex);
        // UnityEngine.Debug.LogFormat("--- all file: {0}", EditorUtils.beautyJson(fileArr));
        foreach (var file in fileArr) {
            if (!rgx.IsMatch(file)) {
                // UnityEngine.Debug.LogFormat("--- delete file: {0}", file);
                Utils.DeleteFile(file);
            }
        }
    }

    // 获取枚举数组
    public static string[] GetEnumArr<T>() {
        return Enum.GetValues(typeof(T)).Cast<int>().ToList().ConvertAll<string>(delegate(int i) { return i.ToString(); }).ToArray();
    }

    // 获取枚举的 索引值
    public static int GetEnumIndex<T>(int value) {
        string valStr = value.ToString();
        string[] arr = GetEnumArr<T>();
        for (int i = 0; i < arr.Length; i++) {
            if (arr[i] == valStr) {
                return i;
            }
        }
        return -1;
    }

    public static string ToTitle(string txt) {
        return txt.Substring(0, 1).ToUpper() + txt.Substring(1).ToLower();
    }
}

// [InitializeOnLoad]
// public class EditorStartup {
//     static EditorStartup() {
//         EditorUtils.ClearConsole();
//     }
// }