/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using UnityEngine;

namespace DeveloperTech.XRProfilingToolkit.UI
{
    /// <summary>
    /// Enables tab switching among tab views by clicking on the <see cref="TabButton"/>
    /// </summary>
    public class TabGroup : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> _tabViews;

        private List<TabButton> _tabButtons;

        public void Subscribe(TabButton button)
        {
            if (_tabButtons == null) _tabButtons = new List<TabButton>();

            _tabButtons.Add(button);
        }

        public void OnTabSelected(TabButton button)
        {
            int index = button.transform.GetSiblingIndex();
            for (int i = 0; i < _tabViews.Count; i++)
            {
                _tabViews[i].SetActive(i == index);
            }
        }
    }
}
