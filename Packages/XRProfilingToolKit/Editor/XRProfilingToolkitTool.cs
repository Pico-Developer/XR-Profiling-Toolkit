/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEditor.PackageManager;
using System.Linq;
using UnityEditor.PackageManager.Requests;
using UnityEditor.XR.Management;
using UnityEngine.XR.Management;
using XRProfilingToolkitTool.Editor;

namespace XRProfilingToolkit.Editor
{
    /// <summary>
    /// Editor menu that provides useful shortcuts for setting up the environments and using the test tools.
    /// </summary>
    public class XRProfilingToolkitEditorMenu : EditorWindow
    {
        [MenuItem("XR Profiling ToolKit/Shortcuts/Validate Provider Plugin (Meta\\PICO)")]
        private static void CheckXRPlugin()
        {
            string currentMacros = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            string picoMacro = "PICO_PERFORMANCE";
            string oculusMacro = "OCULUS_PERFORMANCE";

            var request = Client.List();
            while (!request.IsCompleted) { }
            var packages = request.Result;
            bool isMetaSDKInstalled = packages.Any(p => p.name.Contains("com.unity.xr.oculus"));
            bool isPicoSDKInstalled = packages.Any(p => p.name.Contains("com.unity.xr.picoxr"));

            if (isPicoSDKInstalled)
            {
                if (!currentMacros.Contains(picoMacro))
                {
                    currentMacros += ";" + picoMacro;
                }
                // Update DefineSymbols
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, currentMacros);
                
                if (currentMacros.Contains(picoMacro) &&!currentMacros.Contains(oculusMacro))
                {
                    UnityEngine.Debug.Log("Successfully added PICOPerformance DefineSymbol");
                }
            }
            else
            {
                if (currentMacros.Contains(picoMacro))
                {
                    currentMacros = currentMacros.Replace(picoMacro + ";", "");
                    currentMacros = currentMacros.Replace(";" + picoMacro, "");
                    currentMacros = currentMacros.TrimEnd(';');
                }
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, currentMacros);

            }
            if (isMetaSDKInstalled)
            {
                if (!currentMacros.Contains(oculusMacro))
                {
                    currentMacros += ";" + oculusMacro;
                }
                
                // Update DefineSymbols
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, currentMacros);
                
                if (currentMacros.Contains(oculusMacro))
                {
                    UnityEngine.Debug.Log("Successfully added OculusPerformance DefineSymbol.");
                }
            }
            else
            {
                if (currentMacros.Contains(oculusMacro))
                {
                    currentMacros = currentMacros.Replace(oculusMacro + ";", "");
                    currentMacros = currentMacros.Replace(";" + oculusMacro, "");
                    currentMacros = currentMacros.TrimEnd(';');
                }
                // Update DefineSymbols
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, currentMacros);
            }
        }
        private static string _prefabPackagePath;
        private static GameObject _prefabToCheck;
        
        [MenuItem("XR Profiling ToolKit/Shortcuts/Add CommandRunner To Scene")]
        static void AddCommandRunnerToScene()
        {
            _prefabPackagePath = "Packages/com.pico.xr.profiling.toolkit/Resources/CommandRunner.prefab";
            _prefabToCheck = AssetDatabase.LoadAssetAtPath<GameObject>(_prefabPackagePath);
            if (_prefabToCheck == null)
            {
                UnityEngine.Debug.LogError("Missing prefab error: please try reimporting the package.");
            }
            bool prefabExists = CheckPrefabInScene(_prefabToCheck);

            if (!prefabExists)
            {
                AddPrefabToScene(_prefabToCheck);
                UnityEngine.Debug.Log("Successfully added \"CommandRunner\" to the scene.");
            }
        }
        
        [MenuItem("XR Profiling ToolKit/Shortcuts/Add SceneLoader To Scene")]
        static void AddSceneLoaderToScene()
        {
            _prefabPackagePath = "Packages/com.pico.xr.profiling.toolkit/Resources/SceneLoader.prefab";
            _prefabToCheck = AssetDatabase.LoadAssetAtPath<GameObject>(_prefabPackagePath);
            if (_prefabToCheck == null)
            {
                UnityEngine.Debug.LogError("Missing prefab error: please try reimporting the package.");
            }
            bool prefabExists = CheckPrefabInScene(_prefabToCheck);

            if (!prefabExists)
            {
                AddPrefabToScene(_prefabToCheck);
                UnityEngine.Debug.Log("Successfully added \"SceneLoader\" to the scene.");
            }
        }
        
        [MenuItem("XR Profiling ToolKit/Shortcuts/Add FeatureManager To Scene")]
        static void AddFeatureManagerToScene()
        {
            _prefabPackagePath = "Packages/com.pico.xr.profiling.toolkit/Resources/FeatureBaseManager.prefab";
            _prefabToCheck = AssetDatabase.LoadAssetAtPath<GameObject>(_prefabPackagePath);
            if (_prefabToCheck == null)
            {
                UnityEngine.Debug.LogError("Missing prefab error: please try reimporting the package.");
            }
            bool prefabExists = CheckPrefabInScene(_prefabToCheck);

            if (!prefabExists)
            {
                AddPrefabToScene(_prefabToCheck);
                UnityEngine.Debug.Log("Successfully added \"FeatureBaseManager\" to the scene.");
            }
        }
        
        
        [MenuItem("XR Profiling ToolKit/Shortcuts/Copy ProfilingToolScripts folder To Assets")]
        static void CopyScriptFolderToAssets()
        {
            CopyPackageFolder();
        }
        static void CopyPackageFolder()
        {
            string sourceFolderPath = "Packages/com.pico.xr.profiling.toolkit/Editor/ProfilingToolScripts/";
            string targetFolderPath = Application.dataPath+"/XRProfilingToolKit/Editor/ProfilingToolScripts/";
            
            // Handle directory paths according to Mac/Windows to make sure having the correct directory separator character.
            sourceFolderPath = sourceFolderPath.Replace('/', Path.DirectorySeparatorChar);
            targetFolderPath = targetFolderPath.Replace('/', Path.DirectorySeparatorChar);

            // Create target folder if not exists
            if (!Directory.Exists(targetFolderPath))
            {
                Directory.CreateDirectory(targetFolderPath);
                //FileUtil.CopyFileOrDirectory(sourceFolderPath, targetFolderPath);
                
                // Get all the GUIDs of files and folders under the source folder
                string[] guids = AssetDatabase.FindAssets("", new string[] { sourceFolderPath });
                foreach (string guid in guids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    string relativePath = assetPath.Replace(sourceFolderPath.Replace('\\', '/'), "");
                    string targetPath = Path.Combine(targetFolderPath, relativePath);
                    
                    // Check if it is a file or folder
                    if (Directory.Exists(assetPath))
                    {
                        CopyDirectoryRecursive(assetPath, targetPath);
                    }
                    else
                    {
                        if(assetPath.Contains(".meta"))
                            continue;
                        File.Copy(assetPath, targetPath, true);
                    }
                }

                AssetDatabase.Refresh();
                UnityEngine.Debug.Log("Successfully copied the ProfilingToolScripts folder from Package to Assets.");
            }
            else
            {
                UnityEngine.Debug.Log("ProfilingToolScripts folder already exists in the project's Assets folder.");
            }
        }
        private static void CopyDirectoryRecursive(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);
            string[] files = Directory.GetFiles(sourceDir);
            foreach (string file in files)
            {
                if(file.Contains(".meta"))
                    continue;
                string fileName = Path.GetFileName(file);
                string targetFile = Path.Combine(targetDir, fileName);
                File.Copy(file, targetFile, true);
            }
            string[] directories = Directory.GetDirectories(sourceDir);
            foreach (string dir in directories)
            {
                string dirName = Path.GetFileName(dir);
                string targetSubDir = Path.Combine(targetDir, dirName);
                CopyDirectoryRecursive(dir, targetSubDir);
            }
        }
        static bool CheckPrefabInScene(GameObject prefab)
        {
            GameObject[] allObjectsInScene = UnityEngine.Object.FindObjectsOfType<GameObject>();

            foreach (GameObject obj in allObjectsInScene)
                if (obj.name == prefab.name)
                    return true;

            return false;
        }

        static void AddPrefabToScene(GameObject prefab)
        {
            GameObject newObj = Instantiate(prefab);
            newObj.transform.position = Vector3.zero;
            newObj.transform.rotation = Quaternion.identity;
        }
    }
    
    /// <summary>
    /// The main editor window to run profiling tests with a connected device.
    /// </summary>
    public class DeviceProfilingWindow : EditorWindow
    {
        private const string introductionText =
            "An automated and customizable graphics profiling tool for " +
            "evaluating XR headset performance. You can easily integrate this tool into your existing XR projects or " +
            "explore its capabilities through our showcase project, which features high-quality VR and MR demo scenes. " +
            "The toolkit supports automated test scripting, graphics feature toggling, profiling data export, " +
            "and report generation for in-depth performance analysis and comparison.";
        
        private string scriptDirectionPath;
        private string scriptPackageDirectionPath;
        private string _scriptPath;
        private string[] _scriptArgs;
        private const float DefaultLabelWidth = 120f;
        private AddRequest _addRequest;
        public DeviceProfilingWindow()
        {
            minSize = new Vector2(750f, 820f);
            float windowWidth = Screen.width * 0.4f;
            float windowHeight = windowWidth * 0.75f;
            position = new Rect((Screen.width - windowWidth) / 2, (Screen.height - windowHeight) / 2, 750f, 820f);
        }
        void OnEnable()
        {
            scriptDirectionPath = Application.dataPath+"/XRProfilingToolKit/Editor/ProfilingToolScripts/";
            scriptPackageDirectionPath = UnityEditor.PackageManager.PackageInfo.FindForAssetPath("Packages/com.pico.xr.profiling.toolkit").resolvedPath+"/Editor/ProfilingToolScripts/";
        }
        [MenuItem("XR Profiling ToolKit/Device Profiling Tool Window")]
        public static void ShowWindow()
        {
            GetWindow<DeviceProfilingWindow>("Device Profiling Tool Window");
        }
        
        private void OnGUI()
        {
            GUIStyle titleStyle = new GUIStyle();
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.normal.textColor = new Color(0, 122f / 255f, 204f / 255f);
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.fontSize = 25;
            GUIStyle warningStyle = new GUIStyle();
            warningStyle.normal.textColor = new Color(255, 255, 0);
            warningStyle.fontStyle = FontStyle.Bold;
            warningStyle.fontSize = 15;
            GUIStyle normalTitleStyle = new GUIStyle();
            normalTitleStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
            normalTitleStyle.fontSize = 20;
            normalTitleStyle.margin = new RectOffset(5, 5, 5, 5);
            GUIStyle normalStyle = new GUIStyle();
            normalStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
            normalStyle.margin = new RectOffset(5, 5, 5, 5);
            normalStyle.wordWrap = true;
            EditorGUIUtility.labelWidth = DefaultLabelWidth;
            GUILayout.Label("Device Profiling Tool Window",titleStyle,GUILayout.Height(40f));
            GUIStyle style = "frameBox";
            style.fixedWidth = 750f;
            GUILayout.Space(10);
            GUILayout.Label("Auto Run Tool", normalTitleStyle);
            GUILayout.BeginVertical(style);
            GUILayout.Label("You can select a pre-configurated command sequence (CommandQueue) for running automated tests.\nConnect your XR build device and press the Run Automation button to generate the profiling data to the output path. ", normalStyle);
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            {
                GUIContent commandQueueFilePathLabel = new GUIContent("CommandQueue File Path [?]: ", "The file path to the CommandQueue object.");
                EditorGUILayout.LabelField(commandQueueFilePathLabel, GUILayout.ExpandWidth(false));
                EditorGUILayout.TextField(XRProfilingToolkitToolSettings.CommandQueueFilePath);
                if (GUILayout.Button("Browse",GUILayout.ExpandWidth(false)))
                {
                    XRProfilingToolkitToolSettings.CommandQueueFilePath = EditorUtility.OpenFilePanel("Select CommandQueue File", Directory.Exists(scriptDirectionPath)?scriptDirectionPath:scriptPackageDirectionPath + "AutomationScripts/", "json");
                    if (XRProfilingToolkitToolSettings.CommandQueueFilePath == "")
                    {
                        XRProfilingToolkitToolSettings.CommandQueueFilePath = Directory.Exists(scriptDirectionPath)?scriptDirectionPath:scriptPackageDirectionPath + "AutomationScripts/PerformanceTestSampleCommandQueue.json";
                    }
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            {
                GUIContent outputPathLabel = new GUIContent("Profiling Data Output Path [?]: ", "The file path to output profiling data.");
                EditorGUILayout.LabelField(outputPathLabel, GUILayout.ExpandWidth(false));
                EditorGUILayout.TextField(XRProfilingToolkitToolSettings.OutputPath);
                if (GUILayout.Button("Browse",GUILayout.ExpandWidth(false)))
                {
                    XRProfilingToolkitToolSettings.OutputPath = EditorUtility.OpenFolderPanel("Select OutputPath Path", XRProfilingToolkitToolSettings.OutputPath, "");
                    if (XRProfilingToolkitToolSettings.OutputPath == "")
                    {
                        XRProfilingToolkitToolSettings.OutputPath = Application.dataPath.Replace("/Assets", "");
                    }
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Run Automation",GUILayout.Width(180f), GUILayout.ExpandWidth(false)))
                {
                    RunAutomation();
                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(15f);
            GUILayout.Label("Analysis Tool", normalTitleStyle);
            GUILayout.BeginVertical(style);

            GUILayout.Label("You need to complete the environment configuration before using the following tools.", warningStyle);
            GUILayout.Space(10);
            if (GUILayout.Button(new GUIContent("Set Up Analysis Tool Environment", "Install Python requirements for analysis"), GUILayout.Width(300f),GUILayout.ExpandWidth(false)))
            {
                PythonEnvironmentInit();
            }
                
            GUILayout.Label("After running the test, you can select the profiling data (e.g. from the above step) to generate an analysis report.\nThe folder name in the directory path starts with \"xr_profiling_session_\".", normalStyle);
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            {
                GUIContent analysisDirectoryPathLabel = new GUIContent("Profiling Data Directory [?]: ", "The path to profiling data, folder name starting with \"xr_profiling_session_\".");
                EditorGUILayout.LabelField(analysisDirectoryPathLabel, GUILayout.ExpandWidth(false));
                EditorGUILayout.TextField(XRProfilingToolkitToolSettings.AnalysisDirectoryPath);
                if (GUILayout.Button("Browse",GUILayout.ExpandWidth(false)))
                {
                    XRProfilingToolkitToolSettings.AnalysisDirectoryPath = EditorUtility.OpenFolderPanel("Select Analysis Directory Path", (XRProfilingToolkitToolSettings.AnalysisDirectoryPath!= "")?XRProfilingToolkitToolSettings.AnalysisDirectoryPath:XRProfilingToolkitToolSettings.OutputPath, "");
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Generate Analysis Report",GUILayout.Width(180f),GUILayout.ExpandWidth(false)))
                {
                    RunAnalysis();
                }
                GUILayout.FlexibleSpace();
                
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10f);
            GUILayout.Label("You can compare two profiling data sets from the same CommandQueue by selecting a second data directory path.\nThis generates a comparison report, useful for viewing results from the same test sequences.", normalStyle);
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            {
                GUIContent compareDirectoryPathLabel = new GUIContent("Comparison Profiling Directory [?]: ", "The path to the comparison profiling data, folder name starting with \"xr_profiling_session_\".");
                EditorGUILayout.LabelField(compareDirectoryPathLabel, GUILayout.ExpandWidth(false));
                EditorGUILayout.TextField(XRProfilingToolkitToolSettings.CompareDirectoryPath);
                if (GUILayout.Button("Browse",GUILayout.ExpandWidth(false)))
                {
                    XRProfilingToolkitToolSettings.CompareDirectoryPath = EditorUtility.OpenFolderPanel("Select Comparison Directory Path", (XRProfilingToolkitToolSettings.CompareDirectoryPath!= "")?XRProfilingToolkitToolSettings.CompareDirectoryPath:XRProfilingToolkitToolSettings.OutputPath, "");
                }
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Generate Comparison Report",GUILayout.Width(180f),GUILayout.ExpandWidth(false)))
                {
                    RunCompare();
                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.EndVertical();
            GUILayout.Space(15f);
            GUILayout.Label("Report Shortcuts", normalTitleStyle);
            GUILayout.BeginVertical(style);
            GUILayout.Label("Useful shortcut for opening the output profiling data folder", normalStyle);
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Open Output Folder", GUILayout.Width(220f), GUILayout.ExpandWidth(false)))
                {
                    OpenOutputFolder();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Label("Useful shortcuts to copy command line commands in case the above one-click buttons do not work, You can copy-paste these commands into the terminal.", normalStyle);
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Copy Run Automation Command",GUILayout.Width(240f), GUILayout.ExpandWidth(false)))
                {
                    CopyRunAutomation();
                }
                if (GUILayout.Button("Copy Generate Analysis Command",GUILayout.Width(240f), GUILayout.ExpandWidth(false)))
                {
                    CopyRunAnalysis();
                }
                if (GUILayout.Button("Copy Generate Comparison Command",GUILayout.Width(240f), GUILayout.ExpandWidth(false)))
                {
                    CopyRunCompare();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.BeginVertical();

            EditorGUILayout.LabelField(introductionText);
            GUIContent documentationWebsiteLabel = new GUIContent("Documentation Website");
#if UNITY_2021_1_OR_NEWER
            if (EditorGUILayout.LinkButton(documentationWebsiteLabel))
#else
            if (GUILayout.Button(ODHLabel, GUILayout.ExpandWidth(false)))
#endif
            {
#if UNITY_EDITOR_WIN
                Application.OpenURL(
                    "https://developer.picoxr.com/zh/document/unity/pico-graphics-probe-tool/");
#elif UNITY_EDITOR_OSX
                Application.OpenURL("https://developer.picoxr.com/zh/document/unity/pico-graphics-probe-tool/");
#endif
            }
            GUIContent supportWebsiteLabel = new GUIContent("Support Website");
#if UNITY_2021_1_OR_NEWER
            if (EditorGUILayout.LinkButton(supportWebsiteLabel))
#else
            if (GUILayout.Button(ODHLabel, GUILayout.ExpandWidth(false)))
#endif
            {
#if UNITY_EDITOR_WIN
                Application.OpenURL(
                    "https://developer.picoxr.com/zh/document/unity/pico-graphics-probe-tool/");
#elif UNITY_EDITOR_OSX
                Application.OpenURL("https://developer.picoxr.com/zh/document/unity/pico-graphics-probe-tool/");
#endif
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        public static void CopyToClipboard(string text)
        {
            TextEditor te = new TextEditor();
            te.text = text;
            te.SelectAll();
            te.Copy();
        }
        public void RunAutomation()
        {

            _scriptPath = (Directory.Exists(scriptDirectionPath)?scriptDirectionPath:scriptPackageDirectionPath) + "run.py";
            _scriptArgs = new string[]{ " --file " + XRProfilingToolkitToolSettings.CommandQueueFilePath, " --outputPath " + XRProfilingToolkitToolSettings.OutputPath," --appid " + Application.identifier };
            ExecutePythonWithArgs(_scriptPath,_scriptArgs);
        }
        
        public void CopyRunAutomation()
        {
#if UNITY_EDITOR_WIN
            CopyToClipboard("python "+ (Directory.Exists(scriptDirectionPath)?scriptDirectionPath:scriptPackageDirectionPath) + "run.py --file " + XRProfilingToolkitToolSettings.CommandQueueFilePath + " --outputPath " + XRProfilingToolkitToolSettings.OutputPath + " --appid " + Application.identifier );
#elif UNITY_EDITOR_OSX
            CopyToClipboard("python3 "+ (Directory.Exists(scriptDirectionPath)?scriptDirectionPath:scriptPackageDirectionPath) +  "/run.py --file " + XRProfilingToolkitToolSettings.CommandQueueFilePath + " --outputPath " + XRProfilingToolkitToolSettings.OutputPath + " --appid " + Application.identifier );
#endif
        }
        
        public void RunAnalysis()
        {
            if (string.IsNullOrEmpty(XRProfilingToolkitToolSettings.AnalysisDirectoryPath))
            {
                XRProfilingToolkitToolSettings.AnalysisDirectoryPath = EditorUtility.OpenFolderPanel("Select Analysis Directory Path", (XRProfilingToolkitToolSettings.AnalysisDirectoryPath!= "")?XRProfilingToolkitToolSettings.AnalysisDirectoryPath:XRProfilingToolkitToolSettings.OutputPath, "");
                return;
            }
            _scriptPath = (Directory.Exists(scriptDirectionPath)?scriptDirectionPath:scriptPackageDirectionPath) + "analyze.py";
            _scriptArgs = new string[]{ "-s " + XRProfilingToolkitToolSettings.AnalysisDirectoryPath };
            ExecutePythonWithArgs(_scriptPath,_scriptArgs);
        }
        
        public void CopyRunAnalysis()
        {
            if (string.IsNullOrEmpty(XRProfilingToolkitToolSettings.AnalysisDirectoryPath))
            {
                XRProfilingToolkitToolSettings.AnalysisDirectoryPath = EditorUtility.OpenFolderPanel("Select Analysis Directory Path", (XRProfilingToolkitToolSettings.AnalysisDirectoryPath!= "")?XRProfilingToolkitToolSettings.AnalysisDirectoryPath:XRProfilingToolkitToolSettings.OutputPath, "");
                return;
            }
#if UNITY_EDITOR_WIN
            CopyToClipboard("python "+ (Directory.Exists(scriptDirectionPath)?scriptDirectionPath:scriptPackageDirectionPath) + "analyze.py -s " + XRProfilingToolkitToolSettings.AnalysisDirectoryPath);
#elif UNITY_EDITOR_OSX
            CopyToClipboard("python3 "+ (Directory.Exists(scriptDirectionPath)?scriptDirectionPath:scriptPackageDirectionPath) + "analyze.py -s " + XRProfilingToolkitToolSettings.AnalysisDirectoryPath);
#endif
        }
        
        public void RunCompare()
        {
            if (string.IsNullOrEmpty(XRProfilingToolkitToolSettings.AnalysisDirectoryPath))
            {
                XRProfilingToolkitToolSettings.AnalysisDirectoryPath = EditorUtility.OpenFolderPanel("Select Analysis Directory Path", (XRProfilingToolkitToolSettings.AnalysisDirectoryPath!= "")?XRProfilingToolkitToolSettings.AnalysisDirectoryPath:XRProfilingToolkitToolSettings.OutputPath, "");
                return;
            }
            if (string.IsNullOrEmpty(XRProfilingToolkitToolSettings.CompareDirectoryPath)||XRProfilingToolkitToolSettings.AnalysisDirectoryPath == XRProfilingToolkitToolSettings.CompareDirectoryPath)
            {
                XRProfilingToolkitToolSettings.CompareDirectoryPath = EditorUtility.OpenFolderPanel("Select Analysis Directory Path", (XRProfilingToolkitToolSettings.CompareDirectoryPath!= "")?XRProfilingToolkitToolSettings.CompareDirectoryPath:XRProfilingToolkitToolSettings.OutputPath, "");
                return;
            }
            _scriptPath = (Directory.Exists(scriptDirectionPath)?scriptDirectionPath:scriptPackageDirectionPath) + "compare.py";
            _scriptArgs = new string[]{ "-s " + XRProfilingToolkitToolSettings.AnalysisDirectoryPath + " " + XRProfilingToolkitToolSettings.CompareDirectoryPath };
            ExecutePythonWithArgs(_scriptPath,_scriptArgs);
        }
        
        public void CopyRunCompare()
        {
            if (string.IsNullOrEmpty(XRProfilingToolkitToolSettings.AnalysisDirectoryPath))
            {
                XRProfilingToolkitToolSettings.AnalysisDirectoryPath = EditorUtility.OpenFolderPanel("Select Analysis Directory Path", (XRProfilingToolkitToolSettings.AnalysisDirectoryPath!= "")?XRProfilingToolkitToolSettings.AnalysisDirectoryPath:XRProfilingToolkitToolSettings.OutputPath, "");
                return;
            }
            if (string.IsNullOrEmpty(XRProfilingToolkitToolSettings.CompareDirectoryPath)||XRProfilingToolkitToolSettings.AnalysisDirectoryPath == XRProfilingToolkitToolSettings.CompareDirectoryPath)
            {
                XRProfilingToolkitToolSettings.CompareDirectoryPath = EditorUtility.OpenFolderPanel("Select Analysis Directory Path", (XRProfilingToolkitToolSettings.CompareDirectoryPath!= "")?XRProfilingToolkitToolSettings.CompareDirectoryPath:XRProfilingToolkitToolSettings.OutputPath, "");
                return;
            }
#if UNITY_EDITOR_WIN
            CopyToClipboard("python "+ (Directory.Exists(scriptDirectionPath)?scriptDirectionPath:scriptPackageDirectionPath) + "compare.py -s " + XRProfilingToolkitToolSettings.AnalysisDirectoryPath + " " + XRProfilingToolkitToolSettings.CompareDirectoryPath);
#elif UNITY_EDITOR_OSX
            CopyToClipboard("python3 "+ (Directory.Exists(scriptDirectionPath)?scriptDirectionPath:scriptPackageDirectionPath) + "compare.py -s " + XRProfilingToolkitToolSettings.AnalysisDirectoryPath + " " + XRProfilingToolkitToolSettings.CompareDirectoryPath);
#endif
        }
        
        public void PythonEnvironmentInit()
        {
            string pipCommand;
#if UNITY_EDITOR_WIN
            pipCommand = "pip install -r " + (Directory.Exists(scriptDirectionPath)?scriptDirectionPath:scriptPackageDirectionPath) + "requirements.txt";
#elif UNITY_EDITOR_OSX
            pipCommand = "pip3 install -r " + (Directory.Exists(scriptDirectionPath)?scriptDirectionPath:scriptPackageDirectionPath) + "requirements.txt";
#endif
            Process process = new Process();
#if UNITY_EDITOR_WIN
            process.StartInfo.FileName = "python";
#elif UNITY_EDITOR_OSX
            process.StartInfo.FileName = "python3";
#endif
            process.StartInfo.Arguments = " -m " + pipCommand;
            try
            {
                process.Start();
                process.WaitForExit();
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError("Error running Python script: " + e.Message);
            }
        }

        private static void ExecutePythonWithArgs(string scriptPath, string[] args)
        {
            Process process = new Process();
#if UNITY_EDITOR_WIN
            process.StartInfo.FileName = "python";
#elif UNITY_EDITOR_OSX
            process.StartInfo.FileName = "python3";
#endif
            process.StartInfo.Arguments = scriptPath + " " + string.Join(" ", args);
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            try
            {
                process.Start();
                StringBuilder outputBuilder = new StringBuilder();
                while (!process.StandardOutput.EndOfStream)
                {
                    string line = process.StandardOutput.ReadLine();
                    outputBuilder.Append(line);
                    UnityEngine.Debug.Log(line); 
                }
                string error = process.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(error))
                {
                    UnityEngine.Debug.LogError(error); 
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError("Error running Python script: " + e.Message);
            }
            finally
            {
                process.Close();
            }
        }
        private void OpenScriptFolder()
        {
            EditorUtility.RevealInFinder((Directory.Exists(scriptDirectionPath)?scriptDirectionPath:scriptPackageDirectionPath));
        }
        private void OpenOutputFolder()
        {
            EditorUtility.RevealInFinder(XRProfilingToolkitToolSettings.OutputPath+"/");
        }
    }
}
