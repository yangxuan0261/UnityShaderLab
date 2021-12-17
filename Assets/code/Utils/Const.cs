using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Const {

	public static bool UseAssetBundle = false; // 是否使用 ab 模式
	public static bool LuaBundleMode = true; // 是否使用 lua bundle 模式
	public const string kAesKey = "U5RNM4beTo%@QmA";

	public const string AssetListFileName = "asset_list.json";
	public const string kResArtDir = "resource";
	public const string kResluaDir = "lua";
	public const string kApkName = "rmg.apk";
	public const string kResCfgDir = "dataconfig";
	public const string kDbName = "rummy.db";
	public const string kPackName = "pack.db";
	public const string kPatchName = "patch.db";
	public const string kMyCache = "mycache";

	public const string ApiVersion = "0.0.1";
}