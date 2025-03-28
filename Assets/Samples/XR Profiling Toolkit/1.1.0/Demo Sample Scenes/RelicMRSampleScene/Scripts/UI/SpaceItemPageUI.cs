/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using MRDemoSampleScene.Runtime.Data;
using UnityEngine;
namespace MR_Demo_Sample_Scene.Scripts.UI
{
    public class SpaceItemPageUI : MonoBehaviour
    {
        public Transform Root;
        public GameObject Prefab;
        public List<SpaceItemUI> ItemPool;
        private bool _init = false;
        
        public void Awake()
        {
            if (ItemPool == null)
            {
                ItemPool = new List<SpaceItemUI>();
            }
        }

        public void Start()
        {
            if (!_init)
            {
                foreach (var SpaceItemData in ResourceLoader.Instance.mrAssetsSettings.Id2PrefabDatas)
                {
                    var SpaceItemUI =  GameObject.Instantiate(Prefab).GetComponent<SpaceItemUI>();
                    SpaceItemUI.SetData(SpaceItemData);
                    SpaceItemUI.transform.SetParent(Root, false);
                    SpaceItemUI.gameObject.SetActive(true);
                    ItemPool.Add(SpaceItemUI);
                }
                _init = true;
            }
            
        }
    }
}