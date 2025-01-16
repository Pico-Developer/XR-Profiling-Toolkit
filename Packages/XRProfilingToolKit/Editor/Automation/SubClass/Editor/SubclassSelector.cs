/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace DeveloperTech.XRProfilingToolkit.Utilities
{
    /// <summary>
    /// Dropdown list for the user to select which subclass should be serialized
    /// </summary>
    public class SubclassSelector
    {
        private readonly List<System.Type> _subclasses;
        private readonly string[] _subclassNames;

        private int _selectedType;

        public SubclassSelector(System.Type classType, System.Type defaultType = null, bool includeSelf = false)
        {
            _selectedType = -1;

            _subclasses = new List<System.Type>();

            System.Type[] foundClasses = System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(classType)).ToArray();

            if (includeSelf)
            {
                _subclasses.Add(classType);

                if (defaultType == classType)
                    _selectedType = 0;
            }

            for (int i = 0; i < foundClasses.Length; i++)
            {
                if (foundClasses[i].ContainsGenericParameters)
                    continue;

                if (defaultType != null)
                {
                    if (defaultType == foundClasses[i])
                        _selectedType = _subclasses.Count;
                }

                _subclasses.Add(foundClasses[i]);
            }

            _subclassNames = new string[_subclasses.Count];

            for (int i = 0; i < _subclassNames.Length; i++)
            {
                _subclassNames[i] = _subclasses[i].Name;
            }
        }

        public void RefreshSelection(System.Type defaultType)
        {
            _selectedType = -1;

            for (int i = 0; i < _subclasses.Count; i++)
            {
                if (defaultType == null)
                    break;

                if (defaultType != _subclasses[i])
                    continue;

                _selectedType = i;
                break;
            }
        }

        public bool Draw(Rect position)
        {
            int selectedIdx = _selectedType < 0 ? 0 : _selectedType;

            if (selectedIdx >= 0)
            {
                selectedIdx = EditorGUI.Popup(position, selectedIdx, _subclassNames);
            }

            if (selectedIdx != _selectedType)
            {
                _selectedType = selectedIdx;

                return true;
            }

            return false;
        }

        public object CreateSelected()
        {
            return System.Activator.CreateInstance(_subclasses[_selectedType]);
        }
    }
}