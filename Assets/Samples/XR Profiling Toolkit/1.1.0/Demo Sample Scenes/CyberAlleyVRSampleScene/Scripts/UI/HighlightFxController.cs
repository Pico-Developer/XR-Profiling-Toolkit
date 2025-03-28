/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace DeveloperTech.XRProfilingToolkit.UI
{
    /// <summary>
    /// Highlights an object with special fx
    /// Used to hint interactable objects in the scene
    /// Special fx can be toggled
    /// </summary>
    public class HighlightFxController : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;

        [SerializeField] private Material _lightWipingOnMaterial;
        [SerializeField] private Material _lightWipingOffMaterial;

        [SerializeField] private Material _highlightMaterial;

        private List<Material> _materialList = new();

        private int _highlightLayer;

        private void Awake()
        {
            _highlightLayer = LayerMask.NameToLayer("Highlighted");
        }

        public void ToggleLightWiping(bool enable)
        {
            if (enable)
            {
                _meshRenderer.GetSharedMaterials(_materialList);
                if (_materialList.Contains(_lightWipingOffMaterial))
                    _materialList.Remove(_lightWipingOffMaterial);
                _meshRenderer.SetSharedMaterials(_materialList);

                _meshRenderer.AddMaterial(_lightWipingOnMaterial);
            }
            else
            {
                _meshRenderer.GetSharedMaterials(_materialList);
                if (_materialList.Contains(_lightWipingOnMaterial))
                    _materialList.Remove(_lightWipingOnMaterial);
                _meshRenderer.SetSharedMaterials(_materialList);

                _meshRenderer.AddMaterial(_lightWipingOffMaterial);
            }
        }

        public void ToggleHighlight(bool enable)
        {
            _meshRenderer.GetSharedMaterials(_materialList);
            if (enable)
            {
                if (!_materialList.Contains(_highlightMaterial))
                    _meshRenderer.AddMaterial(_highlightMaterial);
            }
            else
            {
                if (_materialList.Contains(_highlightMaterial))
                {
                    _materialList.Remove(_highlightMaterial);
                    _meshRenderer.SetSharedMaterials(_materialList);
                }
            }
        }

        [ContextMenu("Toggle highlight")]
        public void ToggleHighlightFromMenu()
        {
            _meshRenderer.GetSharedMaterials(_materialList);
            ToggleHighlight(!_materialList.Contains(_highlightMaterial));
        }
    }
}
