/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using System;
using DeveloperTech.XRProfilingToolkit;
using DeveloperTech.XRProfilingToolkit.Scene;
using MRDemoSampleScene.Runtime.Data;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

namespace MRDemoSampleScene.Runtime.UI
{
    [Serializable]
    internal struct InputActionControllerAssetsEntry
    {
        public PlatformSwitcher.PlatformType platformType;
        public InputActionReference menuBtnLeft;
        public InputActionReference primaryBtnLeft;
        public InputActionReference primaryBtnRight;
        public InputActionReference gripBtnLeft;
        public InputActionReference gripBtnRight;
        public InputActionReference triggerBtnLeft;
        public InputActionReference triggerBtnRight;
        public InputActionReference secondaryBtnLeft;
        public InputActionReference secondaryBtnRight;
        public Transform leftControllerPreviewPoint;
        public Transform rightControllerPreviewPoint;
    }
    public enum ControllerState
    {
        Normal,
        AnchorCreate
    }
    public class ControllerManager : MonoBehaviour
    {
        public static ControllerManager Instance;
        public XRInputModalityManager inputModalityManager;
       
        [HideInInspector]public Transform leftControllerPreviewPoint;
        [HideInInspector]public Transform rightControllerPreviewPoint;
        [HideInInspector]public InputActionReference menuBtnLeft;
        [HideInInspector]public InputActionReference primaryBtnLeft;
        [HideInInspector]public InputActionReference primaryBtnRight;
        [HideInInspector]public InputActionReference gripBtnLeft;
        [HideInInspector]public InputActionReference gripBtnRight;
        [HideInInspector]public InputActionReference triggerBtnLeft;
        [HideInInspector]public InputActionReference triggerBtnRight;
        [HideInInspector]public InputActionReference secondaryBtnLeft;
        [HideInInspector]public InputActionReference secondaryBtnRight;
        [HideInInspector]private Action<InputAction.CallbackContext> triggerInputLeft;
        [HideInInspector]private Action<InputAction.CallbackContext> triggerInputRight;
        [HideInInspector]private Action<InputAction.CallbackContext> gripInputLeft;
        [HideInInspector]private Action<InputAction.CallbackContext> gripInputRight;
        [HideInInspector]private Action<InputAction.CallbackContext> primaryInputLeft;
        [HideInInspector]private Action<InputAction.CallbackContext> primaryInputRight;
        [HideInInspector]private Action<InputAction.CallbackContext> secondaryInputLeft;
        [HideInInspector]private Action<InputAction.CallbackContext> secondaryInputRight;
        private readonly string _tag = nameof(ControllerManager);
        [HideInInspector]
        private ControllerState leftControllerState = ControllerState.Normal;
        [HideInInspector]
        private ControllerState rightControllerState = ControllerState.Normal;
        [SerializeField] private InputActionControllerAssetsEntry[] _controllerAssets;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
        }
        
        private void OnEnable()
        {
            RegisterEvent();
        }

        private void OnDisable()
        {
            UnregisterEvent();
        }
        
        private void RegisterEvent()
        {
            ConfigureController(PlatformSwitcher.GetPlatform());
        }

        private void UnregisterEvent()
        {
            
        }
        
        public void ConfigureController(PlatformSwitcher.PlatformType platformType)
        {
            foreach (var entry in _controllerAssets)
            {
                if (entry.platformType == platformType)
                {
                    menuBtnLeft = entry.menuBtnLeft;
                    primaryBtnLeft = entry.primaryBtnLeft;
                    primaryBtnRight = entry.primaryBtnRight;
                    gripBtnLeft = entry.gripBtnLeft;
                    gripBtnRight = entry.gripBtnRight;
                    triggerBtnLeft = entry.triggerBtnLeft;
                    triggerBtnRight = entry.triggerBtnRight;
                    secondaryBtnLeft = entry.secondaryBtnLeft;
                    secondaryBtnRight = entry.secondaryBtnRight;
                    leftControllerPreviewPoint = entry.leftControllerPreviewPoint;
                    rightControllerPreviewPoint = entry.rightControllerPreviewPoint;
                }
            }
        }
        public void BindingMainMenuHotKey()
        {
            if (primaryBtnLeft != null)
            {
                primaryBtnLeft.action.Enable();
                primaryBtnLeft.action.performed += OpenMenu;
                menuBtnLeft.action.Enable();
                menuBtnLeft.action.performed += OpenMenu;
            }

            if (primaryBtnRight != null)
            {
                primaryBtnRight.action.Enable();
                primaryBtnRight.action.performed += OpenMenu;
            }
        }

        public void SetControllerState(bool isLeftController, ControllerState state)
        {
            if (isLeftController)
            {
                leftControllerState = state;
            }
            else
            {
                rightControllerState = state;
            }
        }
        
        public ControllerState GetControllerState(bool isLeftController)
        {
            if (isLeftController)
            {
                return leftControllerState;
            }
            else
            {
                return rightControllerState;
            }
        }
        public GameObject GetController(bool isLeftController)
        {
            if (isLeftController)
            {
                return inputModalityManager.leftController;
            }
            else
            {
                return inputModalityManager.rightController;
            }
        }
        public void BingingTriggerHotKey(bool isLeftController,Action<InputAction.CallbackContext>  downAction)
        {
            if (isLeftController)
            {
                if (triggerInputLeft != null)
                {
                    UnBingingTriggerInputActionLeft();
                }
                triggerInputLeft = downAction;
                if(triggerBtnLeft != null && triggerInputLeft != null)
                {
                    triggerBtnLeft.action.Enable();
                    triggerBtnLeft.action.performed += triggerInputLeft;
                }
            }
            else
            {
                if (triggerInputRight != null)
                {
                    UnBingingTriggerInputActionRight();
                }
                triggerInputRight = downAction;
                if(triggerBtnRight != null && triggerInputRight != null)
                {
                    triggerBtnRight.action.Enable();
                    triggerBtnRight.action.performed += triggerInputRight;
                }
            }
        }
        public void BingingGripHotKey(bool isLeftController,Action<InputAction.CallbackContext>  downAction)
        {
            if (isLeftController)
            {
                if (gripInputLeft != null)
                {
                    UnBingingGripInputActionLeft();
                }
                gripInputLeft = downAction;
                if(gripBtnLeft != null && gripInputLeft != null)
                {
                    gripBtnLeft.action.Enable();
                    gripBtnLeft.action.performed += gripInputLeft;
                }
            }
            else
            {
                if (gripInputRight != null)
                {
                    UnBingingGripInputActionRight();
                }
                gripInputRight = downAction;
                if(gripBtnRight != null && gripInputRight != null)
                {
                    gripBtnRight.action.Enable();
                    gripBtnRight.action.performed += gripInputRight;
                }
            }
        }
        public void BingingPrimaryHotKey(bool isLeftController,Action<InputAction.CallbackContext>  downAction)
        {
            if (isLeftController)
            {
                if (primaryInputLeft != null)
                {
                    UnBingingPrimaryInputActionLeft();
                }
                primaryInputLeft = downAction;
                if (primaryBtnLeft != null && primaryInputLeft!= null)
                {
                    primaryBtnLeft.action.Enable();
                    primaryBtnLeft.action.performed += primaryInputLeft;
                }
            }
            else
            {
                if (primaryInputRight != null)
                {
                    UnBingingPrimaryInputActionRight();
                }
                primaryInputRight = downAction;
                if (primaryBtnRight != null && primaryInputRight != null)
                {
                    primaryBtnRight.action.Enable();
                    primaryBtnRight.action.performed += primaryInputRight;
                }
            }
        }
        public void BingingSecondaryHotKey(bool isLeftController,Action<InputAction.CallbackContext>  downAction)
        {
            if (isLeftController)
            {
                if (secondaryInputLeft != null)
                {
                    UnBingingSecondaryInputActionLeft();
                }
                secondaryInputLeft = downAction;
                if (secondaryBtnLeft != null && secondaryInputLeft!= null)
                {
                    secondaryBtnLeft.action.Enable();
                    secondaryBtnLeft.action.performed += secondaryInputLeft;
                }
            }
            else
            {
                if (secondaryInputRight != null)
                {
                    UnBingingSecondaryInputActionRight();
                }
                secondaryInputRight = downAction;
                if (secondaryBtnRight != null && secondaryInputRight != null)
                {
                    secondaryBtnRight.action.Enable();
                    secondaryBtnRight.action.performed += secondaryInputRight;
                }
            }
        }
        
        public void UnBingingGameHotKey(bool isLeftController)
        {
            if (isLeftController)
            {
                UnBingingTriggerInputActionLeft();
                UnBingingGripInputActionLeft();
            }
            else
            {
                UnBingingTriggerInputActionRight();
                UnBingingGripInputActionRight();
            }
        }

        private void UnBingingTriggerInputActionLeft()
        {
            if (triggerBtnLeft != null && triggerInputLeft != null)
            {
                triggerBtnLeft.action.Disable();
                triggerBtnLeft.action.performed -= triggerInputLeft;
                triggerInputLeft = null;
            }
        }

        private void UnBingingTriggerInputActionRight()
        {
            if (triggerBtnRight != null && triggerInputRight != null)
            {
                triggerBtnRight.action.Disable();
                triggerBtnRight.action.performed -= triggerInputRight;
                triggerInputRight = null;
            }
        }

        private void UnBingingGripInputActionLeft()
        {
            if (gripBtnLeft != null && gripInputLeft != null)
            {
                gripBtnLeft.action.Disable();
                gripBtnLeft.action.performed -= gripInputLeft;
                gripInputLeft = null;
            }
        }

        private void UnBingingGripInputActionRight()
        {
            if (gripBtnRight != null && gripInputRight != null)
            {
                gripBtnRight.action.Disable();
                gripBtnRight.action.performed -= gripInputRight;
                gripInputRight = null;
            }
        }

        private void UnBingingPrimaryInputActionLeft()
        {
            if (primaryBtnLeft != null && primaryInputLeft!= null)
            {
                primaryBtnLeft.action.Disable();
                primaryBtnLeft.action.performed -= primaryInputLeft;
                primaryInputLeft = null;
            }
        }
        public void UnBingingPrimaryInputActionRight()
        {
            if (primaryBtnRight != null && primaryInputRight!= null)
            {
                primaryBtnRight.action.Disable();
                primaryBtnRight.action.performed -= primaryInputRight;
                primaryInputRight = null;
            }
        }
        public void UnBingingSecondaryInputActionLeft()
        {
            if (secondaryBtnLeft != null && secondaryInputLeft!= null)
            {
                secondaryBtnLeft.action.Disable();
                secondaryBtnLeft.action.performed -= secondaryInputLeft;
                secondaryInputLeft = null;
            }
        }
        public void UnBingingSecondaryInputActionRight()
        {
            if (secondaryBtnRight != null && secondaryInputRight!= null)
            {
                secondaryBtnRight.action.Disable();
                secondaryBtnRight.action.performed -= secondaryInputRight;
                secondaryInputRight = null;
            }
        }
        
        private void OpenMenu(InputAction.CallbackContext content)
        { 
            //MenuController.Instance.ToggleUI();
        }
        
        /// <summary>
        /// Display a preview object in a virtual reality scene
        /// </summary>
        /// <param name="previewObj">The game object of the preview object to be displayed</param>
        /// <param name="isLeft">A Boolean value indicating whether the object is displayed on the left (true) or right (false)</param>
        public void ShowAnchorPreview(Id2PrefabData data,bool isLeft)
        {
            GameObject newObj = Instantiate(data.prefab, (isLeft?leftControllerPreviewPoint:rightControllerPreviewPoint).transform.position, (isLeft?leftControllerPreviewPoint:rightControllerPreviewPoint).rotation);
            newObj.transform.localScale = data.scale;
            newObj.transform.SetParent(isLeft?leftControllerPreviewPoint:rightControllerPreviewPoint);
            newObj.SetActive(true);
        }

        public void HideAnchorPreview(bool isLeft)
        {
            foreach (Transform child in (isLeft?leftControllerPreviewPoint:rightControllerPreviewPoint).transform)
            {
                Destroy(child.gameObject);  
            }
        }
    }

    
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T> //注意此约束为T必须为其本身或子类
    {
        private static T _instance; //创建私有对象记录取值，可只赋值一次避免多次赋值

        public static T Instance
        {
            //实现按需加载
            get
            {
                //当已经赋值，则直接返回即可
                if (_instance != null) return _instance;

                _instance = FindObjectOfType<T>();

                //为了防止脚本还未挂到物体上，找不到的异常情况，可以自行创建空物体挂上去
                if (_instance == null)
                {
                    //如果创建对象，则会在创建时调用其身上脚本的Awake即调用T的Awake(T的Awake实际上是继承的父类的）
                    //所以此时无需为instance赋值，其会在Awake中赋值，自然也会初始化所以无需init()
                    /*instance = */
                    new GameObject("Singleton of " + typeof(T)).AddComponent<T>();
                }
                else _instance.Init(); //保证Init只执行一次

                return _instance;

            }
        }

        private void Awake()
        {
            //若无其它脚本在Awake中调用此实例，则可在Awake中自行初始化instance
            _instance = this as T;
            //初始化
            Init();
        }

        //子类对成员进行初始化如果放在Awake里仍会出现Null问题所以自行制作一个init函数解决（可用可不用）
        protected virtual void Init()
        {

        }
    }
}