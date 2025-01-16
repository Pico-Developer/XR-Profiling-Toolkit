/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////


using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;


namespace XRProfilingToolkitTool.Editor
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public sealed class XRProfilingToolkitToolSettings : ScriptableObject
    {
        static XRProfilingToolkitToolSettings()
        {
            // BuildPipeline.isBuildingPlayer cannot be called in a static constructor
            // Run Update once to call TryInitialize then remove delegate
            EditorApplication.update += Update;
        }

        static void Update()
        {
            // Initialize the instance only if a build is not currently running.
            TryInitialize();
            // Stop running Update
            EditorApplication.update -= Update;
        }

        public static string OutputPath
        {
            get
            {
                if (EditorPrefs.HasKey("XRProfilingToolkitToolSettings_OutputPath"))
                {
                    return EditorPrefs.GetString("XRProfilingToolkitToolSettings_OutputPath");
                }
                else
                {
                    return Application.dataPath.Replace("/Assets", "/Scripts");
                }
            }
            set
            {
                EditorPrefs.SetString("XRProfilingToolkitToolSettings_OutputPath", value);
            }
        }
        
        public static string CommandQueueFilePath
        {
            get
            {
                if (EditorPrefs.HasKey("XRProfilingToolkitToolSettings_CommandQueueFilePath"))
                {
                    return EditorPrefs.GetString("XRProfilingToolkitToolSettings_CommandQueueFilePath");
                }
                else
                {
                    return Application.dataPath.Replace("/Assets", "/Scripts/AutomationScripts/commandQueue.json");
                }
            }
            set
            {
                EditorPrefs.SetString("XRProfilingToolkitToolSettings_CommandQueueFilePath", value);
            }
        }
        
        public static string AnalysisDirectoryPath
        {
            get
            {
                if (EditorPrefs.HasKey("XRProfilingToolkitToolSettings_AnalysisDirectoryPath"))
                {
                    return EditorPrefs.GetString("XRProfilingToolkitToolSettings_AnalysisDirectoryPath");
                }
                else
                {
                    return "";
                }
            }
            set
            {
                EditorPrefs.SetString("XRProfilingToolkitToolSettings_AnalysisDirectoryPath", value);
            }
        }
        
        public static string CompareDirectoryPath
        {
            get
            {
                if (EditorPrefs.HasKey("XRProfilingToolkitToolSettings_CompareDirectoryPath"))
                {
                    return EditorPrefs.GetString("XRProfilingToolkitToolSettings_CompareDirectoryPath");
                }
                else
                {
                    return "";
                }
            }
            set
            {
                EditorPrefs.SetString("XRProfilingToolkitToolSettings_CompareDirectoryPath", value);
            }
        }

        public static bool TryInitialize()
        {
            // If not initialized and Build Player is current running, UnityEditor.AssetDatabase.CreateAsset
            // is unsafe to call and will cause a crash. Only load the resource if it already exists.
            if (instance == null && BuildPipeline.isBuildingPlayer)
            {
                instance = Resources.Load<XRProfilingToolkitToolSettings>("XRProfilingToolkitToolSettings");
                return instance != null;
            }

            // Otherwise create/load the resource instance normally.
            return Instance != null;
        }

        private static XRProfilingToolkitToolSettings instance;

        public static XRProfilingToolkitToolSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<XRProfilingToolkitToolSettings>("XRProfilingToolkitToolSettings");

                    if (instance == null)
                    {
                        if (BuildPipeline.isBuildingPlayer)
                        {
                            // UnityEditor.AssetDatabase.CreateAsset is unsafe to call during a build and
                            // may cause a crash.
                            // This should be rare as the asset is created in the static constructor and should
                            // usually exist.
                            throw new UnityEditor.Build.BuildFailedException(
                                "Cannot create XRProfilingToolkitToolSettings asset while building.");
                        }

                        instance = ScriptableObject.CreateInstance<XRProfilingToolkitToolSettings>();

                        string properPath = System.IO.Path.Combine(UnityEngine.Application.dataPath, "Resources");
                        if (!System.IO.Directory.Exists(properPath))
                        {
                            UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");
                        }

                        string fullPath = System.IO.Path.Combine(
                            System.IO.Path.Combine("Assets", "Resources"),
                            "XRProfilingToolkitToolSettings.asset"
                        );
                        UnityEditor.AssetDatabase.CreateAsset(instance, fullPath);
                    }
                }

                return instance;
            }
            set { instance = value; }
        }
    }
}
