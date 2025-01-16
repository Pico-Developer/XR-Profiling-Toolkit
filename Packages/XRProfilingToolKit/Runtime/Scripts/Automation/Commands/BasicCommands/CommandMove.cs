/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SpatialTracking;

namespace DeveloperTech.XRProfilingToolkit.Automation
{
    /// <summary>
    /// Deprecated in favor of <see cref="CommandTeleport"/>.
    /// Automation command to move to a destination position in the scene.
    /// </summary>
    [Serializable]
    public class CommandMove : CommandBase
    {
        public float x, y, z;
        public float speed;
        
        private TrackedPoseDriver _trackedPoseDriver;
        private Transform _cameraOffset;

        private bool _paused = false;

        private Vector3 _destinationPosition;

        private bool DestinationReached => _cameraOffset != null && Vector3.SqrMagnitude(_cameraOffset.transform.position - _destinationPosition) <= 1e-4f;
        
        public override IEnumerator Run()
        {
            _destinationPosition.x = x;
            _destinationPosition.y = y;
            _destinationPosition.z = z;

            _paused = false;
            _trackedPoseDriver ??= Camera.main.GetComponent<TrackedPoseDriver>();
            _cameraOffset ??= Camera.main.transform.parent;

            Vector3 currentVelocity = Vector3.zero;

            XRProfilingToolkitLogger.Log($"{GetType().Name} Moving to {_destinationPosition}");
            while (!DestinationReached)
            {
                if (!_paused)
                {
                    _cameraOffset.position = Vector3.SmoothDamp(_cameraOffset.position, _destinationPosition, ref currentVelocity, 0.2f, speed);
                }
                yield return null;
            }
            XRProfilingToolkitLogger.Log($"{GetType().Name} Reached destination {_destinationPosition}");
        }

        public override void Pause()
        {
            if (_trackedPoseDriver != null)
            {
                _trackedPoseDriver.enabled = _paused;
            }
        }

        public override void Resume()
        {
            if (_trackedPoseDriver != null)
            {
                _trackedPoseDriver.enabled = _paused;
            }
        }
    }
}