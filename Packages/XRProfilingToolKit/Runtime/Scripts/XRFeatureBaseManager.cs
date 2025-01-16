using UnityEngine;
using System.Collections.Generic;
using DeveloperTech.XRProfilingToolkit.Settings;
using UnityEngine.XR;
using System;
using DeveloperTech.XRProfilingToolkit.Automation;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Text;
#if PICO_PERFORMANCE
using Unity.XR.PXR;
#endif
using System.Collections;
using System.Threading.Tasks;

namespace DeveloperTech.XRProfilingToolkit
{
    public enum XRFeature
    {
        FFR, // Fixed Foveated Rendering
        MSAA,
        AdaptiveResolution
    }

    /// <summary>
    /// Base class to handle the core logic related to XR features, like status query and update.
    /// Status update can be triggered through <see cref="XRFeatureToggle"/> (which will be handled in the derived UI class).
    /// Set default execution order to 10 to make sure XR is initialized first.
    /// </summary>
    [DefaultExecutionOrder(10)]
    public class XRFeatureBaseManager : MonoBehaviour
    {
        [SerializeField]
        protected XRFeatureSettings _xrFeatureSettings;

        protected Dictionary<XRFeature, HashSet<XRFeature>> _incompatibleFeatureLookup = new Dictionary<XRFeature, HashSet<XRFeature>>();

        protected static bool _onetimeInitialized;

        protected static bool _shouldDisableAdaptiveResolution;
        
        protected static XRFeatureBaseManager _instance;

        /// <summary>
        /// Build a bit string based on current feature status.
        /// 0 indicates feature off, 1 indicates on.
        /// </summary>
        public static string FeatureStatusFlags
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder(4);
                XRFeature[] enumValues = (XRFeature[])Enum.GetValues(typeof(XRFeature));

                foreach (XRFeature value in enumValues)
                {
                    stringBuilder.Append(XRFeaturePreferences.GetFeatureStatus(value)? "1" : "0");
                }

                return stringBuilder.ToString();
            }
        }

        protected bool IsInProfilingSession
        {
            get
            {
                var commandRunner = FindObjectOfType<CommandRunner>();
                return commandRunner!= null && commandRunner.IsProfilingSession;
            }
        }
        
        public static void CommandToggleFeature(XRFeature feature, bool enable)
        {
            _instance.HandleFeatureToggle(feature, enable);
        }

        /// <summary>
        /// Initialize features based on preference or feature settings.
        /// This should only be called once during startup.
        /// </summary>
        protected virtual void InitializeFeatures()
        {
            bool inXRProfilingToolkitSession = IsInProfilingSession;
            foreach (XRFeature value in Enum.GetValues(typeof(XRFeature)))
            {
                bool isFeatureOn = inXRProfilingToolkitSession
                   ? XRFeaturePreferences.GetFeatureStatus(value)
                    : _xrFeatureSettings.GetFeatureStatusInManualMode(value);

                // Note: The actual toggling in UI is handled in the derived UI class.
                // Here we just set the internal state and perform other related logic like handling incompatibilities.
                HandleFeatureToggle(value, isFeatureOn);
            }
        }

        /// <summary>
        /// Handle the toggling of a feature and related logic like dealing with incompatible features.
        /// </summary>
        /// <param name="feature">The XR feature to toggle.</param>
        /// <param name="enable">Whether to enable or disable the feature.</param>
        protected void HandleFeatureToggle(XRFeature feature, bool enable)
        {
            XRProfilingToolkitLogger.Log($"Toggling feature: {feature}");
            switch (feature)
            {
                case XRFeature.MSAA:
                    ToggleMSAA(enable); break;
                case XRFeature.FFR:
                    ToggleFoveatedRendering(enable); break;
                case XRFeature.AdaptiveResolution:
                    ToggleAdaptiveResolution(enable); break;
            }
            XRFeaturePreferences.SaveFeatureStatus(feature, enable);
            LogFeatureStatus();
            if (!_incompatibleFeatureLookup.ContainsKey(feature))
            {
                return;
            }

            if (enable)
            {
                foreach (var incompatibleFeature in _incompatibleFeatureLookup[feature])
                {
                    // Disabling the actual UI component will be handled in the UI class.
                    // Here we just update the internal state logic related to incompatibilities.
                    SetIncompatibleFeatureState(incompatibleFeature, false);
                }
            }
            else
            {
                foreach (var incompatible in _incompatibleFeatureLookup[feature])
                {
                    var canBeEnabled = true;
                    foreach (var featureCheck in _incompatibleFeatureLookup[incompatible])
                    {
                        if (XRFeaturePreferences.GetFeatureStatus(featureCheck))
                        {
                            canBeEnabled = false;
                            break;
                        }
                    }
                    SetIncompatibleFeatureState(incompatible, canBeEnabled);
                }
            }
        }
        private void LogFeatureStatus()
        {
            var builder = new StringBuilder();
            builder.Append("Feature status:");
            foreach (XRFeature value in Enum.GetValues(typeof(XRFeature)))
            {
                builder.Append($"{value}:{XRFeaturePreferences.GetFeatureStatus(value)}, ");

            }
            if (builder.Length > 2)
            {
                builder.Remove(builder.Length - 2, 2);
            }
            XRProfilingToolkitLogger.Log(builder.ToString());
        }
        
        private void ToggleFoveatedRendering(bool enabled)
        {
            var foveationLevel = enabled ? _xrFeatureSettings.FoveationLevel : CommonFoveationLevel.None;
#if OCULUS_PERFORMANCE
            if (PlatformSwitcher.GetPlatform() == PlatformSwitcher.PlatformType.Quest)
            {
                OVRManager.foveatedRenderingLevel = (OVRManager.FoveatedRenderingLevel)(foveationLevel);
            }
#endif
#if PICO_PERFORMANCE
            if (PlatformSwitcher.GetPlatform() == PlatformSwitcher.PlatformType.Pico)
            {
                PXR_Manager.Instance.foveationLevel = (FoveationLevel)foveationLevel;
                _ = SetFoveationLevel(foveationLevel);
            }
#endif
        }

        private void ToggleAdaptiveResolution(bool enabled)
        {
            // doesn't allow adaptive resolution in fixed resolution mode or XRProfilingToolkit session
            if (_shouldDisableAdaptiveResolution)
            {
                XRProfilingToolkitLogger.Log("Adaptive resolution can't be toggled during b");
                return;
            }
#if OCULUS_PERFORMANCE   
            if (PlatformSwitcher.GetPlatform() == PlatformSwitcher.PlatformType.Quest)
            {
                if (OVRManager.instance)
                {
                    OVRManager.instance.enableDynamicResolution = enabled;
                }
            }
#endif
            if (enabled)
            {
                // Unlocks CPU/GPU frequencies when adaptive resolution is turned on
                SetPerformanceLevel(CommonPxrSettingsLevel.POWER_SAVINGS);
                float maxEyeTextureScale = 1.0f;
#if OCULUS_PERFORMANCE   
                if (PlatformSwitcher.GetPlatform() == PlatformSwitcher.PlatformType.Quest)
                {
                    maxEyeTextureScale = OVRManager.instance.maxDynamicResolutionScale;
                }
#endif
#if PICO_PERFORMANCE
                if (PlatformSwitcher.GetPlatform() == PlatformSwitcher.PlatformType.Pico)
                {
                    maxEyeTextureScale = PXR_Manager.Instance.maxEyeTextureScale;
                }
#endif
                // TODO (xutong.zhou): rely on PXR_Manager/OVRManager on setting eye texture scale
                if (GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset urpPipelineAsset)
                    urpPipelineAsset.renderScale = maxEyeTextureScale;
                XRSettings.eyeTextureResolutionScale = maxEyeTextureScale;
            }
            else
            {
                // TODO (xutong.zhou): rely on PXR_Manager/OVRManager on setting eye texture scale
                XRSettings.eyeTextureResolutionScale = _xrFeatureSettings.EyeTextureScale;
                XRSettings.renderViewportScale = 1.0f;

                if (GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset urpPipelineAsset)
                    urpPipelineAsset.renderScale = _xrFeatureSettings.EyeTextureScale;
                SetPerformanceLevel(_xrFeatureSettings.PerformanceLevel);
            }
#if PICO_PERFORMANCE
            if (PlatformSwitcher.GetPlatform() == PlatformSwitcher.PlatformType.Pico)
            {
                PXR_Manager.Instance.adaptiveResolution = enabled;
            }
#endif
        }

        private void ToggleMSAA(bool enabled)
        {
            var urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (urpAsset == null) return;
            urpAsset.msaaSampleCount = (int)(enabled ? _xrFeatureSettings.MSAASamples : MSAASamples.None);
        }
        
        /// <summary>
        /// Set the internal state for an incompatible feature regarding its enable/disable possibility.
        /// The actual UI update for this will be done in the derived UI class.
        /// </summary>
        /// <param name="feature">The feature whose state to set.</param>
        /// <param name="canBeEnabled">Whether the feature can be enabled or not.</param>
        protected void SetIncompatibleFeatureState(XRFeature feature, bool canBeEnabled)
        {
            // Placeholder for any internal state update related to incompatibilities.
            // The UI update will be handled in the derived class.
        }

        protected virtual void Awake()
        {
            _instance = this;
            foreach (var list in _xrFeatureSettings.IncompatibleFeatures)
            {
                for (int i = 0; i < list.features.Count; i++)
                {
                    for (int j = i + 1; j < list.features.Count; j++)
                    {
                        if (list.features[i] == list.features[j]) continue;
                        if (!_incompatibleFeatureLookup.ContainsKey(list.features[i]))
                        {
                            _incompatibleFeatureLookup.Add(list.features[i], new HashSet<XRFeature>());
                        }
                        _incompatibleFeatureLookup[list.features[i]].Add(list.features[j]);

                        if (!_incompatibleFeatureLookup.ContainsKey(list.features[j]))
                        {
                            _incompatibleFeatureLookup.Add(list.features[j], new HashSet<XRFeature>());
                        }
                        _incompatibleFeatureLookup[list.features[j]].Add(list.features[i]);
                    }
                }
            }
            if (!_onetimeInitialized)
            {
                _shouldDisableAdaptiveResolution = IsInProfilingSession || _xrFeatureSettings.EnableFixedResolution;
                if (!XRFeaturePreferences.GetFeatureStatus(XRFeature.AdaptiveResolution))
                {
                    // Set performance mode to limit GPU frequency.
                    // This is only applied when adaptive resolution is off.
                    SetPerformanceLevel(_xrFeatureSettings.PerformanceLevel);
                }
#if PICO_PERFORMANCE
                if (PlatformSwitcher.GetPlatform() == PlatformSwitcher.PlatformType.Pico)
                {
                    PXR_Manager.Instance.adaptiveResolutionPowerSetting =
                        (AdaptiveResolutionPowerSetting)_xrFeatureSettings.AdaptiveResolutionPowerSetting;
                }
#endif
                _onetimeInitialized = true;

                // Initialize feature after PrintDeviceSpecCoroutine finishes running.
                // We only runs this when adaptive resolution is disabled.
                // Since setting eye buffer scale can potentially conflicts with adaptive resolution.
                if (_shouldDisableAdaptiveResolution)
                {
                    StartCoroutine(PrintDeviceSpecCoroutine(InitializeFeatures));
                }
                else
                {
                    InitializeFeatures();
                }
            }
            else
            {
                InitializeFeatures();
            }
        }

        private IEnumerator PrintDeviceSpecCoroutine(Action callbackOnFinish)
        {
            yield return SetEyeTextureResolution(1.0f);
            int defaultEyeTexWidth = XRSettings.eyeTextureDesc.width;
            int defaultEyeTexHeight = XRSettings.eyeTextureDesc.height;

            _xrFeatureSettings.PopulateEyeTextureScale();

            yield return SetEyeTextureResolution(_xrFeatureSettings.EyeTextureScale);

            XRProfilingToolkitLogger.Log($"Device name: {SystemInfo.deviceName}, Model: {SystemInfo.deviceModel}", XRProfilingToolkitLogger.LogType.Device);
            XRProfilingToolkitLogger.Log($"GPU name: {SystemInfo.graphicsDeviceName}, GPU driver info: {SystemInfo.graphicsDeviceVersion}", XRProfilingToolkitLogger.LogType.Device);
            int eyeTexWidth = XRSettings.eyeTextureDesc.width;
            int eyeTexHeight = XRSettings.eyeTextureDesc.height;

            XRProfilingToolkitLogger.Log($"Eye buffer size: {eyeTexWidth}x{eyeTexHeight}, resolution scale: {XRSettings.eyeTextureResolutionScale}, viewport scale: {XRSettings.renderViewportScale}", XRProfilingToolkitLogger.LogType.Device);
            XRProfilingToolkitLogger.Log($"Default eye buffer size: {defaultEyeTexWidth}x{defaultEyeTexHeight}", XRProfilingToolkitLogger.LogType.Device);

            var leftEyeMatrix = Camera.main.GetStereoNonJitteredProjectionMatrix(Camera.StereoscopicEye.Left);
            var rightEyeMatrix = Camera.main.GetStereoNonJitteredProjectionMatrix(Camera.StereoscopicEye.Right);
            XRProfilingToolkitLogger.Log($"Left eye projection matrix :[{leftEyeMatrix.GetRow(0)}, {leftEyeMatrix.GetRow(1)}, {leftEyeMatrix.GetRow(2)}, {leftEyeMatrix.GetRow(3)}]");
            XRProfilingToolkitLogger.Log($"Right eye projection matrix :[{rightEyeMatrix.GetRow(0)}, {rightEyeMatrix.GetRow(1)}, {rightEyeMatrix.GetRow(2)}, {rightEyeMatrix.GetRow(3)}]");

            // fov calculation, reference: https://www.songho.ca/opengl/gl_projectionmatrix.html
            // Assume both eyes' fov values are symmetrical or the same.
            // Horizontal
            var innerFov = Mathf.Atan(1.0f / (leftEyeMatrix.m00 - leftEyeMatrix.m02)) * Mathf.Rad2Deg;
            var outerFov = Mathf.Atan(1.0f / (leftEyeMatrix.m00 + leftEyeMatrix.m02)) * Mathf.Rad2Deg;
            // Vertical
            var upperFov = Mathf.Atan(1.0f / (leftEyeMatrix.m11 - leftEyeMatrix.m12)) * Mathf.Rad2Deg;
            var lowerFov = Mathf.Atan(1.0f / (leftEyeMatrix.m11 + leftEyeMatrix.m12)) * Mathf.Rad2Deg;

            XRProfilingToolkitLogger.Log($"FOV in degree, inner: {innerFov:0.##}, outer: {outerFov:0.##}, upper: {upperFov:0.##}, lower: {lowerFov:0.##}", XRProfilingToolkitLogger.LogType.Device);

            callbackOnFinish?.Invoke();
        }

        private void SetPerformanceLevel(CommonPxrSettingsLevel performanceLevel)
        {
#if UNITY_EDITOR
            return;
#endif
            
#if OCULUS_PERFORMANCE
            if (PlatformSwitcher.GetPlatform() == PlatformSwitcher.PlatformType.Quest)
            {
                // On Quest, this does not lock to a specific GPU/CPU frequency 
                // To achieve frequency locking, use the following adb command instead
                // adb shell setprop debug.oculus.gpuLevel 
                // ref https://developer.oculus.com/resources/os-cpu-gpu-levels/
                var ovrPerformanceLevel = ConvertToOVRPerformanceLevel(performanceLevel);
                OVRManager.suggestedGpuPerfLevel = ovrPerformanceLevel;
                OVRManager.suggestedCpuPerfLevel = ovrPerformanceLevel;
                XRProfilingToolkitLogger.Log($"Setting CPU/GPU level to {ovrPerformanceLevel}");
            }
#endif
            
#if PICO_PERFORMANCE
            if (PlatformSwitcher.GetPlatform() == PlatformSwitcher.PlatformType.Pico)
            {
                PXR_Plugin.Pxr_SetPerformanceLevels(PxrPerfSettings.GPU, (PxrSettingsLevel)performanceLevel);
                PXR_Plugin.Pxr_SetPerformanceLevels(PxrPerfSettings.CPU, (PxrSettingsLevel)performanceLevel);
                XRProfilingToolkitLogger.Log($"Setting CPU/GPU level to {performanceLevel}");
            }
#endif
        }

#if OCULUS_PERFORMANCE
        private static OVRManager.ProcessorPerformanceLevel ConvertToOVRPerformanceLevel(CommonPxrSettingsLevel pxrPerformanceLevel)
        {
            switch (pxrPerformanceLevel)
            {
                case CommonPxrSettingsLevel.SUSTAINED_LOW:
                    return OVRManager.ProcessorPerformanceLevel.SustainedLow;
                case CommonPxrSettingsLevel.SUSTAINED_HIGH:
                    return OVRManager.ProcessorPerformanceLevel.SustainedHigh;
                case CommonPxrSettingsLevel.BOOST:
                    return OVRManager.ProcessorPerformanceLevel.Boost;
                case CommonPxrSettingsLevel.POWER_SAVINGS:
                    return OVRManager.ProcessorPerformanceLevel.PowerSavings;
                default:
                    return (OVRManager.ProcessorPerformanceLevel)pxrPerformanceLevel;
            }
        }
#endif
        
#if PICO_PERFORMANCE
        private async Task<bool> SetFoveationLevel(CommonFoveationLevel foveationLevel)
        {
            int num = 3;
            bool result;
            do
            {
                
                result = PXR_FoveationRendering.SetFoveationLevel((FoveationLevel)foveationLevel, false);
                await Task.Delay(1000);
            } while (!result && num-- > 0);
            return result;
        }
#endif
        
        private static IEnumerator SetEyeTextureResolution(float scale)
        {
            // Don't allow changing eye texture resolution when adaptive resolution enabled.
            if (!_shouldDisableAdaptiveResolution) 
                yield break;
            
            XRSettings.eyeTextureResolutionScale = scale;
            XRSettings.renderViewportScale = 1.0f;

            if (GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset urpPipelineAsset)
                urpPipelineAsset.renderScale = scale;

            // Wait for eye texture resolution to be populated.
            yield return null;
            yield return null;
        }
    }
}