using OA.Tes.FilePacks.Tes3;
using OA.Core;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using OA.Tes.Core;

// https://github.com/WrinklyNinja/esplugin/tree/master/src
// http://en.uesp.net/wiki/Tes4Mod:Mod_File_Format/Vs_Morrowind
namespace OA.Tes.FilePacks
{
    public enum GameId
    {
        Oblivion,
        Skyrim,
        Fallout3,
        FalloutNV,
        Morrowind,
        Fallout4,
        SkyrimSE,
    }

    public static class RecordUtils
    {
        public static string GetModelFileName(Record record)
        {
            if (record is STATRecord) return ((STATRecord)record).MODL.value;
            else if (record is DOORRecord) return ((DOORRecord)record).MODL.value;
            else if (record is MISCRecord) return ((MISCRecord)record).MODL.value;
            else if (record is WEAPRecord) return ((WEAPRecord)record).MODL.value;
            else if (record is CONTRecord) return ((CONTRecord)record).MODL.value;
            else if (record is LIGHRecord) return ((LIGHRecord)record).MODL.value;
            else if (record is ARMORecord) return ((ARMORecord)record).MODL.value;
            else if (record is CLOTRecord) return ((CLOTRecord)record).MODL.value;
            else if (record is REPARecord) return ((REPARecord)record).MODL.value;
            else if (record is ACTIRecord) return ((ACTIRecord)record).MODL.value;
            else if (record is APPARecord) return ((APPARecord)record).MODL.value;
            else if (record is LOCKRecord) return ((LOCKRecord)record).MODL.value;
            else if (record is PROBRecord) return ((PROBRecord)record).MODL.value;
            else if (record is INGRRecord) return ((INGRRecord)record).MODL.value;
            else if (record is BOOKRecord) return ((BOOKRecord)record).MODL.value;
            else if (record is ALCHRecord) return ((ALCHRecord)record).MODL.value;
            else if (record is CREARecord) return ((CREARecord)record).MODL.value;
            else if (record is NPC_Record) { var npc = (NPC_Record)record; return npc.MODL != null ? npc.MODL.value : null; }
            else return null;
        }
    }

    public class EsmFile : IDisposable
    {
        const int recordHeaderSizeInBytes = 16;
        public Record[] records;
        public Dictionary<Type, List<Record>> recordsByType;
        public Dictionary<string, Record> objectsByIDString;
        public Dictionary<Vector2i, CELLRecord> exteriorCELLRecordsByIndices;
        public Dictionary<Vector2i, LANDRecord> LANDRecordsByIndices;

        public EsmFile(string filePath, GameId gameId)
        {
            ReadRecords(filePath, gameId);
            PostProcessRecords();
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        ~EsmFile()
        {
            Close();
        }

        public void Close() { }

        public List<Record> GetRecordsOfType<T>() where T : Record
        {
            List<Record> records;
            if (recordsByType.TryGetValue(typeof(T), out records))
                return records;
            return null;
        }

        private void ReadRecords(string filePath, GameId gameId, bool loadHeaderOnly = false)
        {
            var r = new UnityBinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read));
            var header = new Header(r, gameId);
            if ((gameId == GameId.Morrowind && header.type != "TES3") || (gameId != GameId.Morrowind && header.type != "TES4"))
                throw new InvalidOperationException(filePath + " is not a valid plugin.");
            var record = header.CreateUninitializedRecord();
            record.DeserializeData(r, gameId);
            if (loadHeaderOnly)
                return;
            var recordList = new List<Record>();
            while (r.BaseStream.Position < r.BaseStream.Length)
            {
                var recordStartStreamPosition = r.BaseStream.Position;
                header = new Header(r, gameId);
                record = header.CreateUninitializedRecord();
                // Read or skip the record.
                if (record != null)
                {
                    var recordDataStreamPosition = r.BaseStream.Position;
                    record.DeserializeData(r, gameId);
                    if (r.BaseStream.Position != (recordDataStreamPosition + record.header.dataSize))
                        throw new FormatException("Failed reading " + header.type + " record at offset " + recordStartStreamPosition + " in " + filePath);
                    recordList.Add(record);
                }
                else
                {
                    // Skip the record.
                    r.BaseStream.Position += header.dataSize;
                    //recordList.Add(null);
                }
            }
            records = recordList.ToArray();
        }

        private void PostProcessRecords()
        {
            recordsByType = new Dictionary<Type, List<Record>>();
            objectsByIDString = new Dictionary<string, Record>();
            exteriorCELLRecordsByIndices = new Dictionary<Vector2i, CELLRecord>();
            LANDRecordsByIndices = new Dictionary<Vector2i, LANDRecord>();
            foreach (var record in records)
            {
                if (record == null)
                    continue;
                // Add the record to the list for it's type.
                var recordType = record.GetType();
                List<Record> recordsOfSameType;
                if (recordsByType.TryGetValue(recordType, out recordsOfSameType))
                    recordsOfSameType.Add(record);
                else
                {
                    recordsOfSameType = new List<Record>();
                    recordsOfSameType.Add(record);
                    recordsByType.Add(recordType, recordsOfSameType);
                }
                // Add the record to the object dictionary if applicable.
                if (record is GMSTRecord) objectsByIDString.Add(((GMSTRecord)record).NAME.value, record);
                else if (record is GLOBRecord) objectsByIDString.Add(((GLOBRecord)record).NAME.value, record);
                else if (record is SOUNRecord) objectsByIDString.Add(((SOUNRecord)record).NAME.value, record);
                else if (record is REGNRecord) objectsByIDString.Add(((REGNRecord)record).NAME.value, record);
                else if (record is LTEXRecord) objectsByIDString.Add(((LTEXRecord)record).NAME.value, record);
                else if (record is STATRecord) objectsByIDString.Add(((STATRecord)record).NAME.value, record);
                else if (record is DOORRecord) objectsByIDString.Add(((DOORRecord)record).NAME.value, record);
                else if (record is MISCRecord) objectsByIDString.Add(((MISCRecord)record).NAME.value, record);
                else if (record is WEAPRecord) objectsByIDString.Add(((WEAPRecord)record).NAME.value, record);
                else if (record is CONTRecord) objectsByIDString.Add(((CONTRecord)record).NAME.value, record);
                else if (record is LIGHRecord) objectsByIDString.Add(((LIGHRecord)record).NAME.value, record);
                else if (record is ARMORecord) objectsByIDString.Add(((ARMORecord)record).NAME.value, record);
                else if (record is CLOTRecord) objectsByIDString.Add(((CLOTRecord)record).NAME.value, record);
                else if (record is REPARecord) objectsByIDString.Add(((REPARecord)record).NAME.value, record);
                else if (record is ACTIRecord) objectsByIDString.Add(((ACTIRecord)record).NAME.value, record);
                else if (record is APPARecord) objectsByIDString.Add(((APPARecord)record).NAME.value, record);
                else if (record is LOCKRecord) objectsByIDString.Add(((LOCKRecord)record).NAME.value, record);
                else if (record is PROBRecord) objectsByIDString.Add(((PROBRecord)record).NAME.value, record);
                else if (record is INGRRecord) objectsByIDString.Add(((INGRRecord)record).NAME.value, record);
                else if (record is BOOKRecord) objectsByIDString.Add(((BOOKRecord)record).NAME.value, record);
                else if (record is ALCHRecord) objectsByIDString.Add(((ALCHRecord)record).NAME.value, record);
                else if (record is CREARecord) objectsByIDString.Add(((CREARecord)record).NAME.value, record);
                else if (record is NPC_Record) objectsByIDString.Add(((NPC_Record)record).NAME.value, record);
                // Add the record to exteriorCELLRecordsByIndices if applicable.
                if (record is CELLRecord)
                {
                    var cell = (CELLRecord)record;
                    if (!cell.isInterior)
                        exteriorCELLRecordsByIndices[cell.gridCoords] = cell;
                }
                // Add the record to LANDRecordsByIndices if applicable.
                if (record is LANDRecord)
                {
                    var land = (LANDRecord)record;
                    LANDRecordsByIndices[land.gridCoords] = land;
                }
            }
        }
    }
}