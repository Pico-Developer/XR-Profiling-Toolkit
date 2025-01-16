/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using DeveloperTech.XRProfilingToolkit.Interaction;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DeveloperTech.XRProfilingToolkit.Automation
{
    /// <summary>
    /// Automation command to teleport to a target teleportation anchor location
    /// </summary>
    [Serializable]
    public class CommandTeleport : CommandBase
    {
        [SerializeField]
        [Tooltip("Id of the area to teleport to, assigned by TeleportationAnchorManager")]
        public int targetId;

        private const float CommandWaitTimeInSeconds = 0.5f;

        public override IEnumerator Run()
        {
            TeleportationAnchorManager anchorManager = Object.FindObjectOfType<TeleportationAnchorManager>(true);
            if (anchorManager == null)
            {
                XRProfilingToolkitLogger.Log($"{GetType().Name} Unable to teleport, scene is missing ${nameof(TeleportationAnchorManager)}");
            }
            
            XRProfilingToolkitLogger.Log($"{GetType().Name} Teleporting to anchor with id {targetId}");
            anchorManager.TeleportToAnchor(targetId);

            yield return new WaitForSeconds(CommandWaitTimeInSeconds);
        }

        public override void Pause()
        {
            // cannot be paused
        }

        public override void Resume()
        {
            // cannot be resumed
        }
    }

}