using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class AESUtil {
    static RijndaelManaged aes256 = new RijndaelManaged();

    public static string EncryptStr(string data, ref string key) {
        byte[] bs = Encoding.UTF8.GetBytes(data);
        return EncryptByte(bs, ref key);
    }

    public static string DecryptStr(string data, ref string key) {
        byte[] bs = Convert.FromBase64String(data);
        return DecryptByte(bs, ref key);
    }

    public static string EncryptByte(byte[] bs, ref string key) {
        byte[] result = EncryptByte2Byte(bs, ref key);
        return result != null ? Convert.ToBase64String(result) : null;
    }

    public static string DecryptByte(byte[] bs, ref string key) {
        byte[] result = DecryptByte2Byte(bs, ref key);
        return result != null ? Encoding.UTF8.GetString(result) : null;
    }

    public static byte[] EncryptByte2Byte(byte[] bs, ref string key) {
        if (key == null || key.Length != 32) {
            LogUtil.E("EncryptByte2Byte Error!, key size != 32");
            return null;
        }

        aes256.Key = Encoding.UTF8.GetBytes(key); // key 长度必须为 32字节 字符串
        aes256.Mode = CipherMode.ECB;
        aes256.Padding = PaddingMode.PKCS7;

        byte[] result = null;
        try {
            result = aes256.CreateEncryptor().TransformFinalBlock(bs, 0, bs.Length);
        } catch (Exception) {
            LogUtil.E("EncryptByte2Byte Error!");
        }
        return result;
    }

    public static byte[] DecryptByte2Byte(byte[] bs, ref string key) {
        if (key == null || key.Length != 32) {
            LogUtil.E("DecryptByte2Byte Error!, key size != 32");
            return null;
        }

        aes256.Key = Encoding.UTF8.GetBytes(key); // key 长度必须为 32字节 字符串
        aes256.Mode = CipherMode.ECB;
        aes256.Padding = PaddingMode.PKCS7;

        byte[] result = null;
        try {
            result = aes256.CreateDecryptor().TransformFinalBlock(bs, 0, bs.Length);
        } catch (Exception) {
            LogUtil.E("DecryptByte2Byte Error!");
        }
        return result;
    }

    // -------------- aesEncryptBase64
    public static string aesEncryptBase64(string SrcStr, string CryptoKey) {
        byte[] srcBts = Encoding.UTF8.GetBytes(SrcStr);
        byte[] dstBts = aesEncryptBase64(srcBts, CryptoKey);
        return dstBts != null ? Encoding.UTF8.GetString(dstBts) : "";
    }

    public static string aesDecryptBase64(string SrcStr, string CryptoKey) {
        byte[] srcBts = Encoding.UTF8.GetBytes(SrcStr);
        byte[] dstBts = aesDecryptBase64(srcBts, CryptoKey);
        return dstBts != null ? Encoding.UTF8.GetString(dstBts) : "";
    }

    public static byte[] aesEncryptBase64(byte[] dataByteArray, string CryptoKey) {
        byte[] encrypt = null;
        try {
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
            byte[] key = sha256.ComputeHash(Encoding.UTF8.GetBytes(CryptoKey));
            byte[] iv = md5.ComputeHash(Encoding.UTF8.GetBytes(CryptoKey));
            aes.Key = key;
            aes.IV = iv;

            using(MemoryStream ms = new MemoryStream()) {
                using(CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write)) {
                    cs.Write(dataByteArray, 0, dataByteArray.Length);
                    cs.FlushFinalBlock();
                    encrypt = ms.ToArray();
                }
            }
        } catch (Exception e) {
            LogUtil.D("aesEncryptBase64 Error!, msg: {0}", e.Message);
        }
        return encrypt;
    }

    public static byte[] aesDecryptBase64(byte[] dataByteArray, string CryptoKey) {
        byte[] decrypt = null;
        try {
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
            byte[] key = sha256.ComputeHash(Encoding.UTF8.GetBytes(CryptoKey));
            byte[] iv = md5.ComputeHash(Encoding.UTF8.GetBytes(CryptoKey));
            aes.Key = key;
            aes.IV = iv;

            using(MemoryStream ms = new MemoryStream()) {
                using(CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write)) {
                    cs.Write(dataByteArray, 0, dataByteArray.Length);
                    cs.FlushFinalBlock();
                    decrypt = ms.ToArray();
                }
            }
        } catch (Exception e) {
            LogUtil.D("aesDecryptBase64 Error!, msg: {0}", e.Message);
        }
        return decrypt;
    }

    // -------------- 自定义加密算法
    public static byte[] PackXor(byte[] data, ref string pstr) {
        int length = data.Length;
        int strCount = 0;
        int pLen = pstr.Length % 8;
        for (int i = 0; i < length; ++i) {
            if (strCount >= pLen)
                strCount = 0;
            data[i] ^= (byte) pstr[strCount++];
        }
        return data;
    }
}