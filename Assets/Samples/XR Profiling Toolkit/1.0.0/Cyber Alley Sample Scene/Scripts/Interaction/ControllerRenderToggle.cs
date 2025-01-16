/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using DeveloperTech.XRProfilingToolkit.Automation;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace DeveloperTech.XRProfilingToolkit.Scene
{
    /// <summary>
    /// Toggle controller visual
    /// Rendering of the controller model and interaction ray will be turned off during a XRProfilingToolkit session
    /// When command runner has active running commands
    /// </summary>
    public class ControllerRenderToggle : MonoBehaviour
    {
        [SerializeField]
        private bool _buttonHintActive;

        [SerializeField]
        private MenuController _menuController;

        private XRBaseController[] _controllers;
        private XRInteractorLineVisual[] _lineVisuals;

        private CommandRunner _commandRunner;

        private XRBaseInteractor[] _interactors;
        
        private void Start()
        {
            _controllers = GetComponentsInChildren<XRBaseController>();
            _lineVisuals = GetComponentsInChildren<XRInteractorLineVisual>();
            _interactors = GetComponentsInChildren<XRBaseInteractor>();
            _commandRunner = FindObjectOfType<CommandRunner>(true);
            if (_commandRunner != null)
            {
                _commandRunner.OnStatusChanged += ToggleRenderer;
                var status = _commandRunner.IsRunning ? CommandRunner.Status.Running : CommandRunner.Status.Idle;
                ToggleRenderer(status);
            }

            if (_menuController == null)
            {
                _menuController = FindObjectOfType<MenuController>(true);
            }

            if (_menuController != null)
            {
                _menuController.MenuToggled.AddListener(OnMenuToggled);
            }

            StartCoroutine(SetButtonActiveOnStartCoroutine());
        }

        private void OnDestroy()
        {
            if (_commandRunner != null)
            {
                _commandRunner.OnStatusChanged -= ToggleRenderer;
            }

            if (_menuController != null)
            {
                _menuController.MenuToggled.RemoveListener(OnMenuToggled);
            }
        }

        private void ToggleRenderer(CommandRunner.Status status)
        {
            bool enable = status != CommandRunner.Status.Running;
            for (var i = 0; i < _controllers.Length; i++)
            {
                _controllers[i].hideControllerModel = !enable;
            }
            for (var i = 0; i < _lineVisuals.Length; i++)
            {
                _lineVisuals[i].enabled = enable;
            }

            foreach (var interactor in _interactors)
            {
                interactor.enabled = enable;
            }
        }

        private void OnMenuToggled(bool isMenuActive) {
            SetButtonGuideActive(!isMenuActive);
        }

        private IEnumerator SetButtonActiveOnStartCoroutine()
        {
            // wait for one frame
            yield return null;
            SetButtonGuideActive(_buttonHintActive);
        }

        private void SetButtonGuideActive(bool active)
        {
            var buttonHints = GetComponentsInChildren<GuideLineController>();
            foreach (var hint in buttonHints)
            {
                Debug.Log($"Setting {hint.gameObject} to {active}");
                hint.Active = active;
            }
        }
    }
}
