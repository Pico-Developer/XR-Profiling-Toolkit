/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace DeveloperTech.XRProfilingToolkit.Interaction
{
    [Serializable]
    internal struct ControllerAssetsEntry
    {
        public PlatformSwitcher.PlatformType platformType;
        public InputActionAsset inputActionAsset;

        public Transform leftControllerRoot;
        public Transform rightControllerRoot;

        public ActionBasedController leftHandTeleport;
        public ActionBasedController rightHandTeleport;

        public SnapTurnProviderBase snapTurnProvider;
    }
    
    /// <summary>
    /// Configures the <see cref="ActionBasedController"/> based on the current platform.
    /// This enables cross-vendor controller support.
    /// </summary>
    internal class ActionBasedControllerConfigurator : MonoBehaviour
    {
        [SerializeField] private Transform _leftHandInteractionRoot;
        [SerializeField] private Transform _rightHandInteractionRoot;

        [SerializeField] private XRTransformStabilizer _leftHandStabilized;
        [SerializeField] private XRTransformStabilizer _rightHandStabilized;

        [SerializeField] private ControllerAssetsEntry[] _controllerAssets;
        
        public void ConfigureController(PlatformSwitcher.PlatformType platformType)
        {
            bool assetsEntryFound = false;
            foreach (var entry in _controllerAssets)
            {
                if (entry.platformType == platformType)
                {
                    assetsEntryFound = true;
                    if (entry.inputActionAsset)
                    {
                        entry.inputActionAsset.Enable();
                    }
                    
                    _leftHandStabilized.transform.SetParent(entry.leftControllerRoot);
                    _leftHandStabilized.targetTransform = entry.leftControllerRoot;
                    
                    _rightHandStabilized.transform.SetParent(entry.rightControllerRoot);
                    _rightHandStabilized.targetTransform = entry.rightControllerRoot;

                    entry.leftControllerRoot.gameObject.SetActive(true);
                    entry.rightControllerRoot.gameObject.SetActive(true);
                    
                    // disable ui scroll when teleport is activated to avoid a NullReferenceException
                    entry.leftHandTeleport.uiScrollAction = new InputActionProperty(null);
                    entry.rightHandTeleport.uiScrollAction = new InputActionProperty(null);

                    if (entry.snapTurnProvider != null)
                        entry.snapTurnProvider.enabled = true;

                    _leftHandInteractionRoot.SetParent(entry.leftControllerRoot, false);
                    _leftHandInteractionRoot.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    _rightHandInteractionRoot.SetParent(entry.rightControllerRoot, false);
                    _leftHandInteractionRoot.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                }
                else
                {
                    if (entry.inputActionAsset)
                    {
                        entry.inputActionAsset.Disable();
                    }


                    if (entry.snapTurnProvider != null)
                        entry.snapTurnProvider.enabled = false;

                    entry.leftControllerRoot.gameObject.SetActive(false);
                    entry.rightControllerRoot.gameObject.SetActive(false);
                }
            }
            _leftHandInteractionRoot.gameObject.SetActive(assetsEntryFound);
            _rightHandInteractionRoot.gameObject.SetActive(assetsEntryFound);
        }
    }
}