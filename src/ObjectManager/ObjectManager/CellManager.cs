using OA.Core;
using System.Collections;
using UnityEngine;

namespace OA
{
    public interface ICellRecord : IRecord
    {
        bool IsInterior { get; }
        Color? AmbientLight { get; }
    }

    public class InRangeCellInfo
    {
        public GameObject GameObject;
        public GameObject ObjectsContainerGameObject;
        public ICellRecord CellRecord;
        public IEnumerator ObjectsCreationCoroutine;

        public InRangeCellInfo(GameObject gameObject, GameObject objectsContainerGameObject, ICellRecord cellRecord, IEnumerator objectsCreationCoroutine)
        {
            GameObject = gameObject;
            ObjectsContainerGameObject = objectsContainerGameObject;
            CellRecord = cellRecord;
            ObjectsCreationCoroutine = objectsCreationCoroutine;
        }
    }

    public class RefCellObjInfo
    {
        public object RefObj; //: CELLRecord.RefObjDataGroup
        public IRecord ReferencedRecord;
        public string ModelFilePath;
    }

    public interface ICellManager
    {
        Vector3Int GetCellId(Vector3 point, int worldId);
        InRangeCellInfo StartCreatingCell(Vector3Int cellId);
        InRangeCellInfo StartCreatingCellByName(int worldId, int cellId, string cellName);
        void UpdateCells(Vector3 currentPosition, int worldId, bool immediate = false, int cellRadiusOverride = -1);
        void DestroyAllCells();
    }
}