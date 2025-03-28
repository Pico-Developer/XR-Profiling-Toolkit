/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;
using MRDemoSampleScene.Runtime.Utils;
using System;
using System.Linq;
using LitJson;
using MRDemoSampleScene.Runtime.Data;
using MRDemoSampleScene.Runtime.Entity;


public class SceneManagerPICO : MonoBehaviour
{
    private const string TAG = "[SceneManagerPICO]";
    public static SceneManagerPICO Instance;
    public GameObject box2DPrefab;
    public GameObject box3DPrefab;
    public Material roomEntityMaterial;
    [SerializeField]
    private TextAsset sceneCaptureData;
    private Dictionary<ulong,Guid > sceneAnchorList = new Dictionary<ulong,Guid>();
    private Dictionary<ulong, Transform> sceneAnchorMap = new Dictionary<ulong, Transform>();
    [SerializeField]
    private float maxDriftDelay = 0.5f;
    private float currDriftDelay = 0f;
    private GameObject mainTestObject;
    
    public bool IsSceneCaptureFinished { get; private set; }
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
        IsSceneCaptureFinished = false;
        if (sceneAnchorList == null)
        {
            sceneAnchorList = new Dictionary<ulong,Guid>();
        }
        if (sceneAnchorMap == null)
        {
            sceneAnchorMap = new Dictionary<ulong, Transform>();
        }
#if UNITY_EDITOR
        LoadSceneDataFromJson();
        FindTableItem();
#else
        PXR_Manager.EnableVideoSeeThrough = true;
        StartSceneCaptureProvider();
        StartSpatialAnchorProvider();
#endif

    }
    
    //Re-enable seethrough after the app resumes
    void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            PXR_Manager.EnableVideoSeeThrough = true;
        }
    }
    
    void OnEnable()
    {
        PXR_Manager.SceneAnchorDataUpdated += SceneAnchorDataUpdated;
    }

    void OnDisable()
    {
        PXR_Manager.SceneAnchorDataUpdated -= SceneAnchorDataUpdated;
    }
    
    private void OnDestroy()
    {
        PXR_Manager.SceneAnchorDataUpdated -= SceneAnchorDataUpdated;
        PXR_Manager.EnableVideoSeeThrough = false;
    }
    
    private void SceneAnchorDataUpdated()
    {
        LoadSceneData();
    }
    
    private async void StartSceneCaptureProvider()
    {
        var result0 = await PXR_MixedReality.StartSenseDataProvider(PxrSenseDataProviderType.SceneCapture);
        if (result0 == PxrResult.SUCCESS)
        {
            LoadSceneData();
        }
        else
        {
            Debug.unityLogger.Log(TAG, "SceneCaptureProvider start fail");
        }
    }
    
    private async void StartSpatialAnchorProvider()
    {
        var result = await PXR_MixedReality.StartSenseDataProvider(PxrSenseDataProviderType.SpatialAnchor);
        Debug.unityLogger.Log(TAG,$"StartSenseDataProvider: {result}");
    }

    private void FixedUpdate()
    {
        HandleSpatialDrift();
    }

    private void HandleSpatialDrift()
    {
        //if no anchors, we don't need to handle drift
        if (sceneAnchorMap.Count == 0)
            return;
        
        currDriftDelay += Time.deltaTime;
        if(currDriftDelay >= maxDriftDelay)
        {
            Debug.unityLogger.Log(TAG, "HandleSpatialDrift Update");
            currDriftDelay = 0f;
            foreach(var handlePair in sceneAnchorMap)
            {
                var handle = handlePair.Key;
                var anchorTransform = handlePair.Value;

                if(handle == UInt64.MinValue)
                {
                    continue;
                }

                PXR_MixedReality.LocateAnchor(handle, out var position, out var rotation);
                Debug.unityLogger.Log(TAG, "HandleSpatialDrift Update LocateAnchor "+handle);
                anchorTransform.position= position;
                anchorTransform.rotation= rotation;
            }
        }
    }
    
    private bool IsTableAnchor(ulong handle, out PxrSemanticLabel label)
    {
        PxrResult labelResult = PXR_MixedReality.GetSceneSemanticLabel(handle, out label);
        return labelResult == PxrResult.SUCCESS && label == PxrSemanticLabel.Table;
    }
    private void FindTableItem()
    {
#if !UNITY_EDITOR
        if (ResourceLoader.Instance.mrAssetsSettings.MainTestPrefab == null)
        {
            return;
        }
#endif
        if (mainTestObject == null)
        {
            mainTestObject = Instantiate(ResourceLoader.Instance.mrAssetsSettings.MainTestPrefab, new Vector3(0, 1f, 1.5f), Quaternion.Euler(new Vector3(0, -120f, 0)), EntityManager.Instance.GetGameEntityRoot());
            EntityManager.Instance.CreateEntity(mainTestObject);
        }

        foreach (var handlePair in sceneAnchorMap)
        {
            if (IsTableAnchor(handlePair.Key, out var label))
            {
                mainTestObject.transform.position = handlePair.Value.position;
                return;
            }
        }
    }
    private async void LoadSceneData()
    {
        Debug.unityLogger.Log(TAG,$"Start LoadSceneData");
        var result = await PXR_MixedReality.QuerySceneAnchorAsync(default);
        Debug.unityLogger.Log(TAG,$"Start LoadSceneData result " + result.result);
        if (result.result == PxrResult.SUCCESS)
        {
            Debug.Log($"Start LoadSpaceData anchorHandleList Count " + result.anchorDictionary.Count);
            Debug.unityLogger.Log(TAG,$"FindTableItem666:"+result.anchorDictionary.Count);
            if (result.anchorDictionary.Count > 0)
            {
                foreach (var item in result.anchorDictionary)
                {
                    if (!sceneAnchorList.Contains(item))
                    {
                        var labelResult = PXR_MixedReality.GetSceneSemanticLabel(item.Key, out var label);
                        if (labelResult == PxrResult.SUCCESS)
                        {
                            DrawSceneModel(item.Key, label);
                        }
                        sceneAnchorList.Add(item.Key, item.Value);
                    }
                }
            }
            else
            {
                var result2 = await PXR_MixedReality.StartSceneCaptureAsync(default);
                if (result2 == PxrResult.SUCCESS)
                {
                    LoadSceneData();
                }
                PLog.e(TAG, "Query scene anchor count is 0", false);
            }
            FindTableItem();
            if (!IsSceneCaptureFinished)
            {
                IsSceneCaptureFinished = true;
            }
        }
        else
        {
            PLog.e(TAG, "Query scene anchor fail" + result.result, false);
        }
        
    }

    private void DrawSceneModel(ulong anchorHandle, PxrSemanticLabel label)
    {
        switch (label)
        {
            //Box3D Objects
            //Volume: The Anchor is located at the center of the rectangle on the upper surface of the cube with Z axis as up
            case PxrSemanticLabel.Unknown:
                break;
            case PxrSemanticLabel.Table:
            case PxrSemanticLabel.Sofa:
            case PxrSemanticLabel.Chair:
            case PxrSemanticLabel.Curtain:
            case PxrSemanticLabel.Cabinet:
            case PxrSemanticLabel.Bed:
            case PxrSemanticLabel.Plant:
            case PxrSemanticLabel.Screen:
            case PxrSemanticLabel.Refrigerator:
            case PxrSemanticLabel.WashingMachine:
            case PxrSemanticLabel.AirConditioner:
            case PxrSemanticLabel.Lamp:
            {
                var result = PXR_MixedReality.GetSceneBox3DData(anchorHandle, out var position, out var rotation, out var extent);
                if (result == PxrResult.SUCCESS)
                {
                    if (box3DPrefab != null)
                    {
                        var sceneAnchor = new GameObject(anchorHandle.ToString());
                        var box3D = Instantiate(box3DPrefab);
                        //currently,rotation not support
                        box3D.transform.localPosition = position;
                        box3D.transform.localScale = extent;
                        PXR_MixedReality.LocateAnchor(anchorHandle, out var anchorPosition, out var anchorRotation);
                        box3D.transform.SetParent(sceneAnchor.transform);
                        sceneAnchor.transform.rotation = anchorRotation;
                        sceneAnchor.transform.position = anchorPosition;
                        sceneAnchorMap.Add(anchorHandle, sceneAnchor.transform);
                    }
                    else
                    {
                        
                        PLog.e(TAG, "box3D prefab is null", false);
                    }
                }
            }
                break;
            //Box2D Objects
            case PxrSemanticLabel.Wall:
            case PxrSemanticLabel.VirtualWall:
            case PxrSemanticLabel.Door:
            case PxrSemanticLabel.Window:
            case PxrSemanticLabel.Opening:
            case PxrSemanticLabel.WallArt:
            {
                var result = PXR_MixedReality.GetSceneBox2DData(anchorHandle, out var offset, out var extent);
                if (result == PxrResult.SUCCESS)
                {
                    //currently,offset not support
                    if (box2DPrefab != null)
                    {
                        var sceneAnchor = new GameObject(anchorHandle.ToString());
                        var box2D = Instantiate(box2DPrefab);
                        box2D.transform.localScale = new Vector3(extent.x, extent.y, 0);
                        PXR_MixedReality.LocateAnchor(anchorHandle, out var anchorPosition, out var anchorRotation);
                        box2D.transform.SetParent(sceneAnchor.transform);
                        sceneAnchor.transform.rotation = anchorRotation;
                        sceneAnchor.transform.position = anchorPosition;
                        sceneAnchorMap.Add(anchorHandle, sceneAnchor.transform);
                    }
                    else
                    {
                        PLog.e(TAG, "box2D prefab is null", false);
                    }
                }
            }
                break;
            //Polygon Objects
            case PxrSemanticLabel.Ceiling:
            case PxrSemanticLabel.Floor:
            {
                var result = PXR_MixedReality.GetScenePolygonData(anchorHandle, out var vertices);
                if (result == PxrResult.SUCCESS)
                {
                    var sceneAnchor = new GameObject(anchorHandle.ToString());
                    var verVector3S = Array.ConvertAll(vertices, v2 => new Vector3(v2.x, v2.y, 0f));
                    var polygon = new GameObject();
                    var lineRenderer = polygon.AddComponent<LineRenderer>();
                    lineRenderer.startColor = Color.red;
                    lineRenderer.endColor = Color.red;
                    lineRenderer.startWidth = 0.1f;
                    lineRenderer.positionCount = verVector3S.Length;
                    lineRenderer.loop = true;
                    lineRenderer.useWorldSpace = false;
                    lineRenderer.endWidth = 0.1f;
                    lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                    lineRenderer.SetPositions(verVector3S);
                    polygon.transform.SetParent(sceneAnchor.transform);
                    PXR_MixedReality.LocateAnchor(anchorHandle, out var anchorPosition, out var anchorRotation);
                    sceneAnchor.transform.rotation = anchorRotation;
                    sceneAnchor.transform.position = anchorPosition;
                    sceneAnchorMap.Add(anchorHandle, sceneAnchor.transform);
                    
                }
            }
                break;
        }
    }


#if UNITY_EDITOR
        private void LoadSceneDataFromJson()
        {
            if (sceneCaptureData != null)
            {
                JsonData jsonData = JsonMapper.ToObject(sceneCaptureData.ToString());
                for (int i = 0; i < jsonData.Count; i++)
                {
                    var sceneAnchorInfo = jsonData[i];

                    var uuid = sceneAnchorInfo["Guid"].ToString();
                    Enum.TryParse(jsonData[i]["SemanticLabel"].ToString(), out PxrSemanticLabel semantic);

                    var pX = Convert.ToSingle(jsonData[i]["Position"]["x"].ToString());
                    var pY = Convert.ToSingle(jsonData[i]["Position"]["y"].ToString());
                    var pZ = Convert.ToSingle(jsonData[i]["Position"]["z"].ToString());
                    var position = new Vector3(pX, pY, pZ);

                    var rX = Convert.ToSingle(jsonData[i]["Rotation"]["x"].ToString());
                    var rY = Convert.ToSingle(jsonData[i]["Rotation"]["y"].ToString());
                    var rZ = Convert.ToSingle(jsonData[i]["Rotation"]["z"].ToString());
                    var rW = Convert.ToSingle(jsonData[i]["Rotation"]["w"].ToString());
                    var rotation = new Quaternion(rX, rY, rZ, rW);

                    var box2DInfo = jsonData[i]["Box2DInfo"];
                    if (box2DInfo != null)
                    {
                        var oX = Convert.ToSingle(jsonData[i]["Box2DInfo"]["Offset"]["x"].ToString());
                        var oY = Convert.ToSingle(jsonData[i]["Box2DInfo"]["Offset"]["y"].ToString());
                        var offset = new Vector2(oX, oY);
                        var eX = Convert.ToSingle(jsonData[i]["Box2DInfo"]["Extent"]["x"].ToString());
                        var eY = Convert.ToSingle(jsonData[i]["Box2DInfo"]["Extent"]["y"].ToString());
                        var extent = new Vector2(eX, eY);
                        DrawSceneCaptureDataBox2D(uuid, semantic, position, rotation, offset, extent);
                    }

                    var box3DInfo = jsonData[i]["Box3DInfo"];
                    if (box3DInfo != null)
                    {
                        var oX = Convert.ToSingle(jsonData[i]["Box3DInfo"]["Offset"]["x"].ToString());
                        var oY = Convert.ToSingle(jsonData[i]["Box3DInfo"]["Offset"]["y"].ToString());
                        var oZ = Convert.ToSingle(jsonData[i]["Box3DInfo"]["Offset"]["z"].ToString());
                        var offset = new Vector3(oX, oY, oZ);
                        var eX = Convert.ToSingle(jsonData[i]["Box3DInfo"]["Extent"]["x"].ToString());
                        var eY = Convert.ToSingle(jsonData[i]["Box3DInfo"]["Extent"]["y"].ToString());
                        var eZ = Convert.ToSingle(jsonData[i]["Box3DInfo"]["Extent"]["z"].ToString());
                        var extent = new Vector3(eX, eY, eZ);
                        DrawSceneCaptureDataBox3D(uuid, semantic, position, rotation, offset, extent);
                    }
                }
            }
        }

        private void DrawSceneCaptureDataBox2D(string uuid, PxrSemanticLabel label, Vector3 position, Quaternion rotation, Vector2 offset, Vector2 extent)
        {
            if (box2DPrefab != null)
            {
                var sceneAnchor = new GameObject(uuid);
                var box2D = Instantiate(box2DPrefab);
                box2D.transform.localScale = new Vector3(extent.x, extent.y, 0);
                box2D.transform.SetParent(sceneAnchor.transform);
                sceneAnchor.transform.rotation = rotation;
                sceneAnchor.transform.position = position;
            }
        }

        private void DrawSceneCaptureDataBox3D(string uuid, PxrSemanticLabel label, Vector3 position, Quaternion rotation, Vector3 offset, Vector3 extent)
        {
            if (box3DPrefab != null)
            {
                var sceneAnchor = new GameObject(uuid);
                var box3D = Instantiate(box3DPrefab);
                //currently,rotation not support
                box3D.transform.localPosition = offset;
                box3D.transform.localScale = extent;
                box3D.transform.SetParent(sceneAnchor.transform);
                sceneAnchor.transform.rotation = rotation;
                sceneAnchor.transform.position = position;
            }
        }
#endif
}
