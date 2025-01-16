/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DeveloperTech.XRProfilingToolkit.UI
{
    /// <summary>
    /// Wrapper of <see cref="Toggle"/> to switch on/off an individual <see cref="XRFeature"/> 
    /// </summary>
    public class XRFeatureToggle : MonoBehaviour
    {
        [SerializeField]
        internal XRFeature _feature;

        [SerializeField]
        private Toggle _toggle;

        [SerializeField]
        private TextMeshProUGUI _toggleLabel;

        [SerializeField]
        private Color _enabledTextColor;

        [SerializeField]
        private Color _disabledTextColor;

        public bool ToggleEnableStatus => _toggle.enabled;

        public bool ToggleOnStatus => _toggle.isOn;

        public void AddListener(UnityAction<XRFeature, bool> onFeatureToggled)
        {
            _toggle.onValueChanged.AddListener(isOn => onFeatureToggled(_feature, isOn));
        }

        public void ToggleFeature(bool isOn)
        {
            if (_toggle.isOn == isOn)
            {
                return;
            }
            _toggle.isOn = isOn;
        }

        public void EnableToggle(bool enabled)
        {
            _toggle.enabled = enabled;
            _toggleLabel.color = enabled ? _enabledTextColor : _disabledTextColor;
        }
    }
}
