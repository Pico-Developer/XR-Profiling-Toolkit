/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Threading.Tasks;
using UnityEngine;

namespace DeveloperTech.XRProfilingToolkit.Scene
{
    /// <summary>
    /// Controls all dynamic objects in the current scene
    /// </summary>
    public class DynamicSystem : MonoBehaviour
    {
        [SerializeField]
        private int _defaultMode = 0;

        private DynamicObjectBase[] _dynamicObjects;

        private static DynamicSystem _instance;
        
        /// <summary>
        /// Sum of the complexity of all dynamic objects
        /// </summary>
        public static int Complexity
        {
            get
            {
                int sum = 0;
                for (int i = 0; i < _instance._dynamicObjects.Length; i++)
                {
                    sum += _instance._dynamicObjects[i].Complexity;
                }

                return sum;
            }
        }
        
        private void Start()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            _dynamicObjects = FindObjectsOfType<DynamicObjectBase>();
            foreach (DynamicObjectBase obj in _dynamicObjects)
            {
                obj.Mode = _defaultMode;
            }
        }
        

        [ContextMenu("Change Mode")]
        private async void ChangeObjectMode()
        {
            if (_dynamicObjects != null && _dynamicObjects.Length > 0)
            {
                int newMode = _dynamicObjects[0].Mode + 1;
                ChangeDynamicObjectsMode(newMode);
                await Task.Delay(1000);
                PrintComplexity();
            }
        }

        [ContextMenu("Print Complexity")]
        private void PrintComplexity()
        {
            if (_dynamicObjects != null && _dynamicObjects.Length > 0)
            {
                Debug.Log($"PXR_XRProfilingToolkit: dynamic system changed to mode {_dynamicObjects[0].Mode} with complexity: {Complexity}");
            }
        }

        private void ChangeDynamicObjectsMode(int mode)
        {
            for (int i = 0; i < _dynamicObjects.Length; i++)
            {
                _dynamicObjects[i].Mode = mode;
            }
        }

        public static void ChangeModeWithCommand(int mode = 0)
        {
            _instance.ChangeDynamicObjectsMode(mode);
        }
    }
}
