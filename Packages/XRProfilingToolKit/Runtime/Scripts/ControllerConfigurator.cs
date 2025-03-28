/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace DeveloperTech.XRProfilingToolkit.Interaction
{
    [Serializable]
    internal struct NewControllerAssetsEntry
    {
        public PlatformSwitcher.PlatformType platformType;
        public InputActionAsset inputActionAsset;

        public Transform leftHandInteractionRoot;
        public Transform rightHandInteractionRoot;

        public Transform leftHandTeleportRoot;
        public Transform rightHandTeleportRoot;
        
        public Transform locomotionRoot;
        public InputActionReference positionInput;
        public InputActionReference rotationInput;
        public InputActionReference trackingStateInput;
    }
    
    /// <summary>
    /// Configures the <see cref="ActionBasedController"/> based on the current platform.
    /// This enables cross-vendor controller support.
    /// </summary>
    internal class ControllerConfigurator : MonoBehaviour
    {
        [SerializeField] private InputActionManager _inputActionManager;
        [SerializeField] private XRInputModalityManager _inputModalityManager;
        [SerializeField] private LocomotionProvider _locomotionProvider;
        
        [SerializeField] private NewControllerAssetsEntry[] _controllerAssets;
        
        public void ConfigureController(PlatformSwitcher.PlatformType platformType)
        {
            foreach (var entry in _controllerAssets)
            {
                if (entry.platformType == platformType)
                {
                    if (entry.inputActionAsset)
                    {
                        entry.inputActionAsset.Enable();
                    }

                    _inputModalityManager.leftController = entry.leftHandInteractionRoot.gameObject;
                    _inputModalityManager.rightController = entry.rightHandInteractionRoot.gameObject;
                    
                    if (!_inputActionManager.actionAssets.Contains(entry.inputActionAsset))
                    {
                        _inputActionManager.actionAssets.Add(entry.inputActionAsset);
                    }

                    var camera = GetComponent<XROrigin>().Camera;
                    var posDriver = camera.GetComponent<TrackedPoseDriver>();
                    posDriver.positionInput = new InputActionProperty(entry.positionInput);
                    posDriver.rotationInput = new InputActionProperty(entry.rotationInput);
                    posDriver.trackingStateInput = new InputActionProperty(entry.trackingStateInput);
                    entry.leftHandInteractionRoot.gameObject.SetActive(true);
                    entry.rightHandInteractionRoot.gameObject.SetActive(true);
                    entry.leftHandTeleportRoot.gameObject.SetActive(true);
                    entry.rightHandTeleportRoot.gameObject.SetActive(true);
                    entry.locomotionRoot.gameObject.SetActive(true);
                }
                else
                {
                    if (entry.inputActionAsset)
                    {
                        entry.inputActionAsset.Disable();
                    }
                    entry.leftHandInteractionRoot.gameObject.SetActive(false);
                    entry.rightHandInteractionRoot.gameObject.SetActive(false);
                    entry.leftHandTeleportRoot.gameObject.SetActive(false);
                    entry.rightHandTeleportRoot.gameObject.SetActive(false);
                    entry.locomotionRoot.gameObject.SetActive(false);
                } }
        }
    }
}