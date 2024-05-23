using System;
using System.IO;
using System.Reflection.Emit;
using UnityEngine;
using UnityEditor;

public class UltimateJsonEditor
{
	private const string UnityName= "/Unity.exe";
	private const string SystemDataDllName = "System.Data.dll";
	private const string SystemDataPath = "/Data/Mono/lib/mono/2.0/" + SystemDataDllName;

	[MenuItem("Edit/UltimateJson/Copy Dll To Project")]
	public static void GetDataDll()
	{
		var assetFolder = Application.dataPath + "/";
		var systemDataAssetPath = assetFolder + SystemDataDllName;

		var info = new FileInfo(systemDataAssetPath);
		if (info == null || info.Exists == false)
		{
			Debug.Log(systemDataAssetPath + " is not Exists");
			CopyFile(systemDataAssetPath);
		}
	}

	private static void CopyFile(string assetFolder)
	{
		var appPath = EditorApplication.applicationPath;
		var startIndexUnity = appPath.IndexOf(UnityName, StringComparison.Ordinal);
		var unityEditorPath = appPath.Remove(startIndexUnity, UnityName.Length);

		var systemDataFullPath = unityEditorPath + SystemDataPath;

		FileUtil.CopyFileOrDirectory(systemDataFullPath, assetFolder);
		AssetDatabase.Refresh();
	}
}
