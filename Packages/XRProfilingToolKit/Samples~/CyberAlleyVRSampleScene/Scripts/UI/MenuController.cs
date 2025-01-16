/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace DeveloperTech.XRProfilingToolkit.Scene
{
    /// <summary>
    /// Toggle the UI main menu on controller button press
    /// </summary>
    public class MenuController : MonoBehaviour
    {
        [Serializable]
        class PlatformInputActions
        {
            public PlatformSwitcher.PlatformType targetPlatform;
            public InputActionReference[] inputActionReferences;
        }
        [SerializeField]
        private Transform _targetTransform;
        
        [SerializeField]
        private EventSystem _uiEventSystem;
        
        [SerializeField]
        private float _uiDistance = 30.0f;

        [SerializeField]
        private GameObject _uiRoot;

        [SerializeField] 
        private PlatformInputActions[] _menuButtonPlatformInputs;

        public bool MenuUIActive => _uiRoot.activeInHierarchy;

        public UnityEvent<bool> MenuToggled { get; set; } = new UnityEvent<bool>();

        public Transform TargetTransform
        {
            get
            {
                if (_targetTransform == null)
                {
                    _targetTransform = Camera.main.transform;
                }

                return _targetTransform;
            }
        }

        private void Awake()
        {
            foreach (var platformInput in _menuButtonPlatformInputs)
            {
                if (platformInput.targetPlatform != PlatformSwitcher.GetPlatform())
                    continue;
                foreach (var inputActionReference in platformInput.inputActionReferences)
                {
                    inputActionReference.action.Enable();
                    inputActionReference.action.performed += _ => ToggleUI();
                    
                }
            }
        }

        [ContextMenu("Toggle UI")]
        private void ToggleUI()
        {
            // deselect last interacted UI component, else opening the UI menu with A button will trigger the component
            if (_uiEventSystem != null) _uiEventSystem.SetSelectedGameObject(null);
            bool wasEnabled = _uiRoot.activeSelf;
            _uiRoot.SetActive(!wasEnabled);
            if (!wasEnabled)
            {
                var forwardDir = TargetTransform.forward;
                forwardDir.y = 0f;
                forwardDir.Normalize();
                _uiRoot.transform.position = TargetTransform.position + forwardDir * _uiDistance;
                _uiRoot.transform.rotation = Quaternion.Euler(0f, TargetTransform.rotation.eulerAngles.y, 0f);
            }
            MenuToggled.Invoke(!wasEnabled);
        }
    }
}
