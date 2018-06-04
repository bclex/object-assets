//
//  EsmFile.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public class EsmFile {
    static let recordHeaderSizeInBytes = 16
    // public override string ToString() => $"{Path.GetFileName(FilePath)}";
    var _r: BinaryReader!
    public let filePath: String
    public let format: GameFormatId
    public var groups: [String : RecordGroup]
    // public var recordsByType: [Type : [IRecord]]
    // public var objectsByIDString: [String : IRecord]
    // public var exteriorCELLRecordsByIndices: [Vector2Int : CELLRecord]
    // public var LANDRecordsByIndices: [Vector2Int : LANDRecord]

    init(filePath: String?, for game: GameId) {
        func getFormatId() -> GameFormatId {
            switch game {
            // tes
            case .morrowind: return .TES3
            case .oblivion: return .TES4
            case .skyrim, .skyrimSE, .skyrimVR: return .TES5
            // fallout
            case .fallout3, .falloutNV: return .TES4
            case .fallout4, .fallout4VR: return .TES5
            default: fatalError("Error")
            }
        }
        guard let filePath = filePath else {
            return
        }
        self.filePath = filePath
        self.format = getFormatId()
        _r = BinaryReader(FileBaseStream(forReadingAtPath: filePath)!)
        let start = Date()
        read(0)
        let endRead = Date()
        debugPrint("Loading: \(endRead.timeIntervalSince(start))")
        postProcessRecords()
    }

    deinit {
        close()
    }

    public func close() {
        _r?.close()
        _r = nil
    }

    // public List<IRecord> GetRecordsOfType<T>() where T : Record { return recordsByType.TryGetValue(typeof(T), out List<IRecord> records) ? records : null; }

    func read(_ level: UInt8) {
        let rootHeader = Header(_r, format)
        guard (format != .TES3 || rootHeader.Type == "TES3") && (format == .TES3 || rootHeader.type == "TES4") else {
            fatalError("\(filePath) record header \(rootHeader.type) is not valid for this \(format)")
        }
        let rootRecord = rootHeader.createRecord(rootHeader.position)
        rootRecord.read(_r, filePath, format)
        // morrowind hack
        guard format != .TES3 else {
            let group = RecordGroup(_r, filePath, format, level)
            group.addHeader(Header(
                label: "",
                dataSize: (_r.baseStream.length - _r.baseStream.position),
                position: _r.baseStream.position
            ), 99);
            group.load()
            // groups = Dictionary(grouping: group.records, by: { $0.header.type! })
            //     .mapValues {
            //         var s = RecordGroup(_r, filePath, format, level, records: $0 }
            //         s.addHeader(Header(label: $0.first!header.type! })
            //         return s
            //      }
            return
        }
        // read groups
        groups = [String : RecordGroup]()
        let endPosition = _r.baseStream.length
        while _r.baseStream.position < endPosition {
            let header = Header(_r, format)
            guard header.type == "GRUP" else {
                fatalError("\(header.type) not GRUP")
            }
            let nextPosition = _r.baseStream.position + header.dataSize
            if let group = groups[header.label] {
                group = RecordGroup(_r, filePath, format, level)
                groups[header.label] = group
            }
            group.addHeader(header); debugPrint("Read: \(group)")
            _r.baseStream.position = nextPosition
        }
    }

    func postProcessRecords() {
        // recordsByType = new Dictionary<Type, List<IRecord>>();
        // objectsByIDString = new Dictionary<string, IRecord>();
        // exteriorCELLRecordsByIndices = new Dictionary<Vector2i, CELLRecord>();
        // LANDRecordsByIndices = new Dictionary<Vector2i, LANDRecord>();
        // foreach (var record in Groups.Values.SelectMany(x => x.Records))
        // {
        //     if (record == null)
        //         continue;
        //     // Add the record to the list for it's type.
        //     var recordType = record.GetType();
        //     if (recordsByType.TryGetValue(recordType, out List<IRecord> recordsOfSameType))
        //         recordsOfSameType.Add(record);
        //     else
        //     {
        //         recordsOfSameType = new List<IRecord> { record };
        //         recordsByType.Add(recordType, recordsOfSameType);
        //     }
        //     // Add the record to the object dictionary if applicable.
        //     if (record is IHaveEDID edid) objectsByIDString.Add(edid.EDID.Value, record);
        //     // Add the record to exteriorCELLRecordsByIndices if applicable.
        //     if (record is CELLRecord cell)
        //         if (!cell.IsInterior)
        //             exteriorCELLRecordsByIndices[cell.GridCoords] = cell;
        //     // Add the record to LANDRecordsByIndices if applicable.
        //     if (record is LANDRecord land)
        //         LANDRecordsByIndices[land.GridCoords] = land;
        // }
    }
}
