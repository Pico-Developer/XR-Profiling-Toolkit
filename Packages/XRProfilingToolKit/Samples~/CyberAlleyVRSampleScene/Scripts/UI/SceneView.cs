/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

namespace DeveloperTech.XRProfilingToolkit
{
    /// <summary>
    /// Manages the visual representation and behavior of scene selection in a user interface, including hover effects.
    /// </summary>
    public class SceneView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>
        /// Gets or sets the index of the scene that this view represents.
        /// </summary>
        public int SceneIndex { get; set; }
        
        /// <summary>
        /// Event triggered when the pointer enters the scene view area.
        /// </summary>
        public Action<int> OnHoverEnter { get; set; }
        
        /// <summary>
        /// Event triggered when the pointer exits the scene view area.
        /// </summary>
        public Action<int> OnHoverExit { get; set; }
        
        [SerializeField] private Image sceneImage;
        [SerializeField] private Image highlightImage;
        [SerializeField] private float normalScale = 0.8f;
        [SerializeField] private float highlightScale = 1f;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color highlightColor = Color.yellow;
        [SerializeField] private float transitionTime = 0.1f;
        [SerializeField] private GameObject sceneDescription;
        private Coroutine _transitionCoroutine;
        
        /// <summary>
        /// Handles the pointer enter event to trigger visual changes and events.
        /// </summary>
        /// <param name="eventData">Event data associated with the pointer enter event.</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            OnHoverEnter?.Invoke(SceneIndex);
        }

        /// <summary>
        /// Handles the pointer exit event to revert visual changes and events.
        /// </summary>
        /// <param name="eventData">Event data associated with the pointer exit event.</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            OnHoverExit?.Invoke(SceneIndex);
        }

        /// <summary>
        /// Sets the visual highlight state of the scene view.
        /// </summary>
        /// <param name="highlight">Whether to highlight the scene view.</param>
        public void SetHighlight(bool highlight)
        {
            if (_transitionCoroutine != null)
            {
                StopCoroutine(_transitionCoroutine);
            }
            _transitionCoroutine = StartCoroutine(AnimateScaleAndColor(highlight));
        }

        /// <summary>
        /// Animates the scaling and color transition of the scene view.
        /// </summary>
        /// <param name="highlight">Whether the animation should highlight or normalize the view.</param>
        /// <returns>An enumerator needed for coroutine continuation.</returns>
        private IEnumerator AnimateScaleAndColor(bool highlight)
        {
            if (highlight == false && highlightImage != null)
                highlightImage.gameObject.SetActive(false);
            
            float time = 0;
            var startScale = transform.localScale;
            var endScale = Vector3.one * (highlight ? highlightScale : normalScale);
            var startColor = sceneImage.color;
            var endColor = highlight ? highlightColor : normalColor;
            
            sceneDescription.SetActive(highlight);

            while (time < transitionTime)
            {
                transform.localScale = Vector3.Lerp(startScale, endScale, time / transitionTime);
                sceneImage.color = Color.Lerp(startColor, endColor, time / transitionTime);
                time += Time.deltaTime;
                yield return null;
            }

            // Ensure the final state is exactly what we want
            transform.localScale = endScale;
            sceneImage.color = endColor;
            
            // Toggle highlight image at the end if necessary
            if (highlight && highlightImage != null)
                highlightImage.gameObject.SetActive(true);
        }
    }
}
