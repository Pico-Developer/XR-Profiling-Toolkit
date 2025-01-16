using UnityEngine;
using System.Collections.Generic;
using DeveloperTech.XRProfilingToolkit.UI;

namespace DeveloperTech.XRProfilingToolkit
{
    public class XRFeatureUIManager : XRFeatureBaseManager
    {
        [SerializeField]
        private Transform _featureUIRoot;
        [SerializeField]
        private XRFeatureToggle[] _featureToggles;
        private Dictionary<XRFeature, XRFeatureToggle> _featureTogglesLookup = new Dictionary<XRFeature, XRFeatureToggle>();

        protected override void Awake()
        {
            
            if (_featureToggles == null || _featureToggles.Length == 0)
            {
                _featureToggles = _featureUIRoot.GetComponentsInChildren<XRFeatureToggle>(true);
            }
            foreach (var component in _featureToggles)
            {
                _featureTogglesLookup.Add(component._feature, component);
            }
            base.Awake();
        }

        protected override void InitializeFeatures()
        {
            base.InitializeFeatures();
            foreach (var component in _featureTogglesLookup.Values)
            {
                if (component._feature == XRFeature.AdaptiveResolution && _shouldDisableAdaptiveResolution)
                {
                    // doesn't allow adaptive resolution in fixed resolution mode
                    component.ToggleFeature(false);
                    component.EnableToggle(false);
                    continue;
                }
                component.ToggleFeature(XRFeaturePreferences.GetFeatureStatus(component._feature));
                component.AddListener(HandleFeatureToggle);
            }
        }

        public void ToggleFeature(XRFeature feature, bool enable)
        {
            if (_featureTogglesLookup.TryGetValue(feature, out XRFeatureToggle featureToggle))
            {
                featureToggle.ToggleFeature(!XRFeaturePreferences.GetFeatureStatus(feature));
            }
            base.HandleFeatureToggle(feature, !XRFeaturePreferences.GetFeatureStatus(feature));
        }
    }
}