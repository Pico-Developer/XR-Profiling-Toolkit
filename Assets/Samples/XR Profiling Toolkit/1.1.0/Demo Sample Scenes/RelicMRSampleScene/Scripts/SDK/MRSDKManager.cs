/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeveloperTech.XRProfilingToolkit;
using MRDemoSampleScene.Runtime.Data;
using Unity.XR.PXR;
using UnityEngine;

namespace MRDemoSampleScene.Runtime.SDK
{
    public class MRSDKManager : MonoBehaviour
    {
        public static MRSDKManager Instance;
        
        private readonly string TAG = nameof(MRSDKManager);
        
        #region Task and flag
        
        ulong uuid = 0;

        #endregion
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        

        public async Task<AnchorData> CreateAnchor(Transform transform)
        {
            Debug.unityLogger.Log(TAG,$"CreateAnchor0:");
            var anchorData = new AnchorData(uuid, new Guid());
#if UNITY_EDITOR
            uuid++;
            Debug.unityLogger.Log(TAG, $"Create Anchor, uuid: {anchorData.Uuid}, handle: {anchorData.Handle}");
            return anchorData;
#endif
#if PICO_PERFORMANCE
            if (PlatformSwitcher.GetPlatform() == PlatformSwitcher.PlatformType.Pico)
            {
                var result = await PXR_MixedReality.CreateSpatialAnchorAsync(transform.position, transform.rotation);
                if (result.result == PxrResult.SUCCESS)
                {
                    anchorData = new AnchorData(result.anchorHandle, result.uuid);
                    Debug.unityLogger.Log(TAG, $"Create Anchor, uuid: {anchorData.Uuid}, handle: {anchorData.Handle}");
                    return anchorData;
                }
            }
#endif
#if OCULUS_PERFORMANCE
            Debug.unityLogger.Log(TAG,$"CreateAnchor5:");
            if (PlatformSwitcher.GetPlatform() == PlatformSwitcher.PlatformType.Quest)
            {
                Debug.unityLogger.Log(TAG,$"CreateAnchor1:");
                var anchor = transform.gameObject.AddComponent<OVRSpatialAnchor>();
                Debug.unityLogger.Log(TAG,$"CreateAnchor2:"+uuid+"|"+anchor.Uuid);
                // Wait for the async creation
                anchorData = new AnchorData(uuid, anchor.Uuid);
                Debug.unityLogger.Log(TAG,$"CreateAnchor3:"+uuid+"|"+anchor.Uuid);
                uuid++;
                Debug.unityLogger.Log(TAG,$"CreateAnchor4 {uuid} - {anchor.Uuid}");
                return anchorData;
            }
#endif
            return null;
        }

        public async Task DeleteAnchor(AnchorData anchorData)
        {
#if PICO_PERFORMANCE
            var result = PXR_MixedReality.DestroyAnchor(anchorData.Handle);
            if (result == PxrResult.SUCCESS)
            {
                Debug.unityLogger.Log(TAG, "PXR_MRSample Destroy spatial anchor succeed with anchorHandle " + anchorData.Handle);
            }
            else
            {
                Debug.unityLogger.Log(TAG, "PXR_MRSample Destroy spatial anchor failed with result:" + result);
            }
            Debug.unityLogger.Log(TAG, $"Start DeleteAnchor anchor "+ anchorData.Handle);

            await PXR_MixedReality.UnPersistSpatialAnchorAsync(anchorData.Handle);
            Debug.unityLogger.Log(TAG, $"Delete Anchor, uuid: {anchorData.Uuid}, handle: {anchorData.Handle}");
#endif
        }
    }
}