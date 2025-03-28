/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace MRDemoSampleScene.Runtime.Item
{
    [RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable))]
    public class SpaceItem :MonoBehaviour
    {
        public GameObject GameObject => gameObject;
        public ulong anchorHandle { get; set; }
        
        protected UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable baseInteractable;
        

        protected virtual void Awake()
        {
            baseInteractable = GetComponentInChildren<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();

        }

        protected void OnEnable()
        {
            RegisterEvent();
        }

        protected void OnDisable()
        {
            UnregisterEvent();
        }

        private void RegisterEvent()
        {
            if (baseInteractable != null)
            {
                baseInteractable.firstHoverEntered.AddListener(OnFirstHoverEntered);
                baseInteractable.lastHoverExited.AddListener(OnLastHoverExited);
                baseInteractable.firstSelectEntered.AddListener(OnFirstSelectEntered);
                baseInteractable.lastSelectExited.AddListener(OnLastSelectExited);
            }
            
        }

        private void UnregisterEvent()
        {
            if (baseInteractable != null)
            {
                baseInteractable.firstHoverEntered.RemoveListener(OnFirstHoverEntered);
                baseInteractable.lastHoverExited.RemoveListener(OnLastHoverExited);
                baseInteractable.firstSelectEntered.RemoveListener(OnFirstSelectEntered);
                baseInteractable.lastSelectExited.RemoveListener(OnLastSelectExited);
            }
        }
        
        protected virtual void OnFirstHoverEntered(HoverEnterEventArgs args) => OnHoverEnter();

        // 处理最后一次悬停退出事件
        protected virtual void OnLastHoverExited(HoverExitEventArgs args) => OnHoverExit();

        // 处理第一次选择进入事件
        protected virtual void OnFirstSelectEntered(SelectEnterEventArgs args) => OnSelectedEnter();

        // 处理最后一次选择退出事件
        protected virtual void OnLastSelectExited(SelectExitEventArgs args) => OnHasSelectedExit();
        
        private void OnHoverEnter()
        {

        }
        
        private void OnHoverExit()
        {

        }
        
        private void OnSelectedEnter()
        {

        }

        private void OnHasSelectedExit()
        {

        }
        
    }
}