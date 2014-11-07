using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

// This class is used to help user configuring different build platform when using Badumna library.
public class BadumnaBuildTargets : EditorWindow
{
    private const string ConfigFileName = "HelperConfig.config";

    private static string dllParentDirPath;
    private static string badumnaAndroidPath;
    private static string badumnaIOSPath;
    private static string badumnaDekstopPath;

    [MenuItem("Badumna/Build Targets")]
    static void Init()
    {
        var window = EditorWindow.GetWindow<BadumnaBuildTargets>(false, "Build Targets");

        // resetting the position of the helper window
        if (window != null)
        {
            window.Focus();
            window.position = new Rect(40, 40, 475, 130);
            LoadConfig();
        }
    }

    void OnGUI()
    {
        // Assemblies location
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Assemblies directory location: ", GUILayout.MaxWidth(200));
        dllParentDirPath = EditorGUILayout.TextField(dllParentDirPath);
        if (GUILayout.Button("Browse", new GUILayoutOption[] { GUILayout.Width(55) }))
        {
            var path = EditorUtility.OpenFolderPanel(name, dllParentDirPath, "");
            this.SearchDllLocation(path);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Bundle Identifier:", GUILayout.MaxWidth(200));
        PlayerSettings.bundleIdentifier = EditorGUILayout.TextField(PlayerSettings.bundleIdentifier);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Configure Android Build"))
        {
            this.ConfigureBuild("Android");
        }
        if (GUILayout.Button("Configure iOS Build"))
        {
            this.ConfigureBuild("iOS");
        }
        if (GUILayout.Button("Configure Desktop Build"))
        {
            this.ConfigureBuild("Desktop");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        GUILayout.Label(!string.IsNullOrEmpty(badumnaAndroidPath) ? "- Badumna.Android.dll found" : "- Badumna.Android.dll not found", GUILayout.MaxWidth(200));
        GUILayout.Label(!string.IsNullOrEmpty(badumnaIOSPath) ? "- Badumna.Unity.iOS.dll found" : "- Badumna.Unity.iOS.dll not found", GUILayout.MaxWidth(200));
        GUILayout.Label(!string.IsNullOrEmpty(badumnaDekstopPath) ? "- Badumna.dll found" : "- Badumna.dll not found", GUILayout.MaxWidth(200));
    }

    void OnFocus()
    {
        LoadConfig();
    }

    private static void SaveConfig()
    {
        var configFile = Path.Combine(".", ConfigFileName);
        File.WriteAllText(
            configFile,
            string.Format(
                "{0};{1};{2};{3}",
                dllParentDirPath,
                badumnaAndroidPath,
                badumnaIOSPath,
                badumnaDekstopPath
        ));
    }

    private static void LoadConfig()
    {
        var configFile = Path.Combine(".", ConfigFileName);
        if (File.Exists(configFile))
        {
            var content = File.ReadAllText(configFile).Split(';');
            dllParentDirPath = content[0];
            badumnaAndroidPath = content[1];
            badumnaIOSPath = content[2];
            badumnaDekstopPath = content[3];
        }
    }

    private void SearchDllLocation(string path)
    {
        if (string.IsNullOrEmpty(path))
            return;

        dllParentDirPath = path;

        // clear the existing dll paths
        badumnaAndroidPath = badumnaDekstopPath = badumnaIOSPath = string.Empty;
        try
        {
            foreach (string dir in Directory.GetDirectories(dllParentDirPath))
            {
                foreach (string file in Directory.GetFiles(dir))
                {
                    if (Path.GetExtension(file).Equals(".dll"))
                    {
                        switch (Path.GetFileName(file))
                        {
                            case "Badumna.Android.dll":
                                badumnaAndroidPath = file;
                                break;
                            case "Badumna.Unity.iOS.dll":
                                badumnaIOSPath = file;
                                break;
                            case "Badumna.dll":
                                badumnaDekstopPath = file;
                                break;
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        SaveConfig();
    }

    private void ConfigureBuild(string platform)
    {
        this.DeleteExistingDllFiles();

        switch (platform)
        {
            case "Android":
                CopyDllFile(badumnaAndroidPath);
                PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0_Subset;
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
                break;
            case "iOS":
                CopyDllFile(badumnaIOSPath);
                PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0_Subset;
                PlayerSettings.aotOptions = "nimt-trampolines=4096";
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iPhone);
                break;
            case "Desktop":
                CopyDllFile(badumnaDekstopPath);
                PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0;
                PlayerSettings.runInBackground = true;
                switch (Application.platform)
                {
                    case RuntimePlatform.WindowsEditor:
                        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows64);
                        break;
                    case RuntimePlatform.OSXEditor:
                        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneOSXIntel);
                        break;
                }
                break;
        }
    }

    private void CopyDllFile(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            this.Trace("error", "Path cannot be null or empty");
        }
        if (File.Exists(path))
        {
            this.Trace("info", path + " exist. Copying file now");
            File.Copy(path, Path.Combine("./Assets/Assemblies/", Path.GetFileName(path)));
        }
        else
        {
            this.Trace("error", "Error copying dll, please check Assemblies directory location, and try again.");
        }
    }

    private void Trace(string type, string message)
    {
        var m = string.Format("{0} : {1}\n", DateTime.Now.ToLocalTime(), message);
        switch (type)
        {
            case "error":
                Debug.LogError(m);
                break;
            case "info":
                Debug.Log(m);
                break;
        }
    }

    private void DeleteExistingDllFiles()
    {
        // Delete the existing dlls
        string[] filePaths = Directory.GetFiles("./Assets/Assemblies");

        foreach (var file in filePaths)
        {
            if (Path.GetExtension(file).Equals(".dll")
                || Path.GetExtension(file).Equals(".meta"))
            {
                this.Trace("info", "Removing " + file);
                File.Delete(file);
            }
        }
    }
}
