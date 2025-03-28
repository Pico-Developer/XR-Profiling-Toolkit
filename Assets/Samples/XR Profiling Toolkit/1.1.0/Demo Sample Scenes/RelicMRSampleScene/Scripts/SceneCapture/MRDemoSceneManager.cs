/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;
using System;
using System.Linq;
using System.Threading.Tasks;
using DeveloperTech.XRProfilingToolkit;
using MRDemoSampleScene.Runtime.Utils;


public class MRDemoSceneManager : MonoBehaviour
{
    public static MRDemoSceneManager Instance;
    
#if PICO_PERFORMANCE
    public SceneManagerPICO sceneManagerPico;
#endif
#if OCULUS_PERFORMANCE
    public SceneManagerMeta sceneManagerMeta;
#endif
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
    
    // Start is called before the first frame update
    void Start()
    {
#if PICO_PERFORMANCE
        if (PlatformSwitcher.GetPlatform() == PlatformSwitcher.PlatformType.Pico||
            PlatformSwitcher.GetPlatform() == PlatformSwitcher.PlatformType.Unknown)
        {
            sceneManagerPico.enabled = true;
        }
#endif
#if OCULUS_PERFORMANCE
        if (PlatformSwitcher.GetPlatform() == PlatformSwitcher.PlatformType.Quest)
        {
            sceneManagerMeta.enabled = true;
        }
#endif
    }

    public bool SceneCaptureFinished()
    {
#if PICO_PERFORMANCE
        if (PlatformSwitcher.GetPlatform() == PlatformSwitcher.PlatformType.Pico ||
            PlatformSwitcher.GetPlatform() == PlatformSwitcher.PlatformType.Unknown)
        {
            return sceneManagerPico.IsSceneCaptureFinished;
        }
#endif
#if OCULUS_PERFORMANCE
        //return sceneManagerMeta;
        if (PlatformSwitcher.GetPlatform() == PlatformSwitcher.PlatformType.Quest)
        {
            return sceneManagerMeta.IsSceneCaptureFinished;
        }
#endif
        return false;
    }
    
}
