/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace DeveloperTech.XRProfilingToolkit.UI
{
    /// <summary>
    /// Applies toggle animation when <see cref="XRFeatureToggle"/> being switched
    /// Tweening will be applied between on and off positions 
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class ToggleAnimation : MonoBehaviour
    {
        [SerializeField]
        private RectTransform uiHandleRectTransform;
        
        [SerializeField]
        private Color backgroundActiveColor;

        [SerializeField]
        private Color handleActiveColor;
        
        [SerializeField]
        private Toggle toggle;

        [SerializeField]
        private Image backgroundImage;

        [SerializeField]
        private Image handleImage;

        private Color backgroundDefaultColor, handleDefaultColor;

        private Vector2 handlePosition;

        private void Awake()
        {
            if (toggle == null)
            {
                toggle = GetComponent<Toggle>();
            }

            handlePosition = uiHandleRectTransform.anchoredPosition;

            backgroundDefaultColor = backgroundImage.color;
            handleDefaultColor = handleImage.color;

            toggle.onValueChanged.AddListener(OnSwitch);

            if (toggle.isOn)
            {
                OnSwitch(true);
            }
        }

        private void OnSwitch(bool on)
        {
            uiHandleRectTransform.DOAnchorPos(on ? handlePosition * -1.0f : handlePosition, 0.4f).SetEase(Ease.InOutBack);
            backgroundImage.DOColor(on ? backgroundActiveColor : backgroundDefaultColor, 0.6f);
            handleImage.DOColor(on ? handleActiveColor : handleDefaultColor, 0.4f);
        }

        private void OnDestroy()
        {
            toggle.onValueChanged.RemoveListener(OnSwitch);
        }
    }
}
