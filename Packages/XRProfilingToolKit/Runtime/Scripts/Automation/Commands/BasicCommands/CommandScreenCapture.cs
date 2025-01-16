/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DeveloperTech.XRProfilingToolkit.Automation
{
    /// <summary>
    /// Automation command to capture screen
    /// </summary>
    [Serializable]
    public class CommandScreenCapture : CommandBase
    {
        /// <summary>
        /// Types of capture, the first three options are for screen image capture and
        /// the last two are capturing low-level GPU metrics.
        /// </summary>
        public enum CaptureType
        {
            CaptureScreen,
            StartScreenRecord,
            EndScreenRecord,
            CaptureRenderingStage,
            CaptureDrawCall,
        }

        public CaptureType type;

        [Tooltip("Context of the capture, captures with the same context will be grouped together.")]
        public string context;

        [SerializeField]
        private bool useUnityScreenCapture;

        private const float CommandWaitTimeInSeconds = 2.0f;

        public override IEnumerator Run()
        {
            if (useUnityScreenCapture && type == CaptureType.CaptureScreen)
            {
                XRProfilingToolkitLogger.Log($"{GetType().Name} Capturing screen with Unity service: {context}");
                ScreenCapture.CaptureScreenshot($"{SceneManager.GetActiveScene().name}_{context}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}");
            }
            else
            {
                XRProfilingToolkitLogger.Log($"{GetType().Name} {type}:{context}");
            }
            // wait for the log to be processed, makes this longer to account for adb log and screen capture latency
            yield return new WaitForSeconds(CommandWaitTimeInSeconds);
        }

        public override void Pause()
        {
            XRProfilingToolkitLogger.Log($"{GetType().Name} cannot be paused.");
        }

        public override void Resume()
        {
            XRProfilingToolkitLogger.Log($"{GetType().Name} cannot be resumed.");
        }
    }
}