using OA.Core;
using OA.Tes.FilePacks.Records;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace OA.Tes.FilePacks
{
    partial class EsmFile
    {
        // TES3
        internal Dictionary<string, IRecord> _MANYsById;
        Dictionary<long, LTEXRecord> _LTEXsById;
        Dictionary<Vector3Int, LANDRecord> _LANDsById;
        Dictionary<Vector3Int, CELLRecord> _CELLsById;
        Dictionary<string, CELLRecord> _CELLsByName;

        // TES4
        public Dictionary<uint, Tuple<WRLDRecord, RecordGroup[]>> _WRLDsById;
        public Dictionary<string, LTEXRecord> _LTEXsByEid;

        void Process()
        {
            if (Format == GameFormatId.TES3)
            {
                var groups = new List<Record>[] { Groups.ContainsKey("STAT") ? Groups["STAT"].Load() : null };
                _MANYsById = groups.SelectMany(x => x).Cast<IHaveEDID>().Where(x => x != null).ToDictionary(x => x.EDID.Value, x => (IRecord)x);
                _LTEXsById = Groups["LTEX"].Load().Cast<LTEXRecord>().ToDictionary(x => x.INTV.Value);
                _LANDsById = Groups["LAND"].Load().Cast<LANDRecord>().ToDictionary(x => x.GridId);
                var cells = Groups["CELL"].Load().Cast<CELLRecord>().ToList();
                _CELLsById = cells.Where(x => !x.IsInterior).ToDictionary(x => x.GridId);
                _CELLsByName = cells.Where(x => x.IsInterior).ToDictionary(x => x.EDID.Value);
                return;
            }
            var wrldsByLabel = Groups["WRLD"].Groups.GroupBy(x => BitConverter.ToUInt32(Encoding.ASCII.GetBytes(x.Label), 0)).ToDictionary(x => x.Key, x => x.ToArray());
            _WRLDsById = Groups["WRLD"].Load().Cast<WRLDRecord>().ToDictionary(x => x.Id, x => new Tuple<WRLDRecord, RecordGroup[]>(x, wrldsByLabel.ContainsKey(x.Id) ? wrldsByLabel[x.Id] : null));
            _LTEXsByEid = Groups["LTEX"].Load().Cast<LTEXRecord>().ToDictionary(x => x.EDID.Value);
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

        public LANDRecord FindLANDRecord(Vector3Int cellId)
        {
            if (Format == GameFormatId.TES3)
            {
                _LANDsById.TryGetValue(cellId, out var land);
                return land;
            }
            throw new NotImplementedException();
        }

        public CELLRecord FindCellRecord(Vector3Int cellId)
        {
            if (Format == GameFormatId.TES3)
            {
                _CELLsById.TryGetValue(cellId, out var cell);
                return cell;
            }
            var world = _WRLDsById[(uint)cellId.z];
            foreach (var i in world.Item2)
            {
                var x = i.Load();
            }
            return null;
        }

        public CELLRecord FindCellRecordByName(int worldId, int cellId, string cellName)
        {
            if (Format == GameFormatId.TES3)
            {
                _CELLsByName.TryGetValue(cellName, out var cell);
                return cell;
            }
            throw new NotImplementedException();
        }
    }
}