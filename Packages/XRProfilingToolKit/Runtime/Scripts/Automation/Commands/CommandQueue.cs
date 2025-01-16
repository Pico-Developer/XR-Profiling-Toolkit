/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using DeveloperTech.XRProfilingToolkit.Utilities;
using UnityEngine;

namespace DeveloperTech.XRProfilingToolkit.Automation
{
    /// <summary>
    /// Automation commands queue that can be run in an XR profiling session
    /// </summary>
    [CreateAssetMenu(menuName = "XRProfilingToolkit/Command Queue")]
    public class CommandQueue : ScriptableObject
    {
        /// <summary>
        /// Id of the command queue, should end with semantic version
        /// </summary>
        [Tooltip("Id of the command queue, should end with semantic version")]
        public string id;

        /// <summary>
        /// Automation stops after this time, needs to be larger than 0 to be valid
        /// </summary>
        [Tooltip("Automation stops after this time, needs to be larger than 0 to be valid")]
        public float stopTime = -1.0f;

        /// <summary>
        /// List of automation commands that will be executed in sequence at runtime
        /// </summary>
        [SerializeReference, Subclass]
        [Tooltip("List of automation commands that will be executed in sequence at runtime")]
        public CommandBase[] commands;

        /// <summary>
        /// Copies a command queue and its id.
        /// </summary>
        /// <param name="other">command to copy</param>
        public void Copy(CommandQueue other)
        {
            commands = other.commands;
            id = other.id;
        }
    }
}