/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

namespace DeveloperTech.XRProfilingToolkit.Automation
{
    /// <summary>
    /// Serialize or deserialize a <see cref="CommandQueue"/> between ScriptableObject and JSON
    /// </summary>
    public static class CommandSerializer
    {
        private static string FileName = "CommandQueue.json";

        public static string UploadedFilePath => Path.Combine(Application.persistentDataPath, FileName);
        public static string FilePath => Path.Combine(Application.streamingAssetsPath, FileName);

        private static JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        /// <summary>
        /// Serialize the <see cref="CommandQueue"/> into streaming assets folder
        /// </summary>
        public static void SerializeCommandQueue(CommandQueue cq)
        {
            string jsonString = JsonConvert.SerializeObject(cq, Formatting.Indented, jsonSettings);
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }
            File.WriteAllText(FilePath, jsonString);
        }

        /// <summary>
        /// Serialize the <see cref="CommandQueue"/> to the given path
        /// </summary>
        public static void SerializeCommandQueueTo(CommandQueue cq, string filePath)
        {
            string jsonString = JsonConvert.SerializeObject(cq, Formatting.Indented, jsonSettings);
            File.WriteAllText(filePath, jsonString);
        }

        /// <summary>
        /// Deserialize the <see cref="CommandQueue"/> that is built into the apk or uploaded to persistent data path of the app
        /// </summary>
        public static bool DeserializeCommandQueue(ref CommandQueue cq)
        {
            string jsonString = "";
            if (Application.platform == RuntimePlatform.Android)
            {
                try
                {
                    if (File.Exists(UploadedFilePath))
                    {
                        using (StreamReader reader = File.OpenText(UploadedFilePath))
                        {
                            jsonString = reader.ReadToEnd();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error reading the file {UploadedFilePath}, exception {ex}");
                }

                if (string.IsNullOrEmpty(jsonString))
                {
                    UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(FilePath);
                    var asyncOp = www.SendWebRequest();
                    while (!asyncOp.isDone) { }
                    if (!string.IsNullOrEmpty(www.error))
                    {
                        return false;
                    }
                    jsonString = www.downloadHandler.text;
                }
            }
            else
            {
                if (!Directory.Exists(Application.streamingAssetsPath) || !File.Exists(FilePath))
                {
                    return false;
                }
                jsonString = File.ReadAllText(FilePath);
            }
            JsonConvert.PopulateObject(jsonString, cq, jsonSettings);
            return true;
        }
    }
}