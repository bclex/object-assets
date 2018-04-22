using OA.Core;
using System.Collections;
using UnityEngine;

namespace OA
{
    public interface ICellRecord : IRecord
    {
        bool IsInterior { get; }
    }

    public class InRangeCellInfo
    {
        public GameObject gameObject;
        public GameObject objectsContainerGameObject;
        public ICellRecord cellRecord;
        public IEnumerator objectsCreationCoroutine;

        public InRangeCellInfo(GameObject gameObject, GameObject objectsContainerGameObject, ICellRecord cellRecord, IEnumerator objectsCreationCoroutine)
        {
            this.gameObject = gameObject;
            this.objectsContainerGameObject = objectsContainerGameObject;
            this.cellRecord = cellRecord;
            this.objectsCreationCoroutine = objectsCreationCoroutine;
        }
    }

    public class RefCellObjInfo
    {
        public object refObjDataGroup; //: CELLRecord.RefObjDataGroup
        public IRecord referencedRecord;
        public string modelFilePath;
    }

    public interface ICellManager
    {
        InRangeCellInfo StartCreatingExteriorCell(Vector2i cellIndices);
        void UpdateExteriorCells(Vector3 currentPosition, bool immediate = false, int cellRadiusOverride = -1);
    }
}