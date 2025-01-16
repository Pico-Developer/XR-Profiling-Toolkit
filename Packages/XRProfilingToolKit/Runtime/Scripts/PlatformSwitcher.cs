/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using DeveloperTech.XRProfilingToolkit.Interaction;
#if PICO_PERFORMANCE
using Unity.XR.PXR;
#endif
using UnityEngine;
using UnityEngine.XR.Management;

namespace DeveloperTech.XRProfilingToolkit
{
    /// <summary>
    /// Detects the headset model and configures XR loader/manager accordingly.
    /// </summary>
    /// <remarks>
    /// This is necessary for cross vendor/platform scene setup and should always exist in the Scene.
    /// </remarks>
    [DefaultExecutionOrder(-1000)]
    public class PlatformSwitcher : MonoBehaviour
    {
        /// <summary>
        /// The type of supported device platforms. Extend this to add extra platforms.
        /// </summary>
        public enum PlatformType
        {
            Quest,
            Pico,
            Unknown,
        }

        private static PlatformType Platform;
        public static PlatformType GetPlatform()
        {
            return Platform;
        }

        private static readonly Dictionary<PlatformType, string> PlatformToLoaderName = new Dictionary<PlatformType, string>
        {
            { PlatformType.Quest, "OculusLoader"},
            { PlatformType.Pico, "PXR_Loader"}, 
            { PlatformType.Unknown, "PXR_Loader"},
        };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ReadCommandLineArgs()
        {
            string[] args = System.Environment.GetCommandLineArgs();
            
            // read the command line arguments, platform type can be overriden through command line
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "platform_pxr")
                {
                    Platform = PlatformType.Pico;
                }
                else if (args[i] == "platform_ovr")
                {
                    Platform = PlatformType.Quest;
                }
            }

            if (Platform == PlatformType.Unknown)
            {
                if (SystemInfo.deviceModel == "Oculus Quest")
                {
                    Platform = PlatformType.Quest;
                }
                else if (SystemInfo.deviceModel.Contains("Pico"))
                {
                    Platform = PlatformType.Pico;
                }
            }
            if (XRGeneralSettings.Instance == null)
            {
                Platform = PlatformType.Pico;
            }
            else if (XRGeneralSettings.Instance.Manager.activeLoaders.Count == 0)
            {
#if UNITY_EDITOR
                Platform = PlatformType.Pico;
                //Platform = PlatformType.Quest;
#else
                Debug.LogError("Please use at least one XR Plugin to run XRProfilingToolkit!");
#endif
            }
            else if (XRGeneralSettings.Instance.Manager.activeLoaders.Count == 1)
            {
                if (XRGeneralSettings.Instance.Manager.activeLoaders[0].name == "PXR_Loader")
                {
                    Platform = PlatformType.Pico;
                }
                else if (XRGeneralSettings.Instance.Manager.activeLoaders[0].name == "OculusLoader")
                {
                    Platform = PlatformType.Quest;
                }
            }
            else
            {
                string targetLoaderName = PlatformToLoaderName[Platform];
                List<XRLoader> invalidLoaders = new List<XRLoader>();
                for (int i = 0; i < XRGeneralSettings.Instance.Manager.activeLoaders.Count; i++)
                {
                    if (XRGeneralSettings.Instance.Manager.activeLoaders[i].name != targetLoaderName)
                    {
                        invalidLoaders.Add(XRGeneralSettings.Instance.Manager.activeLoaders[i]);
                    }
                }

                for (int i = 0; i < invalidLoaders.Count; i++)
                {
                    XRGeneralSettings.Instance.Manager.TryRemoveLoader(invalidLoaders[i]);
                    invalidLoaders[i].Deinitialize();
                    Destroy(invalidLoaders[i]);
                    XRProfilingToolkitLogger.Log($"Removing XR loader: {invalidLoaders[i].name}");
                }
            }

            XRProfilingToolkitLogger.Log($"Platform type is {Platform}");

            bool vfxGraphSupported = SystemInfo.supportsComputeShaders && SystemInfo.maxComputeBufferInputsVertex >= 5;
            XRProfilingToolkitLogger.Log($"Platform VFX graph support: {vfxGraphSupported}");
        }
        private void ConfigurePicoRelatedObjects()
        {
#if PICO_PERFORMANCE
            var pxrManager = FindObjectOfType<PXR_Manager>(true);
            if (pxrManager!= null) pxrManager.enabled = Platform == PlatformType.Pico;
#endif
        }

        private void ConfigureOculusRelatedObjects()
        {
#if OCULUS_PERFORMANCE
            var ovrManager = FindObjectOfType<OVRManager>(true);
            if (ovrManager!= null) ovrManager.enabled = Platform == PlatformType.Quest;
#endif
        }
        private void Awake()
        {
            ConfigurePicoRelatedObjects();
            ConfigureOculusRelatedObjects();
            var controllerConfigurator = FindObjectOfType<ActionBasedControllerConfigurator>(true);
            if (controllerConfigurator != null)
                controllerConfigurator.ConfigureController(Platform);
        }
    }
}