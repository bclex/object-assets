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
                if (ltex.INTV.value == index)
                    return ltex;
            }
            return null;
        }

        public LANDRecord FindLANDRecord(Vector2i cellIndices)
        {
            LANDRecord land;
            LANDRecordsByIndices.TryGetValue(cellIndices, out land);
            return land;
        }

        public CELLRecord FindExteriorCellRecord(Vector2i cellIndices)
        {
            CELLRecord cell;
            exteriorCELLRecordsByIndices.TryGetValue(cellIndices, out cell);
            return cell;
        }

        public CELLRecord FindInteriorCellRecord(string cellName)
        {
            var records = GetRecordsOfType<CELLRecord>();
            CELLRecord cell = null;
            for (int i = 0, l = records.Count; i < l; i++)
            {
                cell = (CELLRecord)records[i];
                if (cell.NAME.value == cellName)
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
                if (cell.gridCoords.x == gridCoords.x && cell.gridCoords.y == gridCoords.y)
                    return cell;
            }
            return null;
        }
    }
}