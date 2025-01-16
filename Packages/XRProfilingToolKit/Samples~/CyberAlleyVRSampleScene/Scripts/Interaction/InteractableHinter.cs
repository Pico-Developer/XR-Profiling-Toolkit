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

namespace DeveloperTech.XRProfilingToolkit.Scene
{ 
    /// <summary>
    /// Pop up UI indicating the object is interactable
    /// UI will show up when <see cref="XRBaseInteractor"/> is hovering on the object for a certain period
    /// </summary>
    [RequireComponent(typeof(XRBaseInteractable))]
    public class InteractableHinter : MonoBehaviour
    {
        [SerializeField]
        private XRBaseInteractable _interactor;

        [SerializeField]
        private GameObject _hintObject;

        [SerializeField]
        private float _hintDelayInSec = 3.0f;

        [SerializeField]
        private TMP_Text _hintText;

        [SerializeField]
        private string _formatString;

        private float hoverStartTime = float.MaxValue;

        private void Start()
        {
            if (_interactor == null) _interactor = GetComponent<XRBaseInteractable>();
            if (_hintText == null) _hintText = GetComponentInChildren<TextMeshProUGUI>();

            _interactor.hoverEntered.AddListener(OnHoverEnter);
            _interactor.hoverExited.AddListener(OnHoverExit);
        }

        private void Update()
        {
            _hintObject.SetActive((Time.time - hoverStartTime) > _hintDelayInSec);
        }

        public void UpdateHintText(string text)
        {
            if (_hintText == null) return;
            _hintText.text = string.Format(_formatString, text);
        }
        
        private void OnHoverEnter(HoverEnterEventArgs args)
        {
            hoverStartTime = Time.time;
        }

        private void OnHoverExit(HoverExitEventArgs args)
        {
            hoverStartTime = float.MaxValue;
        }
    }
}
