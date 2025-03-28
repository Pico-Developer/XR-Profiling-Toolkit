/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MRDemoSampleScene.Runtime.Data;
using MRDemoSampleScene.Runtime.SDK;
using MRDemoSampleScene.Runtime.UI;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using Object = UnityEngine.Object;

namespace MRDemoSampleScene.Runtime.Entity
{
    public class EntityManager : MonoBehaviour
    {
        public static EntityManager Instance;
        private readonly string TAG = nameof(EntityManager);

        private List<Entity> _gameEntities = new List<Entity>();
        public GameObject gameEntityRoot;
        
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
        
        void Start()
        {
            ControllerManager.Instance.BingingSecondaryHotKey(true, (args) => { DeleteObj(true); });
            ControllerManager.Instance.BingingSecondaryHotKey(false, (args) => { DeleteObj(false); });
           
        }

        private void OnDestroy()
        {
            ControllerManager.Instance.UnBingingSecondaryInputActionLeft();
            ControllerManager.Instance.UnBingingSecondaryInputActionRight();
        }
        public async void CreateObjByCommand(ulong itemId,Vector3 position,Quaternion rotation,Vector3 scale)
        {
            var spaceItemData = ResourceLoader.Instance.mrAssetsSettings.Id2PrefabDatas.FirstOrDefault(x => x.id == itemId);
            var item = Object.Instantiate(spaceItemData.prefab,position,rotation,gameEntityRoot.transform);
            if (item == null) return;
            item.transform.localScale = scale;
            var entity = await CreateEntity(item);
        }
        public async void CreateObj(Vector3 position,Quaternion rotation,Id2PrefabData data)
        {
            var item = Object.Instantiate(data.prefab,position,rotation,gameEntityRoot.transform);
            if (item == null) return;
            var entity = await CreateEntity(item);
        }
        private void DeleteObj( bool isLeftController)
        {
            var controller = isLeftController
                ? ControllerManager.Instance.inputModalityManager.leftController
                : ControllerManager.Instance.inputModalityManager.rightController;
            var nearfarInteractor = controller.GetComponentInChildren<NearFarInteractor>();
            if (nearfarInteractor != null)
            {
                foreach (var interactable in nearfarInteractor.interactablesHovered)
                {
                    Debug.Log("Near interactor is touching: " + interactable.transform.gameObject.name);
                    if (interactable.transform.tag == "SpaceItem")
                    {
                        DeleteEntity(interactable.transform.GetComponent<Entity>());
                    }
                }
            }
        }
        public Transform GetGameEntityRoot()
        {
            return gameEntityRoot.transform;
        }
        public async Task<Entity> CreateEntity(GameObject gameObject)
        {

            var anchorData = await MRSDKManager.Instance.CreateAnchor(gameObject.transform);
            if (anchorData != null)
            {
                var entity = gameObject.AddComponent<Entity>();
                entity.AnchorData = anchorData;
                entity.GameObject = gameObject;
                Debug.unityLogger.Log(TAG, $"Create Entity, uuid: {entity.AnchorData.Uuid}, handle: {entity.AnchorData.Handle} Position:({entity.GameObject.transform.position.x:F3}, {entity.GameObject.transform.position.y:F3}, {entity.GameObject.transform.position.z:F3})");
                _gameEntities.Add(entity);
                return entity;
            }
            else
            {
                Debug.unityLogger.LogError(TAG, $"Create Entity anchorData == null");
                return null;
            }
        }

        public void DeleteEntity(Entity entity)
        {
            MRSDKManager.Instance.DeleteAnchor(entity.AnchorData);
            Object.Destroy(entity.GameObject);
        }
        
        public IList<Entity> GetGameEntities()
        {
            return _gameEntities;
        }
        
        public async Task UpdateSpatialAnchorPosition()
        {
            var gameEntities = _gameEntities.ToArray();
            foreach (var entity in gameEntities)
            {
                var result = PXR_MixedReality.LocateAnchor(entity.AnchorData.Handle, out var position, out var rotation);
                //Debug.unityLogger.Log(TAG, $"Update Spatial Anchor Position Anchor Key: {entity.AnchorData.Handle}, Guid: {entity.AnchorData.Uuid} Position:({entity.GameObject.transform.position.x:F3}, {entity.GameObject.transform.position.y:F3}, {entity.GameObject.transform.position.z:F3} " +
                //                           $"TO Position:({position.x:F3}, {position.y:F3}, {position.z:F3}");
                if (result == PxrResult.SUCCESS)
                {
                    entity.GameObject.transform.position = position;
                    entity.GameObject.transform.rotation = rotation;
                }
            }
        }
        
    }
}