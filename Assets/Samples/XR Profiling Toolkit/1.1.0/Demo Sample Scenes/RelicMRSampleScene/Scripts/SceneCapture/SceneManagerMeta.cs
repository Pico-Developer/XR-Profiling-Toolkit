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
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using MRDemoSampleScene.Runtime.Data;
using MRDemoSampleScene.Runtime.Entity;
using Unity.Collections;


public class SceneManagerMeta : MonoBehaviour
{
    private const string TAG = "[SceneManagerMeta]";
    public static SceneManagerMeta Instance;
    
    public GameObject box2DPrefab;
    public GameObject box3DPrefab;
    public GameObject roomMeshPrefab;
    public float UpdateFrequencySeconds = 5;
    [SerializeField] private Transform _trackingSpace;
#if OCULUS_PERFORMANCE
    List<(GameObject, OVRLocatable)> _locatableObjects = new List<(GameObject, OVRLocatable)>();
#endif
    public bool IsSceneCaptureFinished { get; private set; }
    private GameObject mainTestObject;
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
    
#if OCULUS_PERFORMANCE
    IEnumerator UpdateAnchorsPeriodically()
    {
        while (true)
        {
            foreach (var (gameObject, locatable) in _locatableObjects)
            {
                SetLocation(gameObject,locatable);
            }
            yield return new WaitForSeconds(UpdateFrequencySeconds);
        }
    }

    private void SetLocation(GameObject anchorGameObject,OVRLocatable locatable)
    {
        if (!locatable.TryGetSceneAnchorPose(out var pose))
            return;

        var position = pose.ComputeWorldPosition(_trackingSpace);
        var rotation = pose.ComputeWorldRotation(_trackingSpace);

        if (position != null && rotation != null)
            anchorGameObject.transform.SetPositionAndRotation(
                position.Value, rotation.Value);
    }
#endif   
    // Start is called before the first frame update
    void Start()
    {
        IsSceneCaptureFinished = false;
        
#if UNITY_EDITOR
        FindTableItem();
#endif

        StartSceneCaptureProvider();
#if OCULUS_PERFORMANCE
        StartCoroutine(UpdateAnchorsPeriodically());
#endif
    }
    private async void StartSceneCaptureProvider()
    {
        
#if OCULUS_PERFORMANCE
        if (!await HasQueryableSceneModel())
            return;
        IsSceneCaptureFinished = true;
        // fetch all rooms by querying for all anchors with the room layout component
        var rooms = new List<OVRAnchor>();
        await OVRAnchor.FetchAnchorsAsync(rooms, new OVRAnchor.FetchOptions
        {
            SingleComponentType = typeof(OVRRoomLayout),
        });

        // fetch room elements, create objects for them asynchronously
        var tasks = rooms.Select(async room =>
        {
            var roomObject = new GameObject($"Room-{room.Uuid}");
            if (!room.TryGetComponent(out OVRAnchorContainer container))
                return;
            if (!room.TryGetComponent(out OVRRoomLayout roomLayout))
                return;
            var children = new List<OVRAnchor>();
            await container.FetchAnchorsAsync(children);
            await CreateSceneObjects(roomObject, children);
        });
        await Task.WhenAll(tasks);
        await FindTableItem();

    }
    async Task CreateSceneObjects(GameObject roomGameObject, List<OVRAnchor> anchors)
    {
        // we create tasks to iterate through all anchors asynchronously
        var tasks = anchors.Select(async anchor =>
        {
            // can we locate it in the world?
            if (!anchor.TryGetComponent(out OVRLocatable locatable))
                return;
            await locatable.SetEnabledAsync(true);

            // get semantic classification for object name
            var classifications = new HashSet<OVRSemanticLabels.Classification>
            {
                OVRSemanticLabels.Classification.Other
            };
            if (anchor.TryGetComponent(out OVRSemanticLabels labels))
                labels.GetClassifications(classifications);

            
            var gameObject = new GameObject(string.Join(',', classifications));
            gameObject.transform.SetParent(roomGameObject.transform);
            SetLocation(gameObject,locatable);
            // activate and populate Unity object with Scene object data
            // different objects have different data (volumes, planes, meshes)
            if (anchor.TryGetComponent(out OVRTriangleMesh trimesh) && trimesh.IsEnabled)
            {
                var gObj = Instantiate(roomMeshPrefab, gameObject.transform);
                // set pose of object
                if (trimesh.TryGetCounts(out var vertexCount, out var triangleCount))
                {
                    using var vertices = new NativeArray<Vector3>(vertexCount, Allocator.Temp);
                    using var triangles = new NativeArray<int>(triangleCount * 3, Allocator.Temp);

                    if (trimesh.TryGetMesh(vertices, triangles))
                    {
                        var mesh = new Mesh { indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
                        mesh.SetVertices(vertices);
                        mesh.SetTriangles(triangles.ToArray(), 0);
                        mesh.RecalculateNormals();

                        gObj.GetComponent<MeshFilter>().mesh = mesh;
                        gObj.GetComponent<MeshCollider>().sharedMesh = mesh;
                    }
                }
            }
            if (anchor.TryGetComponent(out OVRBounded3D bounds3D) && bounds3D.IsEnabled)
            {
                var gObj = Instantiate(box3DPrefab, gameObject.transform);
                gObj.transform.localPosition = bounds3D.BoundingBox.center;
                gObj.transform.localScale = bounds3D.BoundingBox.size;
            }
            if (anchor.TryGetComponent(out OVRBounded2D bounds2D) && bounds2D.IsEnabled)
            {
                var gObj = Instantiate(box2DPrefab, gameObject.transform);
                gObj.transform.localScale = new Vector3(
                    bounds2D.BoundingBox.size.x,
                    bounds2D.BoundingBox.size.y,
                    0.01f);
            }
            _locatableObjects.Add((gameObject,locatable));
        });
        await Task.WhenAll(tasks);
    }
    async OVRTask<bool> HasQueryableSceneModel()
    {
        // check Spatial Data permission
        const string permission = "com.oculus.permission.USE_SCENE";
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(permission))
        {
            Debug.LogError("Spatial Data permission has not been granted. " +
                           "Use OVRCameraRig's OVRManager Permission Requests On Startup " +
                           "to perform the runtime permission request, or use " +
                           "Unity's Android Permission API.");
            return false;
        }

        // check that we have room data
        var rooms = new List<OVRAnchor>();
        await OVRAnchor.FetchAnchorsAsync(rooms, new OVRAnchor.FetchOptions
        {
            SingleComponentType = typeof(OVRRoomLayout),
        });
        if (rooms.Count != 0)
            return true;

#if UNITY_EDITOR
        Debug.LogError("No Scene Model found. " +
                       "When using Meta Quest Link, ensure that you have enabled " +
                       "Spatial Data over Meta Quest Link (Settings > Beta).\n" +
                       "If you have not yet captured a Scene Model, run Space Setup " +
                       "on-device, as doing this on Meta Quest Link is not supported");
#endif
        return await OVRScene.RequestSpaceSetup();
    }
#endif
    private async Task FindTableItem()
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
            await EntityManager.Instance.CreateEntity(mainTestObject);
        }
        if (_locatableObjects.Count != 0)
        {
            foreach (var locatableObject in _locatableObjects)
            {
                if (locatableObject.Item1.name.Contains("Table"))
                {
                    mainTestObject.transform.position = locatableObject.Item1.transform.position;
                    return;
                }
            }
        }
    }
}
