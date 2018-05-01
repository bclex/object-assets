using OA.Core;
using UnityEngine;

namespace OA.Ultima.Formats
{
    public class StaObjectBuilder
    {
        readonly StFile _file;
        readonly MaterialManager _materialManager;

        public StaObjectBuilder(StFile file, MaterialManager materialManager)
        {
            _file = file;
            _materialManager = materialManager;
        }

        public GameObject BuildObject()
        {
            Debug.Assert(_file.Name != null);

            var rootStObject = _file.Blocks[0];
            var gameObject = InstantiateRootStaObject(rootStObject);
            // If the file doesn't contain any StObjects we are looking for, return an empty GameObject.
            if (gameObject == null)
            {
                Utils.Log($"{_file.Name} resulted in a null GameObject when instantiated.");
                gameObject = new GameObject(_file.Name);
            }
            return gameObject;
        }

        private GameObject InstantiateRootStaObject(StObject obj)
        {
            var gameObject = InstantiateStShape(obj, true, false);
            //ProcessExtraData(obj, out bool shouldAddMissingColliders, out bool isMarker);
            //if (file.name != null && IsMarkerFileName(file.name))
            //{
            //    shouldAddMissingColliders = false;
            //    isMarker = true;
            //}
            //// Add colliders to the object if it doesn't already contain one.
            //if (shouldAddMissingColliders && gameObject.GetComponentInChildren<Collider>() == null)
            //    GameObjectUtils.AddMissingMeshCollidersRecursively(gameObject);
            return gameObject;
        }

        private GameObject InstantiateStShape(StObject na, bool visual, bool collidable)
        {
            var game = UltimaSettings.Game;
            Debug.Assert(visual || collidable);
            var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = _file.Name;
            if (visual)
            {
                var materialProps = StAVObjectPropertiesToMaterialProperties(na);
                var meshRenderer = obj.AddComponent<MeshRenderer>();
                meshRenderer.material = _materialManager.BuildMaterialFromProperties(materialProps);
                meshRenderer.enabled = false;
                obj.isStatic = true;
            }
            if (collidable)
            {
                if (game.KinematicRigidbodies)
                    obj.AddComponent<Rigidbody>().isKinematic = true;
            }
            return obj;
        }

        private MaterialProps StAVObjectPropertiesToMaterialProperties(StObject na)
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
    }
}