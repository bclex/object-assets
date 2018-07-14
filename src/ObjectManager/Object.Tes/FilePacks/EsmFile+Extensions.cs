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
        internal Dictionary<string, Record> _MANYsById;
        Dictionary<long, LTEXRecord> _LTEXsById;
        Dictionary<Vector3Int, LANDRecord> _LANDsById;
        Dictionary<Vector3Int, CELLRecord> _CELLsById;
        Dictionary<string, CELLRecord> _CELLsByName;

        // TES4
        Dictionary<uint, Tuple<WRLDRecord, RecordGroup[]>> _WRLDsById;
        Dictionary<string, LTEXRecord> _LTEXsByEid;

        void Process()
        {
            if (Format == GameFormatId.TES3)
            {
                var manyGroups = new List<Record>[] { Groups.ContainsKey("STAT") ? Groups["STAT"].Load() : null };
                _MANYsById = manyGroups.SelectMany(x => x).Cast<IHaveEDID>().Where(x => x != null).ToDictionary(x => x.EDID.Value, x => (Record)x);
                _LTEXsById = Groups["LTEX"].Load().Cast<LTEXRecord>().ToDictionary(x => x.INTV.Value);
                _LANDsById = Groups["LAND"].Load().Cast<LANDRecord>().ToDictionary(x => x.GridId);
                var cells = Groups["CELL"].Load().Cast<CELLRecord>().ToList();
                foreach (var cell in cells)
                    cell.GridId = new Vector3Int(cell.XCLC.Value.GridX, cell.XCLC.Value.GridY, !cell.IsInterior ? 0 : -1);
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
            var world = _WRLDsById[(uint)cellId.z];
            foreach (var wrld in world.Item2)
                foreach (var cellBlock in wrld.EnsureWrldAndCell(cellId))
                    if (cellBlock.LANDsById.TryGetValue(cellId, out var land))
                        return land;
            return null;
        }

        public CELLRecord FindCellRecord(Vector3Int cellId)
        {
            if (Format == GameFormatId.TES3)
            {
                _CELLsById.TryGetValue(cellId, out var cell);
                return cell;
            }
            var world = _WRLDsById[(uint)cellId.z];
            foreach (var wrld in world.Item2)
                foreach (var cellBlock in wrld.EnsureWrldAndCell(cellId))
                    if (cellBlock.CELLsById.TryGetValue(cellId, out var cell))
                        return cell;
            return null;
        }

        public CELLRecord FindCellRecordByName(int world, int cellId, string cellName)
        {
            if (Format == GameFormatId.TES3)
            {
                _CELLsByName.TryGetValue(cellName, out var cell);
                return cell;
            }
            throw new NotImplementedException();
        }
    }

    partial class RecordGroup
    {
        internal HashSet<byte[]> _ensureCELLsByLabel;
        internal Dictionary<Vector3Int, CELLRecord> CELLsById;
        internal Dictionary<Vector3Int, LANDRecord> LANDsById;

        public RecordGroup[] EnsureWrldAndCell(Vector3Int cellId)
        {
            var cellBlockX = (short)(cellId.x >> 5);
            var cellBlockY = (short)(cellId.y >> 5);
            var cellBlockId = new byte[4];
            Buffer.BlockCopy(BitConverter.GetBytes(cellBlockY), 0, cellBlockId, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(cellBlockX), 0, cellBlockId, 2, 2);
            Load();
            if (GroupsByLabel.TryGetValue(cellBlockId, out var cellBlocks))
                return cellBlocks.Select(x => x.EnsureCell(cellId)).ToArray();
            return null;
        }

        //  = nxn[nbits] + 4x4[2bits] + 8x8[3bit]
        public RecordGroup EnsureCell(Vector3Int cellId)
        {
            if (_ensureCELLsByLabel == null)
                _ensureCELLsByLabel = new HashSet<byte[]>(ByteArrayComparer.Default);
            var cellBlockX = (short)(cellId.x >> 5);
            var cellBlockY = (short)(cellId.y >> 5);
            var cellSubBlockX = (short)(cellId.x >> 3);
            var cellSubBlockY = (short)(cellId.y >> 3);
            var cellSubBlockId = new byte[4];
            Buffer.BlockCopy(BitConverter.GetBytes(cellSubBlockY), 0, cellSubBlockId, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(cellSubBlockX), 0, cellSubBlockId, 2, 2);
            if (_ensureCELLsByLabel.Contains(cellSubBlockId))
                return this;
            Load();
            if (CELLsById == null)
                CELLsById = new Dictionary<Vector3Int, CELLRecord>();
            if (LANDsById == null && cellId.z >= 0)
                LANDsById = new Dictionary<Vector3Int, LANDRecord>();
            if (GroupsByLabel.TryGetValue(cellSubBlockId, out var cellSubBlocks))
            {
                // find cell
                var cellSubBlock = cellSubBlocks.Single();
                cellSubBlock.Load(true);
                foreach (var cell in cellSubBlock.Records.Cast<CELLRecord>())
                {
                    cell.GridId = new Vector3Int(cell.XCLC.Value.GridX, cell.XCLC.Value.GridY, !cell.IsInterior ? cellId.z : -1);
                    CELLsById.Add(cell.GridId, cell);
                    // find children
                    if (cellSubBlock.GroupsByLabel.TryGetValue(BitConverter.GetBytes(cell.Id), out var cellChildren))
                    {
                        var cellChild = cellChildren.Single();
                        var cellTemporaryChildren = cellChild.Groups.Single(x => x.Headers.First().GroupType == Header.HeaderGroupType.CellTemporaryChildren);
                        foreach (var land in cellTemporaryChildren.Records.Cast<LANDRecord>())
                        {
                            land.GridId = new Vector3Int(cell.XCLC.Value.GridX, cell.XCLC.Value.GridY, !cell.IsInterior ? cellId.z : -1);
                            LANDsById.Add(land.GridId, land);
                        }
                    }
                }
                _ensureCELLsByLabel.Add(cellSubBlockId);
                return this;
            }
            return null;
        }
    }
}
