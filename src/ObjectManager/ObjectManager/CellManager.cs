using System.Collections;
using UnityEngine;

namespace OA
{
    public class InRangeCellInfo
    {
        public GameObject gameObject;
        public GameObject objectsContainerGameObject;
        public IRecord cellRecord;
        public IEnumerator objectsCreationCoroutine;

        public InRangeCellInfo(GameObject gameObject, GameObject objectsContainerGameObject, IRecord cellRecord, IEnumerator objectsCreationCoroutine)
        {
            this.gameObject = gameObject;
            this.objectsContainerGameObject = objectsContainerGameObject;
            this.cellRecord = cellRecord;
            this.objectsCreationCoroutine = objectsCreationCoroutine;
        }
    }

    public class RefCellObjInfo
    {
        public object refObjDataGroup;
        public IRecord referencedRecord;
        public string modelFilePath;
    }
}