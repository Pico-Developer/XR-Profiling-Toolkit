/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.IO;
using UnityEngine;

namespace PXR.XRProfilingToolkit.Utilities
{
    /// <summary>
    /// Capture the current scene
    /// Can be used for app logo generation
    /// </summary>
    public class ScreenCaptureCamera : MonoBehaviour
    {
        [SerializeField] private string outputPath;

        [ContextMenu("Capture Screen")]
        private void CaptureScreen()
        {
            StartCoroutine(CaptureScreenCoroutine());
        }

        private IEnumerator CaptureScreenCoroutine()
        {   
            yield return new WaitForEndOfFrame();

            int width = Screen.width;
            int height = Screen.height;
            var outputTex = new Texture2D(width, height, TextureFormat.ARGB32, false);
            var grabArea = new Rect(0, 0, width, height);
            
            outputTex.ReadPixels(grabArea, 0, 0, false);
            outputTex.Apply();
            
            // Encode the resulting output texture to a byte array then write to the file
            byte[] pngShot = ImageConversion.EncodeToPNG(outputTex);
            File.WriteAllBytes(outputPath, pngShot);
        }
    }
}