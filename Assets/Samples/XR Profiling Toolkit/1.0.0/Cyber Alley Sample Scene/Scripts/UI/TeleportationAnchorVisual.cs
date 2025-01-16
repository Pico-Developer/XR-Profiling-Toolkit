/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace DeveloperTech.XRProfilingToolkit.UI
{
    /// <summary>
    /// Switches between teleportation anchor visual modes
    /// Active fx when anchor is being hovered on
    /// </summary>
    public class TeleportationAnchorVisual : MonoBehaviour
    {
        [SerializeField] private TMP_Text _indexText;

        [SerializeField] private GameObject _hoverEffect;

        [SerializeField] private GameObject _normalEffect;

        public void SetIndexText(int index)
        {
            _indexText.text = index.ToString();
        }

        public void ToggleText(bool enable)
        {
            _indexText.enabled = enable;
        }

        public void OnHoverEnter(HoverEnterEventArgs arg)
        {
            _hoverEffect.SetActive(true);
            _normalEffect.SetActive(false);
        }

        public void OnHoverExit(HoverExitEventArgs arg)
        {
            _hoverEffect.SetActive(false);
            _normalEffect.SetActive(true);
        }
    }
}
