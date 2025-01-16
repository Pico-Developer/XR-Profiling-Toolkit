/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace DeveloperTech.XRProfilingToolkit
{
    /// <summary>
    /// Singleton class that loads XR profiling scenes
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField]
        private List<string> sceneNames;

        [SerializeField]
        private string mainMenuSceneName;

        private static SceneLoader _instance = null;

        // Start is called before the first frame update
        private void Start()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Loads XRProfilingToolkit scenes
        /// <param name="index">Index of teh scene to load, 0 corresponds to the first scene</param>
        /// </summary>
        public static AsyncOperation OnSceneSelected(int index)
        {
            Assert.IsTrue(index >= 0 && index < _instance.sceneNames.Count);
            return SceneManager.LoadSceneAsync(_instance.sceneNames[index], LoadSceneMode.Single);
        }

        public static AsyncOperation LoadMainMenu()
        {
            return SceneManager.LoadSceneAsync(_instance.mainMenuSceneName, LoadSceneMode.Single);
        }
    }
}
