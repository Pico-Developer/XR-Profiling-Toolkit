/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using TMPro;
using UnityEngine;

namespace DeveloperTech.XRProfilingToolkit
{
    /// <summary>
    /// Controller for the main menu UI and scene selection views.
    /// </summary>
    public class MainMenuSceneUIController : MonoBehaviour
    {
        [SerializeField] private SceneView[] sceneViews;
        [SerializeField] private TMP_Text sceneDescriptionText;
        [SerializeField] private string defaultDescription = "Choose a scene to enter, hover to see the details.";
    
        private int _currentSelectedIndex = -1;
        
        private void Start()
        {
            sceneDescriptionText.text = defaultDescription;
    
            for (var i = 0; i < sceneViews.Length; i++)
            {
                sceneViews[i].SceneIndex = i;
            }
    
            UnHighlightAll();
        }
    
        private void OnEnable()
        {
            foreach (var sceneView in sceneViews)
            {
                sceneView.OnHoverEnter += HighlightSceneView;
                sceneView.OnHoverExit += UnHighlightSceneView;
            }
        }
        
        private void OnDisable()
        {
            foreach (var sceneView in sceneViews)
            {
                sceneView.OnHoverEnter -= HighlightSceneView;
                sceneView.OnHoverExit -= UnHighlightSceneView;
            }
        }
    
        private void HighlightSceneView(int index)
        {
            if (_currentSelectedIndex == index || 
                index > sceneViews.Length || index < 0)
                return;
    
            _currentSelectedIndex = index;
    
            sceneViews[_currentSelectedIndex].SetHighlight(false);
            sceneViews[index].SetHighlight(true);
        }
        
        private void UnHighlightSceneView(int index)
        {
            if (_currentSelectedIndex != index || 
                index > sceneViews.Length || index < 0)
                return;
    
            sceneViews[_currentSelectedIndex].SetHighlight(false);
            _currentSelectedIndex = -1;
        }
    
        private void UnHighlightAll()
        {
            foreach (var sceneButton in sceneViews)
            {
                sceneButton.SetHighlight(false);
            }
    
            _currentSelectedIndex = -1;
        }
    
        public static void LoadScene(int index)
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(index + 1);
        }
    }
}

