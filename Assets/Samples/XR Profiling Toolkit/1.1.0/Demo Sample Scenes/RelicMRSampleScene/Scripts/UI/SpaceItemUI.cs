/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using MRDemoSampleScene.Runtime.Data;
using MRDemoSampleScene.Runtime.Entity;
using MRDemoSampleScene.Runtime.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Object = UnityEngine.Object;

namespace MR_Demo_Sample_Scene.Scripts.UI
{
    public class SpaceItemUI : MonoBehaviour
    {
        public Button itemBtn;
        public Image sprite;
        public TMP_Text description;
        public Id2PrefabData itemdata;
        public void Start()
        {
            
        }

        public void SetData(Id2PrefabData data)
        {
            itemdata = data;
            description.text = data.description;
            sprite.sprite = data.sprite;
            itemBtn.GetComponent<XRSimpleInteractable>().lastSelectExited.AddListener(arg =>
                //itemBtn.onClick.AddListener(async () =>
            {
                bool isLeftController = arg.interactorObject.handedness == InteractorHandedness.Left;
                if (ControllerManager.Instance.GetControllerState(isLeftController) != ControllerState.Normal)
                {

                }
                else
                {
                    // binding delect event
                    ControllerManager.Instance.ShowAnchorPreview(data, isLeftController);
                    ControllerManager.Instance.BingingTriggerHotKey(isLeftController,
                        (args) => { CreateObj(isLeftController, data); });
                    ControllerManager.Instance.BingingGripHotKey(isLeftController,
                        (args) =>
                        {
                            ControllerManager.Instance.HideAnchorPreview(isLeftController);
                            ControllerManager.Instance.UnBingingGameHotKey(isLeftController);
                            ControllerManager.Instance.SetControllerState(isLeftController,
                                ControllerState.Normal);
                        });
                    ControllerManager.Instance.SetControllerState(isLeftController, ControllerState.AnchorCreate);
                }

            });
        }
        
        private void CreateObj( Id2PrefabData data,bool isLeft,out GameObject gameObject)
        {
            gameObject = Object.Instantiate(data.prefab, (isLeft?ControllerManager.Instance.leftControllerPreviewPoint:ControllerManager.Instance.rightControllerPreviewPoint).transform.position, (isLeft?ControllerManager.Instance.leftControllerPreviewPoint:ControllerManager.Instance.rightControllerPreviewPoint).transform.rotation,EntityManager.Instance.GetGameEntityRoot());
            gameObject.transform.localScale = data.scale;
            gameObject.tag = "SpaceItem";
        }

        public async void CreateObj(bool isLeftController,Id2PrefabData data)
        {
            CreateObj(data, isLeftController, out var item);
            if (item == null) return;
            var entity = await EntityManager.Instance.CreateEntity(item);

        }
    }
}