/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

namespace DeveloperTech.XRProfilingToolkit.Settings
{
    public enum CommonFoveationLevel
    {
        None = -1,
        Low,
        Med,
        High,
        TopHigh
    }
    public enum CommonPxrSettingsLevel {
        POWER_SAVINGS = 0,
        SUSTAINED_LOW = 1,
        SUSTAINED_HIGH = 3,
        BOOST = 5,
    }
    public enum CommonAdaptiveResolutionPowerSetting
    {
        HIGH_QUALITY, // performance factor = 0.9
        BALANCED, // performance factor = 0.8
        BATTERY_SAVING // performance factor = 0.7
    }
    /// <summary>
    /// Holds configurations of the XR features
    /// </summary>
    [CreateAssetMenu(fileName = "XRFeatureSettings", menuName = "XRProfilingToolkit/XR Feature Settings")]
    public class XRFeatureSettings : ScriptableObject
    {
        /// <summary>
        /// List of PXR features that are not compatible
        /// </summary>
        [Serializable]
        public struct FeatureIncompatibility
        {
            public List<XRFeature> features;
        }

        [Serializable]
        public struct FeatureStatus
        {
            public XRFeature feature;
            public bool isOn;
        }

        [SerializeField]
        [Tooltip("Lists of the features that are not compatible with each other")]
        private List<FeatureIncompatibility> _incompatibleFeatures;

        [SerializeField]
        [Tooltip("Foveation level when FFR is turned on")]
        private CommonFoveationLevel _foveationLevel = CommonFoveationLevel.Med;

        [SerializeField]
        [Tooltip("MSAA sample count when MSAA is on")]
        private MSAASamples _MSAASamples = MSAASamples.MSAA4x;
        
        [SerializeField]
        [Tooltip("Whether rendering feature is turned on in manual mode")]
        private FeatureStatus[] _featureStatusInManualMode;

        [SerializeField]
        [Tooltip("If enabled, build on all devices will have the same eye buffer resolution, if disabled, will use the default eye buffer resolution")]
        private bool _enableFixedResolutuon;
        
        [SerializeField]
        [Tooltip("Target eye buffer resolution")]
        private Vector2Int _targetResolution = new Vector2Int(1504, 1504);

        [SerializeField]
        [Tooltip("CPU/GPU performance level, this will be unlocked when adaptive resolution is turned on")]
        private CommonPxrSettingsLevel _performanceLevel = CommonPxrSettingsLevel.SUSTAINED_HIGH;

        [SerializeField] [Tooltip("Adaptive Resolution power settings")]
        private CommonAdaptiveResolutionPowerSetting _adaptiveResolutionPowerSetting =
            CommonAdaptiveResolutionPowerSetting.HIGH_QUALITY;

        private float _eyeTextureScale = 1.0f;

        public MSAASamples MSAASamples => _MSAASamples;
        public CommonFoveationLevel FoveationLevel => _foveationLevel;

        public CommonPxrSettingsLevel PerformanceLevel => _performanceLevel;

        public CommonAdaptiveResolutionPowerSetting AdaptiveResolutionPowerSetting => _adaptiveResolutionPowerSetting;

        public float EyeTextureScale =>
#if UNITY_EDITOR
            1.0f;
#else
            _eyeTextureScale;
#endif

        private bool _commandLineRead = false;
        internal bool EnableFixedResolution
        {
            get
            {
                if (!_commandLineRead)
                {
                    string[] args = Environment.GetCommandLineArgs();
                    foreach (var arg in args)
                    {
                        if (arg == "fixed_res")
                        {
                            _enableFixedResolutuon = true;
                        }
                    }

                    _commandLineRead = true;
                }

                return _enableFixedResolutuon;
            }
        }

        public List<FeatureIncompatibility> IncompatibleFeatures => _incompatibleFeatures;
        
        /// <summary>
        /// Gets whether a rendering feature is turned on in manual mode.
        /// </summary>
        /// <param name="feature">the rendering feature to check </param>
        /// <returns> <c>true</c> if the status is found and turned on in manual mode,
        /// if the status is not defined, return <c>false</c>.</returns>
        public bool GetFeatureStatusInManualMode(XRFeature feature)
        {
            foreach (FeatureStatus featureStatus in _featureStatusInManualMode)
            {
                if (featureStatus.feature == feature)
                {
                    return featureStatus.isOn;
                }
            }

            return false;
        }

        /// <summary>
        /// Populates eye texture resolution scale.
        /// </summary>
        /// <remarks> 
        /// Call this function first before getting EyeTextureScale when fixed resolution is enabled.
        /// The eye texture scale will be adjusted in order to match the target resolution in terms of pixel count.
        /// </remarks>
        public void PopulateEyeTextureScale()
        {
            if (_targetResolution.x <= 0 || _targetResolution.y <= 0 || !EnableFixedResolution)
            {
                _eyeTextureScale = 1.0f;
                return;
            }
            var eyeTexWidth = XRSettings.eyeTextureDesc.width;
            var eyeTexHeight = XRSettings.eyeTextureDesc.height;

            var defaultEyeTexWidth = (int)(eyeTexWidth / XRSettings.eyeTextureResolutionScale);
            var defaultEyeTexHeight = (int)(eyeTexHeight / XRSettings.eyeTextureResolutionScale);
            _eyeTextureScale = Mathf.Sqrt(((float)(_targetResolution.x * _targetResolution.y)) / (defaultEyeTexWidth * defaultEyeTexHeight));
            XRProfilingToolkitLogger.Log($"Eyebuffer scale populated : {_eyeTextureScale}");
        }
    }
}