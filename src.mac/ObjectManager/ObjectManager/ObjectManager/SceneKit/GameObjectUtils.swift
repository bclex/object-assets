//
//  GameObjectUtils.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import simd

public class GameObjectUtils {

//        public static GameObject CreateMainCamera(float3 position, Quaternion orientation)
//        {
//            var cameraObject = new GameObject("Main Camera") { tag = "MainCamera" };
//            cameraObject.AddComponent<Camera>();
//            cameraObject.AddComponent<FlareLayer>();
//            cameraObject.AddComponent<AudioListener>();
//            cameraObject.transform.position = position;
//            cameraObject.transform.rotation = orientation;
//            return cameraObject;
//        }
//
//        public static GameObject CreateDirectionalLight(float3 position, Quaternion orientation)
//        {
//            var light = new GameObject("Directional Light");
//            var lightComponent = light.AddComponent<Light>();
//            lightComponent.type = LightType.Directional;
//            light.transform.position = position;
//            light.transform.rotation = orientation;
//            return light;
//        }
    
    public class TerrainData {
        public let heightmapResolution: Int
        public var size: float3!
        public var heights: [[Float]]!
        public var splatPrototypes: [SplatPrototype]!
        public var alphamaps: [[[Float]]]?
        public var alphamapResolution: Int?
        
        public init(heightmapResolution: Int) {
            self.heightmapResolution = heightmapResolution
        }
        
        public func setHeights(_ x: Int, _ y: Int, _ z: [[Float]]) {
            heights = z
        }
        
        public func setAlphamaps(_ x: Int, _ y: Int, _ z: [[[Float]]]) {
            alphamaps = z
        }
    }
    
    public static func createTerrainData(offset: Int, heightPercents: [[Float]], maxHeight: Float, heightSampleDistance: Float, splatPrototypes: [SplatPrototype], alphaMap: [[[Float]]]?) -> TerrainData {
        assert(heightPercents.count == heightPercents[0].count && maxHeight >= 0 && heightSampleDistance >= 0)
        // Create the TerrainData.
        let heightmapResolution = heightPercents.count
        let terrainData = TerrainData(heightmapResolution: heightmapResolution)
        let terrainWidth = Float(heightmapResolution + offset) * heightSampleDistance
        // If maxHeight is 0, leave all the heights in terrainData at 0 and make the vertical size of the terrain 1 to ensure valid AABBs.
        if !Float.approximately(maxHeight, 0) {
            terrainData.size = float3(terrainWidth, maxHeight, terrainWidth)
            terrainData.setHeights(0, 0, heightPercents)
        }
        else { terrainData.size = float3(terrainWidth, 1, terrainWidth) }
        terrainData.splatPrototypes = splatPrototypes
        if alphaMap != nil {
            assert(alphaMap!.count == alphaMap![0].count)
            terrainData.alphamapResolution = alphaMap!.count
            terrainData.setAlphamaps(0, 0, alphaMap!)
        }
        return terrainData
    }

    public static func createTerrain(offset: Int, heightPercents: [[Float]], maxHeight: Float, heightSampleDistance: Float, splatPrototypes: [SplatPrototype], alphaMap: [[[Float]]]?, position: float3) -> GameObject {
        let terrainData = createTerrainData(offset: offset, heightPercents: heightPercents, maxHeight: maxHeight, heightSampleDistance: heightSampleDistance, splatPrototypes: splatPrototypes, alphaMap: alphaMap)
        return createTerrainFromTerrainData(terrainData: terrainData, position: position)
    }

    public static func createTerrainFromTerrainData(terrainData: TerrainData, position: float3) -> GameObject {
        // Create the terrain game object.
        let terrainObject = GameObject(name: "terrain") // { isStatic = true }
//        var terrain = terrainObject.AddComponent<Terrain>()
//        terrain.terrainData = terrainData
//        terrainObject.AddComponent<TerrainCollider>().terrainData = terrainData
//        terrainObject.transform.position = position
        return terrainObject
    }
//
//
//        public static Bounds CalcVisualBoundsRecursive(GameObject gameObject)
//        {
//            Debug.Assert(gameObject != null);
//
//            // Gets all the renderers in the object and it's descendants.
//            var renderers = gameObject.transform.GetComponentsInChildren<Renderer>();
//            if (renderers.Length > 0)
//            {
//                // Encapsulate the first renderer.
//                var visualBounds = renderers[0].bounds;
//                // Encapsulate the rest of the renderers.
//                for (var i = 1; i < renderers.Length; i++)
//                    visualBounds.Encapsulate(renderers[i].bounds);
//                return visualBounds;
//            }
//            // If there are no renderers in the object or any of it's children, simply return a degenerate AABB where the object is.
//            else return new Bounds(gameObject.transform.position, float3.zero);
//        }
//
//        public static GameObject FindChildRecursively(GameObject parent, string name)
//        {
//            var resultTransform = parent.transform.Find(name);
//            // Search through each of parent's children.
//            if (resultTransform != null)
//                return resultTransform.gameObject;
//            // Perform the search recursively for each child of parent.
//            foreach (Transform childTransform in parent.transform)
//            {
//                var result = FindChildRecursively(childTransform.gameObject, name);
//                if (result != null)
//                    return result;
//            }
//            return null;
//        }
//
//        public static GameObject FindChildWithNameSubstringRecursively(GameObject parent, string nameSubstring)
//        {
//            // Search through each of parent's children.
//            foreach (Transform childTransform in parent.transform)
//                if (childTransform.name.Contains(nameSubstring))
//                    return childTransform.gameObject;
//            // Perform the search recursively for each child of parent.
//            foreach (Transform childTransform in parent.transform)
//            {
//                var result = FindChildWithNameSubstringRecursively(childTransform.gameObject, nameSubstring);
//                if (result != null)
//                    return result;
//            }
//            return null;
//        }
//
//        public static GameObject FindObjectWithTagUpHeirarchy(GameObject gameObject, string tag)
//        {
//            while (gameObject != null)
//            {
//                if (gameObject.tag == tag)
//                    return gameObject;
//                // Go up one level in the object hierarchy.
//                var parentTransform = gameObject.transform.parent;
//                gameObject = parentTransform?.gameObject;
//            }
//            return null;
//        }
//
//        public static GameObject FindTopLevelObject(GameObject baseObject)
//        {
//            if (baseObject.transform.parent == null)
//                return baseObject;
//            var p = baseObject.transform;
//            while (p.parent != null)
//            {
//                if (p.parent.gameObject.name == "objects")
//                    break;
//                p = p.parent;
//            }
//            return p.gameObject;
//        }
//
//        public static void SetLayerRecursively(GameObject gameObject, int layer)
//        {
//            gameObject.layer = layer;
//            foreach (Transform childTransform in gameObject.transform)
//                SetLayerRecursively(childTransform.gameObject, layer);
//        }
//
//
//        public static void AddMissingMeshCollidersRecursively(GameObject gameObject)
//        {
//            // If gameObject has a MeshFilter but no Collider, add a MeshCollider.
//            if (gameObject.GetComponent<Collider>() == null)
//            {
//                var meshFilter = gameObject.GetComponent<MeshFilter>();
//                if (meshFilter != null && meshFilter.mesh != null)
//                    gameObject.AddComponent<MeshCollider>();
//            }
//            // Perform the above procedure on gameObject's children recursively.
//            foreach (Transform childTransform in gameObject.transform)
//                AddMissingMeshCollidersRecursively(childTransform.gameObject);
//        }
}
