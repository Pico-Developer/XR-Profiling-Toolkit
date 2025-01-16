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
    /// Automation command to load a specific Scene to do profiling.
    /// </summary>
    [Serializable]
    public class CommandLoadLevel : CommandBase
    {
        /// <summary>
        /// Index of the scene to load, starting from 0.
        /// </summary>
        /// <remarks>
        /// If set to -1, will try loading the main menu scene."
        /// </remarks>
        [Tooltip("Index of the scene to load, starting from 0. If set to -1, will try loading the main menu")]
        public int levelIndex;

        private AsyncOperation _operation;

        /// <summary>
        /// Event when a profiling scene is loaded.
        /// </summary>
        public static Action OnLevelLoaded;

        /// <summary>
        /// Event when a profiling scene is unloaded.
        /// </summary>
        public static Action OnLevelUnloaded;

        public override IEnumerator Run()
        {
            OnLevelUnloaded?.Invoke();
            _operation = levelIndex < 0 ? SceneLoader.LoadMainMenu() : SceneLoader.OnSceneSelected(levelIndex);
            while (!_operation.isDone)
            {
                yield return null;
            }
            XRProfilingToolkitLogger.Log($"{GetType().Name} Scene loaded, name: {SceneManager.GetActiveScene().name}");
            OnLevelLoaded?.Invoke();
        }

        public override void Pause()
        {
            if (_operation == null || _operation.isDone) return;
            _operation.allowSceneActivation = false;
        }

        public override void Resume()
        {
            if (_operation == null || _operation.isDone) return;
            _operation.allowSceneActivation = true;
        }
    }
}