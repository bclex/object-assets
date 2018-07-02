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
            var wrldsByLabel = Groups["WRLD"].GroupsByLabel;
            _WRLDsById = Groups["WRLD"].Load().Cast<WRLDRecord>().ToDictionary(x => x.Id, x =>
            {
                wrldsByLabel.TryGetValue(BitConverter.GetBytes(x.Id), out var wrlds);
                return new Tuple<WRLDRecord, RecordGroup[]>(x, wrlds);
            });
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
            var cellBlockX = (short)cellId.x >> 8;
            var cellBlockY = (short)cellId.y >> 8;
            var cellBlockId = new byte[4];
            Buffer.BlockCopy(BitConverter.GetBytes(cellBlockY), 0, cellBlockId, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(cellBlockX), 0, cellBlockId, 2, 2);
            foreach (var wrld in world.Item2)
            {
                wrld.Load();
                if (wrld.GroupsByLabel.TryGetValue(cellBlockId, out var cellBlock))
                {
                    var cellSubBlockX = ((ushort)cellId.x >> 6) % 4;
                    var cellSubBlockY = ((ushort)cellId.y >> 6) % 4;
                    var cellSubBlockId = new byte[4];
                    Buffer.BlockCopy(BitConverter.GetBytes(cellSubBlockY), 0, cellSubBlockId, 0, 2);
                    Buffer.BlockCopy(BitConverter.GetBytes(cellSubBlockX), 0, cellSubBlockId, 2, 2);
                    foreach (var i1 in cellBlock)
                    {
                        i1.Load();
                        if (i1.GroupsByLabel.TryGetValue(cellSubBlockId, out var cellSubBlock))
                        {
                            var cellX = (ushort)cellId.x % 64;
                            var cellY = (ushort)cellId.y % 64;
                            foreach (var i2 in cellSubBlock)
                            {
                                i2.Load();
                                var cell = i2.Records.Cast<CELLRecord>().SingleOrDefault(x => x.XCLC.Value.GridX == cellX && x.XCLC.Value.GridY == cellY);
                                return cell;
                            }
                            return null;
                        }
                    }
                }
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