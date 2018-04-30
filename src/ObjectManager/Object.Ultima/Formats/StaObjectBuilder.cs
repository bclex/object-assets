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

            var gameObject = InstantiateStObject();
            // If the file doesn't contain any StObjects we are looking for, return an empty GameObject.
            if (gameObject == null)
            {
                Utils.Log($"{_file.Name} resulted in a null GameObject when instantiated.");
                gameObject = new GameObject(_file.Name);
            }
            return gameObject;
        }

        private GameObject InstantiateStObject()
        {
            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.name = _file.Name;
            return gameObject;
        }
    }
}