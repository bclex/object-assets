using OA.Core;
using System;
using UnityEngine;

namespace OA.Ultima.Formats
{
    public class StaObjectBuilder
    {
        readonly SiFile _file;
        readonly MaterialManager _materialManager;
        readonly int _markerLayer;

        public StaObjectBuilder(SiFile file, MaterialManager materialManager, int markerLayer)
        {
            _file = file;
            _materialManager = materialManager;
            _markerLayer = markerLayer;
        }

        public GameObject BuildObject()
        {
            Debug.Assert(_file.Name != null);

            var rootSiObject = _file.Blocks[0];
            var gameObject = InstantiateRootSiObject(rootSiObject);
            // If the file doesn't contain any StObjects we are looking for, return an empty GameObject.
            if (gameObject == null)
            {
                Utils.Log($"{_file.Name} resulted in a null GameObject when instantiated.");
                gameObject = new GameObject(_file.Name);
            }
            // If gameObject != null and the root SiObject is an SiNode, discard any transformations.
            else if (rootSiObject is SiNode)
            {
                gameObject.transform.position = Vector3.zero;
                gameObject.transform.rotation = Quaternion.identity;
                gameObject.transform.localScale = Vector3.one;
            }
            return gameObject;
        }

        private GameObject InstantiateRootSiObject(SiObject obj)
        {
            var gameObject = InstantiateSiObject(obj);
            ProcessExtraData(obj, out bool shouldAddMissingColliders, out bool isMarker);
            if (_file.Name != null && IsMarkerFileName(_file.Name))
            {
                shouldAddMissingColliders = false;
                isMarker = true;
            }
            // Add colliders to the object if it doesn't already contain one.
            if (shouldAddMissingColliders && gameObject.GetComponentInChildren<Collider>() == null)
                GameObjectUtils.AddMissingMeshCollidersRecursively(gameObject);
            if (isMarker)
                GameObjectUtils.SetLayerRecursively(gameObject, _markerLayer);
            return gameObject;
        }

        private void ProcessExtraData(SiObject obj, out bool shouldAddMissingColliders, out bool isMarker)
        {
            shouldAddMissingColliders = true;
            isMarker = false;
            if (obj is SiObjectNET objNET)
            {
                var extraData = objNET.ExtraData.Value >= 0 ? (SiExtraData)_file.Blocks[objNET.ExtraData.Value] : null;
                while (extraData != null)
                {
                    if (extraData is SiStringExtraData strExtraData)
                    {
                        if (strExtraData.Str == "NCO" || strExtraData.Str == "NCC")
                            shouldAddMissingColliders = false;
                        else if (strExtraData.Str == "MRK")
                        {
                            shouldAddMissingColliders = false;
                            isMarker = true;
                        }
                    }
                    // Move to the next NiExtraData.
                    if (extraData.NextExtraData.Value >= 0) extraData = (SiExtraData)_file.Blocks[extraData.NextExtraData.Value];
                    else extraData = null;
                }
            }
        }

        /// <summary>
        /// Creates a GameObject representation of an NiObject.
        /// </summary>
        /// <returns>Returns the created GameObject, or null if the NiObject does not need its own GameObject.</returns>
        private GameObject InstantiateSiObject(SiObject obj)
        {
            if (obj.GetType() == typeof(SiNode)) return InstantiateSiNode((SiNode)obj);
            else if (obj.GetType() == typeof(SiPrimitive)) return InstantiateSiPrimitive((SiPrimitive)obj, true, false);
            else if (obj.GetType() == typeof(SiTriShape)) return InstantiateSiTriShape((SiTriShape)obj, true, false);
            else throw new NotImplementedException($"Tried to instantiate an unsupported SiObject ({obj.GetType().Name}).");
        }

        private GameObject InstantiateSiNode(SiNode node)
        {
            var obj = new GameObject(node.Name);
            foreach (var childIndex in node.Children)
                // SiNodes can have child references < 0 meaning null.
                if (!childIndex.IsNull)
                {
                    var child = InstantiateSiObject(_file.Blocks[childIndex.Value]);
                    if (child != null)
                        child.transform.SetParent(obj.transform, false);
                }
            ApplySiAVObject(node, obj);
            return obj;
        }

        private GameObject InstantiateSiPrimitive(SiPrimitive primitive, bool visual, bool collidable)
        {
            var game = UltimaSettings.Game;
            Debug.Assert(visual || collidable);
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = primitive.Name;
            if (visual)
            {
                var materialProps = SiAVObjectPropertiesToMaterialProperties(primitive);
                var meshRenderer = obj.GetComponent<MeshRenderer>();
                meshRenderer.material = _materialManager.BuildMaterialFromProperties(materialProps);
                if (Utils.ContainsBitFlags(primitive.Flags, (uint)SiAVObject.SiFlags.Hidden))
                    meshRenderer.enabled = false;
                obj.isStatic = true;
            }
            if (collidable)
            {
                if (game.KinematicRigidbodies)
                    obj.AddComponent<Rigidbody>().isKinematic = true;
            }
            ApplySiAVObject(primitive, obj);
            return obj;
        }

        private GameObject InstantiateSiTriShape(SiTriShape triShape, bool visual, bool collidable)
        {
            var game = UltimaSettings.Game;
            Debug.Assert(visual || collidable);
            var mesh = SiTriShapeDataToMesh((SiTriShapeData)_file.Blocks[triShape.Data.Value]);
            var obj = new GameObject(triShape.Name);
            if (visual)
            {
                obj.AddComponent<MeshFilter>().mesh = mesh;
                var materialProps = SiAVObjectPropertiesToMaterialProperties(triShape);
                var meshRenderer = obj.AddComponent<MeshRenderer>();
                meshRenderer.material = _materialManager.BuildMaterialFromProperties(materialProps);
                if (Utils.ContainsBitFlags(triShape.Flags, (uint)SiAVObject.SiFlags.Hidden))
                    meshRenderer.enabled = false;
                obj.isStatic = true;
            }
            if (collidable)
            {
                obj.AddComponent<MeshCollider>().sharedMesh = mesh;
                if (game.KinematicRigidbodies)
                    obj.AddComponent<Rigidbody>().isKinematic = true;
            }
            ApplySiAVObject(triShape, obj);
            return obj;
        }

        private void ApplySiAVObject(SiAVObject siAVObject, GameObject obj)
        {
            obj.transform.position = StaUtils.StaPointToUnityPoint(siAVObject.Translation);
            obj.transform.rotation = StaUtils.StaRotationMatrixToUnityQuaternion(siAVObject.Rotation);
            obj.transform.localScale = siAVObject.Scale * Vector3.one;
        }

        private Mesh SiTriShapeDataToMesh(SiTriShapeData data)
        {
            return null;
        }

        private MaterialProps SiAVObjectPropertiesToMaterialProperties(SiObject na)
        {
            var mp = new MaterialProps
            {
                alphaBlended = false,
                alphaTest = false,
                textures = new MaterialTextures
                {
                    mainFilePath = _file.Name,
                }
            };
            return mp;
        }

        private bool IsMarkerFileName(string name)
        {
            return false;
        }
    }
}