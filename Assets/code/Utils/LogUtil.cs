using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public enum ELogLevel {
    Debug = 1,
    Warn = 2,
    Error = 3,
    None = 4,
}

// 运行时日志工具, 不要使用 在 editor 模式下
public static class LogUtil {

    public static Action<string, string> ErrorHandler = null;

    private static ELogLevel logLevel = ELogLevel.Error;

    private static string Now {
        get { return string.Format("{0} | ", System.DateTime.Now.ToString("HH:mm:ss.fff")); }
    }

    public static void SetLv(int lv) {
        logLevel = (ELogLevel) lv;
    }

    public static void D(string fmt, params object[] args) {
        if (ELogLevel.Debug < logLevel) return;
        UnityEngine.Debug.LogFormat(Now + fmt, args);
    }

    public static void D(string fmt) {
        if (ELogLevel.Debug < logLevel) return;
        UnityEngine.Debug.Log(Now + fmt);
    }

    public static void W(string fmt, params object[] args) {
        if (ELogLevel.Warn < logLevel) return;
        UnityEngine.Debug.LogWarningFormat(Now + fmt, args);
    }

    public static void W(string fmt) {
        if (ELogLevel.Warn < logLevel) return;
        UnityEngine.Debug.LogWarning(Now + fmt);
    }

    public static void E(string fmt, params object[] args) {
        if (ELogLevel.Error < logLevel) return;
        if (ErrorHandler != null) {
            ErrorHandler(Now + Utils.StrFormat(fmt, args), StackTraceUtility.ExtractStackTrace());
        } else {
            UnityEngine.Debug.LogErrorFormat(Now + fmt, args);
        }
    }

    public static void E(string fmt) {
        if (ELogLevel.Error < logLevel) return;
        if (ErrorHandler != null) {
            ErrorHandler(Now + fmt, StackTraceUtility.ExtractStackTrace());
        } else {
            UnityEngine.Debug.LogError(Now + fmt);
        }
    }

    public static void A(bool cond, string fmt) {
        if (ELogLevel.Error < logLevel) return;
        if (!cond) {
            E(fmt);
        }
    }

    public static void A(bool cond, string fmt, params object[] args) {
        if (ELogLevel.Error < logLevel) return;
        if (!cond) {
            E(fmt, args);
        }
    }
}