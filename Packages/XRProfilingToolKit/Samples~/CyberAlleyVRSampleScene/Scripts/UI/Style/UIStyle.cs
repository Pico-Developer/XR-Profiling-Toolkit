/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

/// <summary>
/// Holds style of various types of UI components
/// </summary>
[CreateAssetMenu(fileName = "UIStyle", menuName = "XRProfilingToolkit/UI/UIStyle", order = 1)]
public class UIStyle : ScriptableObject
{
    public TMP_FontAsset fontAsset;
    public TMP_FontAsset boldFontAsset;
    
    [Header("Colors")]
    [Header("Interaction State Colors")]
    public ColorBlock stateColorBlock;

    /// <summary>
    /// Default color for text.
    /// </summary>
    public Color textColor; // #D6D6D6
    
    /// <summary>
    /// Default color for text.
    /// </summary>
    public Color textGreyedColor; // #797979
    
    [Header("Base and Accent Colors")]
    public Color baseColor; // #3D3D3D
    public Color baseLowColor; // #313131
    public Color baseHighColor; // #474747
    public Color transparentColor; 
    
    /// <summary>
    /// Default color for highlighted content in blue.
    /// </summary>
    public Color blueAccentColor; // #3D8BFF
    
    /// <summary>
    /// Default color for highlighted content in purple/indigo.
    /// </summary>
    public Color purpleAccentColor; // #564CFF
    
    /// <summary>
    /// Default color for highlighted content in orange.
    /// </summary>
    public Color orangeAccentColor; // #FFB34C
    /// <summary>
    /// Default color for highlighted content in red.
    /// </summary>
    public Color redAccentColor; // #FF274B
    /// <summary>
    /// Default color for highlighted content in green.
    /// </summary>
    public Color greenAccentColor; // #00EB6D
    [Header("Item Selected and Active Colors")] 
    public Color selectedColor;
    public Color activeColor;

    /// <summary>
    /// Used for key data display. Size: 40, Line Height: 56, Weight: Bold (700).
    /// </summary>
    [Header("Typography Styles")]
    public TypographyStyle display;
    
    /// <summary>
    /// Used for titles. Size: 20, Line Height: 28, Weight: Bold (700).
    /// </summary>
    public TypographyStyle title;
    
    /// <summary>
    /// Used for bold remarks in list items. Size: 12, Line Height: 16, Weight: Semi-Bold (600).
    /// </summary>
    public TypographyStyle remark;
    
    /// <summary>
    /// Used for large body text. Size: 24, Line Height: 36.
    /// </summary>
    public TypographyStyle bodyLarge;
    
    /// <summary>
    /// Used for large body text. Size: 16, Line Height: 26.
    /// </summary>
    public TypographyStyle body;

    [Header("Spacing")] 
    public int[] spacing;
}

[System.Serializable]
public class TypographyStyle
{
    public string name; // For editor visibility and debugging
    public int fontSize;
    public int lineHeight; // Use this as a guideline for UI spacing, Unity doesn't directly support lineHeight
    public FontWeight fontWeight;

    public enum FontWeight
    {
        Thin100 = 100,
        Light300 = 300,
        Regular400 = 400,
        SemiProminent600 = 600,
        Prominent700 = 700,
    }
}

