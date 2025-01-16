/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEditor;
using UnityEngine;
using System.IO;

namespace DeveloperTech.XRProfilingToolkit.Automation.Editor
{
    /// <summary>
    /// Custom editor window for <see cref="CommandQueue"/> editing
    /// </summary>
    [CustomEditor(typeof(CommandQueue))]
    public class CommandQueueEditor : UnityEditor.Editor
    {
        private CommandQueue _cq;

        private void OnEnable()
        {
            _cq = target as CommandQueue;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Save to File"))
            {
                SaveFileTo();
            }
        }

        private void SerializeCommandQueue()
        {
            CommandSerializer.SerializeCommandQueue(_cq);
            BumpMinorVersion(_cq);
        }

        private void RemoveSerialized()
        {
            if (File.Exists(CommandSerializer.FilePath))
            {
                File.Delete(CommandSerializer.FilePath);
            }
        }

        private void BumpMinorVersion(CommandQueue cq)
        {
            string[] splitArray = cq.id.Split(char.Parse("."));
            int minorVersion = int.Parse(splitArray[splitArray.Length - 1]);
            splitArray[splitArray.Length - 1] = (++minorVersion).ToString();
            cq.id = string.Join(".", splitArray);
        }

        private void LoadSerialized()
        {
            var deserialized = CreateInstance<CommandQueue>();
            CommandSerializer.DeserializeCommandQueue(ref deserialized);
            _cq.Copy(deserialized);
            DestroyImmediate(deserialized);
        }

        private void SaveFileTo()
        {
            string path = EditorUtility.SaveFilePanel("Save automation file", "", "CommandQueue", "json");
            CommandSerializer.SerializeCommandQueueTo(_cq, path);
            BumpMinorVersion(_cq);
        }
    }
}