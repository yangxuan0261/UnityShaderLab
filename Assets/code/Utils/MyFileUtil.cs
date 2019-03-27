using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MyFileUtil {

    // public static void CopyDir(string sourceDir, string destDir, string ext = "", string searchPattern = "*.lua", string encryptKey = "", SearchOption option = SearchOption.AllDirectories) {
    //     if (!Directory.Exists(sourceDir)) {
    //         Debug.LogErrorFormat("--- CopyDir 不存在路径, sourceDir:{0}", sourceDir);
    //         return;
    //     }

    //     string[] files = Directory.GetFiles(sourceDir, searchPattern, option);
    //     int len = sourceDir.Length;

    //     if (sourceDir[len - 1] == '/' || sourceDir[len - 1] == '\\') {
    //         --len;
    //     }

    //     for (int i = 0; i < files.Length; i++) {
    //         string str = files[i].Remove(0, len);
    //         string dest = destDir + "/" + str;
    //         if (ext != "") {
    //             dest += ext;
    //         }
    //         string dir = Path.GetDirectoryName(dest);
    //         Directory.CreateDirectory(dir);

    //         if (encryptKey != "") { // lua 加密模式
    //             byte[] content = File.ReadAllBytes(files[i]);
    //             content = AESUtil.PackXor(content, ref encryptKey);
    //             Utils.WriteFile(dest, content);
    //         } else {
    //             File.Copy(files[i], dest, true);
    //         }
    //     }
    // }

    public static string GetFullPath(string assetPath) {
        return Path.Combine(Application.dataPath.Replace("Assets",""), assetPath).Replace("\\", "/");
    }

    
    public static string GetAssetPath(string fullPath) {
        fullPath = fullPath.Replace("\\", "/");
        int pos = fullPath.IndexOf("Assets");
        if (pos != -1) {
            return fullPath.Substring(pos);
        } else {
            return fullPath;
        }
    }

    public static List<string> GetResourcePath(string path, string resourceType, bool isSensitiveCase = false, SearchOption so = SearchOption.AllDirectories, bool isAssertsPath = true, bool isDir = false)
    {
        List<string> resourceList = new List<string>();
        if (Directory.Exists(path))
        {
            string[] matPaths = null;
            if (isDir) {
                matPaths = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
            } else {
                matPaths = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            }
            matPaths = matPaths.Where((string s) => {
                if (isSensitiveCase) {
                    return s.EndsWith(resourceType);
                } else {
                    return s.ToLower().EndsWith(resourceType.ToLower());
                }
                }).ToArray();
            resourceList.AddRange(matPaths);
        } else if (path.EndsWith(resourceType)) {
            resourceList.Add(path);
        }

        for (int i = 0; i < resourceList.Count; i++) {
            string fullPath = resourceList[i].Replace("\\", "/");
            resourceList[i] = GetAssetPath(fullPath);
        }
        return resourceList;
    }

    public static string GetDirBaseName(string path) {
        string dirName = Path.GetDirectoryName(path).Replace("\\", "/");
        int pos = dirName.LastIndexOf("/");
        if (pos != -1) {
            return dirName.Substring(pos + 1);
        } else {
            return dirName;
        }
    }

}
