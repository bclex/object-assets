using OA.Core;
using OA.Tes.FilePacks.Records;
using System;
using System.Linq;
using System.Collections.Generic;

namespace OA.Tes.FilePacks
{
    partial class EsmFile
    {
        // TES3
        internal Dictionary<string, IRecord> _MANYsById;
        Dictionary<long, LTEXRecord> _LTEXsById;
        Dictionary<Vector2i, CELLRecord> _CELLsById;
        Dictionary<Vector2i, LANDRecord> _LANDsById;

        // TES4
        public Dictionary<string, LTEXRecord> _ltexsByEid;

        void Process()
        {
            if (Format == GameFormatId.TES3)
            {
                var groups = new List<Record>[] { Groups.ContainsKey("STAT") ? Groups["STAT"].Load() : null };
                _MANYsById = groups.SelectMany(x => x).Cast<IHaveEDID>().Where(x => x != null).ToDictionary(x => x.EDID.Value, x => (IRecord)x);
                _LTEXsById = Groups.ContainsKey("LTEX") ? Groups["LTEX"].Load().Cast<LTEXRecord>().ToDictionary(x => x.INTV.Value) : null;
                _CELLsById = Groups.ContainsKey("CELL") ? Groups["CELL"].Load().Cast<CELLRecord>().Where(x => !x.IsInterior).ToDictionary(x => x.GridCoords) : null;
                _LANDsById = Groups.ContainsKey("LAND") ? Groups["LAND"].Load().Cast<LANDRecord>().ToDictionary(x => x.GridCoords) : null;
                return;
            }
            _ltexsByEid = Groups["LTEX"].Load().Cast<LTEXRecord>().ToDictionary(x => x.EDID.Value);
        }

        public LTEXRecord FindLTEXRecord(int index)
        {
            if (Format == GameFormatId.TES3)
            {
                _LTEXsById.TryGetValue(index, out var ltex);
                return ltex;
            }
            throw new NotImplementedException();
        }

        public LANDRecord FindLANDRecord(Vector2i cellId)
        {
            if (Format == GameFormatId.TES3)
            {
                _LANDsById.TryGetValue(cellId, out var land);
                return land;
            }
            throw new NotImplementedException();
        }

        public CELLRecord FindExteriorCellRecord(Vector2i cellId)
        {
            if (Format == GameFormatId.TES3)
            {
                _CELLsById.TryGetValue(cellId, out var cell);
                return cell;
            }
            throw new NotImplementedException();
        }

        public CELLRecord FindInteriorCellRecord(FormId<CELLRecord> cellId)
        {
            throw new NotImplementedException();
            //    var records = GetRecordsOfType<CELLRecord>();
            //    CELLRecord cell = null;
            //    for (int i = 0, l = records.Count; i < l; i++)
            //    {
            //        cell = (CELLRecord)records[i];
            //        if (cell.EDID.Value == cellName)
            //            return cell;
            //    }
            //    return null;
        }

        public CELLRecord FindInteriorCellRecord(Vector2i gridCoords)
        {
            throw new NotImplementedException();
            //var records = GetRecordsOfType<CELLRecord>();
            //CELLRecord cell = null;
            //for (int i = 0, l = records.Count; i < l; i++)
            //{
            //    cell = (CELLRecord)records[i];
            //    if (cell.GridCoords.x == gridCoords.x && cell.GridCoords.y == gridCoords.y)
            //        return cell;
            //}
            //return null;
        }
    }
}