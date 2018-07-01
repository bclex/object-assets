//
//  EsmFile.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public class EsmFile: CustomStringConvertible {
    static let recordHeaderSizeInBytes = 16
    public var description: String { return "\(filePath)" }
    var _r: BinaryReader!
    public let filePath: String
    public let format: GameFormatId
    public var groups: [String : RecordGroup]? = nil
    // TES3
    var _LTEXsById: [Int : LTEXRecord]
    var _CELLsById: [Vector2Int : CELLRecord]
    var _LANDsById: [Vector2Int : LANDRecord]

    init(_ filePath: URL?, for game: GameId) {
        func getFormatId() -> GameFormatId {
            switch game {
            // tes
            case .morrowind: return .TES3
            case .oblivion: return .TES4
            case .skyrim, .skyrimSE, .skyrimVR: return .TES5
            // fallout
            case .fallout3, .falloutNV: return .TES4
            case .fallout4, .fallout4VR: return .TES5
            }
        }
        self.format = getFormatId()
        guard let filePath = filePath else {
            self.filePath = "missing"
            return
        }
        self.filePath = filePath.path
        _r = BinaryReader(FileBaseStream(path: self.filePath)!)
        let start = Date()
        read(level: 1)
        let endRead = Date()
        debugPrint("Loading: \(endRead.timeIntervalSince(start))")
        //process()
    }

    deinit {
        close()
    }

    public func close() {
        _r?.close()
        _r = nil
    }

    func read(_ recordLevel: Int) {
        let rootHeader = Header(_r, for: format)
        guard (format != .TES3 || rootHeader.type == "TES3") && (format == .TES3 || rootHeader.type == "TES4") else {
            fatalError("\(filePath) record header \(rootHeader.type) is not valid for this \(format)")
        }
        let rootRecord = rootHeader.createRecord(at: rootHeader.position, recordLevel: recordLevel)!
        rootRecord.read(_r, filePath, for: format)
        // morrowind hack
        guard format != .TES3 else {
            let group = RecordGroup(_r, filePath, for: format, recordLevel: recordLevel)
            group.addHeader(Header(
                label: "",
                dataSize: UInt32(_r.baseStream.length - _r.baseStream.position),
                position: _r.baseStream.position
            ));
            group.load()
//            groups = Dictionary(grouping: group.records, by: { $0.header.type! })
//            .mapValues {
//                var s = RecordGroup(_r, filePath, format, level, records: $0 }
//                s.addHeader(Header(label: $0.first!header.type! })
//                return s
//            }
            return
        }
        // read groups
        groups = [String : RecordGroup]()
        let endPosition = _r.baseStream.length
        while _r.baseStream.position < endPosition {
            let header = Header(_r, for: format)
            guard header.type == "GRUP" else {
                fatalError("\(header.type) not GRUP")
            }
            let nextPosition = _r.baseStream.position + UInt64(header.dataSize)
            var group = groups![header.label!]
            if group == nil {
                group = RecordGroup(_r, filePath, for: format, level: level)
                groups![header.label!] = group
            }
            group!.addHeader(header); debugPrint("Read: \(group!)")
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
