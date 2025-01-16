/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace DeveloperTech.XRProfilingToolkit
{
    /// <summary>
    /// Prints out the logs related to the XR Profiling Toolkit.
    /// Use this over <see cref="UnityEngine.Debug.Log"/> for XR Profiling Toolkit related logging
    /// as this will add XR Profiling Toolkit contexts to the message.
    /// </summary>
    public static class XRProfilingToolkitLogger
    {
        private const string _logTag = "XRProfilingToolkit";

        /// <summary>
        /// Types of XR profiling log
        /// </summary>
        public enum LogType
        {
            /// <summary>
            /// General information
            /// </summary>
            General = 0,
            
            /// <summary>
            /// Device specifications
            /// </summary>
            Device = 1,
        }
        public static void Log(string message, LogType logType = LogType.General)
        {
            if (logType > 0)
            {
                message = $"${logType} {message}";
            }
            Debug.Log($"{_logTag} FrameID={Time.frameCount}>>>>>>{message}");
        }
    }
}
