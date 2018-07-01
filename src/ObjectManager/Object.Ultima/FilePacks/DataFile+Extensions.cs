using OA.Ultima.FilePacks.Records;
using UnityEngine;

namespace OA.Ultima.FilePacks
{
    partial class DataFile
    {
        public LANDRecord FindLANDRecord(Vector3Int cellId)
        {
            _LANDsById.TryGetValue(cellId, out LANDRecord land);
            return land;
        }

        public CELLRecord FindCellRecord(Vector3Int cellId)
        {
            _CELLsById.TryGetValue(cellId, out CELLRecord cell);
            return cell;
        }

        public CELLRecord FindCellRecordByName(int worldId, int cellId, string cellName)
        {
            return null;
            //_CELLsByName.TryGetValue(cellName, out var cell);
            //return cell;
        }
    }
}