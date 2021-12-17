using System;
using System.IO;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using UnityEngine;

public class ZipHelper {

    static ZipHelper() {
        // 设置编码, 不然会报错: Encoding 437 data could not be found while unzipping
        // 参考: https://answers.unity.com/questions/1720968/encoding-437-data-could-not-be-found-while-unzippi.html
        ZipConstants.DefaultCodePage = System.Text.Encoding.UTF8.CodePage;
    }
    /// 压缩单个文件

    /// 要压缩的文件
    /// 压缩后的文件
    /// 压缩等级
    /// 每次写入大小
    public static void ZipFile(string fileToZip, string zipedFile, int compressionLevel, int blockSize) {
        //如果文件没有找到，则报错
        fileToZip = fileToZip.Replace("\\", "/");
        zipedFile = zipedFile.Replace("\\", "/");
        if (!System.IO.File.Exists(fileToZip)) {
            throw new System.IO.FileNotFoundException("指定要压缩的文件: " + fileToZip + " 不存在!");
        }

        using(System.IO.FileStream ZipFile = System.IO.File.Create(zipedFile)) {
            using(ZipOutputStream ZipStream = new ZipOutputStream(ZipFile)) {
                using(System.IO.FileStream StreamToZip = new System.IO.FileStream(fileToZip, System.IO.FileMode.Open, System.IO.FileAccess.Read)) {
                    string fileName = fileToZip.Substring(fileToZip.LastIndexOf("/") + 1);
                    ZipEntry ZipEntry = new ZipEntry(fileName);
                    ZipStream.PutNextEntry(ZipEntry);
                    ZipStream.SetLevel(compressionLevel);
                    byte[] buffer = new byte[blockSize];
                    int sizeRead = 0;
                    try {
                        do {
                            sizeRead = StreamToZip.Read(buffer, 0, buffer.Length);
                            ZipStream.Write(buffer, 0, sizeRead);
                        }
                        while (sizeRead > 0);
                    } catch (System.Exception ex) {
                        throw ex;
                    }
                    StreamToZip.Close();
                }
                ZipStream.Finish();
                ZipStream.Close();
            }
            ZipFile.Close();
        }
    }

    /// 压缩单个文件

    /// 要进行压缩的文件名
    /// 压缩后生成的压缩文件名
    public static void ZipFile(string fileToZip, string zipedFile) {
        ZipConstants.DefaultCodePage = System.Text.Encoding.UTF8.CodePage;

        //如果文件没有找到，则报错
        fileToZip = fileToZip.Replace("\\", "/");
        zipedFile = zipedFile.Replace("\\", "/");
        if (!File.Exists(fileToZip)) {
            throw new System.IO.FileNotFoundException("指定要压缩的文件: " + fileToZip + " 不存在!");
        }

        using(FileStream fs = File.OpenRead(fileToZip)) {
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();

            using(FileStream ZipFile = File.Create(zipedFile)) {
                using(ZipOutputStream ZipStream = new ZipOutputStream(ZipFile)) {
                    string fileName = fileToZip.Substring(fileToZip.LastIndexOf("/") + 1);
                    ZipEntry ZipEntry = new ZipEntry(fileName);
                    ZipStream.PutNextEntry(ZipEntry);
                    ZipStream.SetLevel(5);

                    ZipStream.Write(buffer, 0, buffer.Length);
                    ZipStream.Finish();
                    ZipStream.Close();
                }
            }
        }
    }

    /// 压缩多层目录

    /// The directory.
    /// The ziped file.
    public static void ZipFileDirectory(string strDirectory, string zipedFile) {
        using(System.IO.FileStream ZipFile = System.IO.File.Create(zipedFile)) {
            using(ZipOutputStream s = new ZipOutputStream(ZipFile)) {
                ZipSetp(strDirectory, s, "");
            }
        }
    }

    /// 递归遍历目录

    /// The directory.
    /// The ZipOutputStream Object.
    /// The parent path.
    private static void ZipSetp(string strDirectory, ZipOutputStream s, string parentPath) {
        strDirectory = strDirectory.Replace("\\", "/");
        parentPath = parentPath.Replace("\\", "/");
        if (strDirectory[strDirectory.Length - 1] != Path.DirectorySeparatorChar) {
            strDirectory += Path.DirectorySeparatorChar;
        }
        Crc32 crc = new Crc32();

        string[] filenames = Directory.GetFileSystemEntries(strDirectory);
        foreach (string filetmp in filenames) // 遍历所有的文件和目录
        {
            string file = filetmp.Replace("\\", "/");
            if (Directory.Exists(file)) // 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
            {
                string pPath = parentPath;
                pPath += file.Substring(file.LastIndexOf("/") + 1);
                pPath += "/";
                ZipSetp(file, s, pPath);
            } else // 否则直接压缩文件
            {
                //打开压缩文件
                using(FileStream fs = File.OpenRead(file)) {
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);

                    string fileName = parentPath + file.Substring(file.LastIndexOf("/") + 1);
                    ZipEntry entry = new ZipEntry(fileName);

                    entry.DateTime = DateTime.Now;
                    entry.Size = fs.Length;

                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);

                    entry.Crc = crc.Value;
                    s.PutNextEntry(entry);
                    s.Write(buffer, 0, buffer.Length);
                }
            }
        }
    }

    /// 解压缩一个 zip 文件。

    /// The ziped file.
    /// The STR directory.
    /// zip 文件的密码。
    /// 是否覆盖已存在的文件。
    public static bool UnZip(string zipedFile, string strDirectory, string password, bool overWrite) {
        zipedFile = zipedFile.Replace("\\", "/");
        strDirectory = strDirectory.Replace("\\", "/");

        if (strDirectory == "")
            strDirectory = Directory.GetCurrentDirectory();
        if (!strDirectory.EndsWith("/"))
            strDirectory = strDirectory + "/";
        try {
            using(ZipInputStream s = new ZipInputStream(File.OpenRead(zipedFile))) {
                s.Password = password;
                ZipEntry theEntry;

                while ((theEntry = s.GetNextEntry()) != null) {
                    string directoryName = "";
                    string pathToZip = "";
                    pathToZip = theEntry.Name;

                    if (pathToZip != "")
                        directoryName = Path.GetDirectoryName(pathToZip) + "/";

                    string fileName = Path.GetFileName(pathToZip);
                    Directory.CreateDirectory(strDirectory + directoryName);
                    if (fileName != "") {
                        if ((File.Exists(strDirectory + directoryName + fileName) && overWrite) || (!File.Exists(strDirectory + directoryName + fileName))) {
                            using(FileStream streamWriter = File.Create(strDirectory + directoryName + fileName)) {
                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true) {
                                    size = s.Read(data, 0, data.Length);

                                    if (size > 0)
                                        streamWriter.Write(data, 0, size);
                                    else
                                        break;
                                }
                                streamWriter.Close();
                            }
                        }
                    }
                }
                s.Close();
                return true;
            }
        } catch (Exception ex) {
            LogUtil.E("解压文件{0}失败:{1}", zipedFile, ex);
            return false;
        }
    }

}