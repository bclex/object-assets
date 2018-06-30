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
        Vector2i GetExteriorCellId(Vector3 point);
        InRangeCellInfo StartCreatingExteriorCell(Vector2i cellId);
        //InRangeCellInfo StartCreatingInteriorCell(FormId<CELLRecord> cellId);
        InRangeCellInfo StartCreatingInteriorCell(Vector2i gridId);
        void UpdateExteriorCells(Vector3 currentPosition, bool immediate = false, int cellRadiusOverride = -1);
        void DestroyAllCells();
    }
}