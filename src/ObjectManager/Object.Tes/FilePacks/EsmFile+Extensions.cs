using OA.Core;
using OA.Tes.FilePacks.Records;

namespace OA.Tes.FilePacks
{
    partial class EsmFile
    {
        public LTEXRecord FindLTEXRecord(int index)
        {
            var records = GetRecordsOfType<LTEXRecord>();
            LTEXRecord ltex = null;
            for (int i = 0, l = records.Count; i < l; i++)
            {
                ltex = (LTEXRecord)records[i];
                if (ltex.INTV.Value == index)
                    return ltex;
            }
            return null;
        }

        public LANDRecord FindLANDRecord(Vector2i cellIndices)
        {
            LANDRecordsByIndices.TryGetValue(cellIndices, out LANDRecord land);
            return land;
        }

        public CELLRecord FindExteriorCellRecord(Vector2i cellIndices)
        {
            exteriorCELLRecordsByIndices.TryGetValue(cellIndices, out CELLRecord cell);
            return cell;
        }

        public CELLRecord FindInteriorCellRecord(string cellName)
        {
            var records = GetRecordsOfType<CELLRecord>();
            CELLRecord cell = null;
            for (int i = 0, l = records.Count; i < l; i++)
            {
                cell = (CELLRecord)records[i];
                if (cell.NAME.Value == cellName)
                    return cell;
            }
            return null;
        }

        public CELLRecord FindInteriorCellRecord(Vector2i gridCoords)
        {
            var records = GetRecordsOfType<CELLRecord>();
            CELLRecord cell = null;
            for (int i = 0, l = records.Count; i < l; i++)
            {
                cell = (CELLRecord)records[i];
                if (cell.GridCoords.x == gridCoords.x && cell.GridCoords.y == gridCoords.y)
                    return cell;
            }
            return null;
        }
    }
}