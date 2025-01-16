/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using DeveloperTech.XRProfilingToolkit.Automation;
using DeveloperTech.XRProfilingToolkit.UI;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace DeveloperTech.XRProfilingToolkit.Interaction
{
    /// <summary>
    /// Handles the visual update of <see cref="TeleportationAnchor"/> in the scene
    /// Also handles teleportation requests from <see cref="CommandTeleport"/>
    /// </summary>
    public class TeleportationAnchorManager : MonoBehaviour
    {
        [SerializeField] private GameObject _teleportationAnchorRoot;

        [SerializeField] private TeleportationAnchor[] _teleportationInteractables;

        [SerializeField] private TeleportationProvider _teleportationProvider;

        [SerializeField] private XRRayInteractor[] _teleportInteractors;

        [SerializeField]
        [Tooltip("Visual of Teleportation anchor is disabled when its distance to camera is smaller than given value")]
        private float _disableVisualDistance = 2f;

        private List<TeleportationAnchorVisual> _anchorVisuals;

        private CommandRunner _commandRunner;

        private XROrigin _xrOrigin;

        private void Start()
        {
            if (_teleportationProvider == null)
            {
                _teleportationProvider = FindObjectOfType<TeleportationProvider>(true);
            }

            _xrOrigin = FindObjectOfType<XROrigin>(false);

            _teleportationInteractables = _teleportationAnchorRoot.GetComponentsInChildren<TeleportationAnchor>(true);
            _anchorVisuals = new List<TeleportationAnchorVisual>(_teleportationInteractables.Length);
            for (int i = 0; i < _teleportationInteractables.Length; i++)
            {
                var visual = _teleportationInteractables[i].GetComponentInChildren<TeleportationAnchorVisual>(true);
                if (visual != null)
                {
                    visual.SetIndexText(i);
                }
                _anchorVisuals.Add(visual);
            }
            
            _commandRunner = FindObjectOfType<CommandRunner>(true);
            if (_commandRunner != null)
            {
                _commandRunner.OnStatusChanged += ToggleAnchorVisual;
                var status = _commandRunner.IsRunning ? CommandRunner.Status.Running : CommandRunner.Status.Idle;
                ToggleAnchorVisual(status);
            }

            if (_teleportationProvider != null)
            {
                _teleportationProvider.endLocomotion += OnLocomotionEnd;
            }

            OnRayInteractorChanged(null);
        }

        private void OnDestroy()
        {
            if (_commandRunner != null)
            {
                _commandRunner.OnStatusChanged -= ToggleAnchorVisual;
            }
            
            if (_teleportationProvider != null)
            {
                _teleportationProvider.endLocomotion -= OnLocomotionEnd;
            }
        }

        /// <summary>
        /// Turn on the teleportation anchor visual when teleportation is activated
        /// </summary>
        public void OnRayInteractorChanged(IXRRayProvider _)
        {
            StartCoroutine(ToggleAnchorVisualCoroutine());
        }

        /// <summary>
        /// Teleport the anchor associated with the id
        /// Reference the teleportation anchor prefab in the scene to find out which id corresponds to which anchor
        /// </summary>
        public void TeleportToAnchor(int anchorId)
        {
            if (_teleportationProvider == null)
            {
                XRProfilingToolkitLogger.Log("Teleportation provider not setup in the scene, can't teleport.");
            }
            if (anchorId < 0 || anchorId >= _teleportationInteractables.Length)
            {
                XRProfilingToolkitLogger.Log("Invalid teleport destination.");
                return;
            }

            _teleportationProvider.QueueTeleportRequest(GenerateTeleportRequest(_teleportationInteractables[anchorId]));
        }

        private IEnumerator ToggleAnchorVisualCoroutine()
        {
            // wait one frame for Teleport Interactor status to be updated
            yield return null;

            bool isTeleportActive = false;
            foreach (var interactor in _teleportInteractors)
            {
                if (interactor.gameObject.activeInHierarchy)
                {
                    isTeleportActive = true;
                    break;
                }
            }

            foreach (var anchor in _teleportationInteractables)
            {
                anchor.gameObject.SetActive(isTeleportActive);
            }
        }

        private void OnLocomotionEnd(LocomotionSystem system)
        {
            ToggleAnchorVisual(system.xrOrigin);
        }

        private void ToggleAnchorVisual(XROrigin xrOrigin)
        {
            foreach (var visual in _anchorVisuals)
            {
                var cameraToAnchor = visual.transform.position - xrOrigin.transform.position;
                cameraToAnchor.y = 0f;
                visual.ToggleText(cameraToAnchor.sqrMagnitude >  _disableVisualDistance * _disableVisualDistance);
            }
        }
        
        private void ToggleAnchorVisual(CommandRunner.Status status)
        {
            bool enable = status != CommandRunner.Status.Running;
            foreach (var visual in _anchorVisuals)
            {
                if (visual != null)
                {
                    visual.gameObject.SetActive(enable);
                }
            }

            if (enable)
            {
                ToggleAnchorVisual(_xrOrigin);
            }
        }
        
        /// <summary>
        /// Generate <see cref="TeleportRequest"/> to teleport to the given anchor
        /// </summary>
        /// <remarks>
        /// The is reference the implementations of <see cref="TeleportationAnchor.GenerateTeleportRequest"/>
        /// as well as <see cref="BaseTeleportationInteractable.UpdateTeleportRequestRotation"/>>
        /// </remarks>
        private static TeleportRequest GenerateTeleportRequest(BaseTeleportationInteractable anchor)
        {
            Transform anchorTransform = anchor.transform;
            TeleportRequest request = new TeleportRequest
            {
                destinationPosition = anchorTransform.position,
                destinationRotation = anchorTransform.rotation,
                matchOrientation = anchor.matchOrientation,
                requestTime = Time.time
            };

            switch (request.matchOrientation)
            {
                case MatchOrientation.TargetUp:
                    request.destinationRotation = Quaternion.LookRotation(anchorTransform.forward, anchorTransform.up);
                    request.matchOrientation = MatchOrientation.TargetUpAndForward;
                    break;
                case MatchOrientation.WorldSpaceUp:
                    request.destinationRotation = Quaternion.LookRotation(anchorTransform.forward, Vector3.up);
                    request.matchOrientation = MatchOrientation.TargetUpAndForward;
                    break;
            }

            return request;
        }
    }
}