/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;

namespace MRDemoSampleScene.Runtime.Data
{
    [Serializable]
    public class SpaceItemData : MonoBehaviour
    {
        [SerializeField] private int _id;
        
        [SerializeField] private string _title;

        [SerializeField] private string _description;
        
        [SerializeField] private GameObject _prefab;

        [SerializeField] private Sprite _sprite;

        public int Id
        {
            get => _id;
            set => _id = value;
        }
        
        public string Title
        {
            get => _title;
            set => _title = value;
        }

        public string Description
        {
            get => _description;
            set => _description = value;
        }

        public GameObject Prefab
        {
            get => _prefab;
            set => _prefab = value;
        }

        public Sprite Sprite
        {
            get => _sprite;
            set => _sprite = value;
        }
    }
}