//
//  EsmFile.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation
import simd

public class EsmFile: CustomStringConvertible {
    static let recordHeaderSizeInBytes = 16
    public var description: String { return "\(filePath)" }
    var _r: BinaryReader!
    public let filePath: String
    public let format: GameFormatId
    public var groups: [String : RecordGroup]? = nil
    // TES3
    var _MANYsById: [String : Record]!
    var _LTEXsById: [Int64 : LTEXRecord]!
    var _LANDsById: [int3 : LANDRecord]!
    var _CELLsById: [int3 : CELLRecord]!
    var _CELLsByName: [String : CELLRecord]!
    // TES4
    var _WRLDsById: [UInt32 : (x: WRLDRecord, y: [RecordGroup])]!
    var _LTEXsByEid: [String : LTEXRecord]!

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
        read(recordLevel: 1)
        let endRead = Date()
        debugPrint("Loading: \(endRead.timeIntervalSince(start))")
        process()
    }

    deinit {
        close()
    }

    public func close() {
        _r?.close()
        _r = nil
    }

    func read(recordLevel: Int) {
        let rootHeader = Header(_r, for: format, parent: nil)
        guard (format != .TES3 || rootHeader.type == "TES3") && (format == .TES3 || rootHeader.type == "TES4") else {
            fatalError("\(filePath) record header \(rootHeader.type) is not valid for this \(format)")
        }
        let rootRecord = rootHeader.createRecord(at: rootHeader.position, recordLevel: recordLevel)!
        rootRecord.read(_r, filePath, for: format)
        // morrowind hack
        guard format != .TES3 else {
            let group = RecordGroup(_r, filePath, for: format, recordLevel: recordLevel)
            group.addHeader(Header(
                label: nil,
                dataSize: UInt32(_r.baseStream.length - _r.baseStream.position),
                position: _r.baseStream.position
            ));
            group.load()
            groups = Dictionary(uniqueKeysWithValues:
                Dictionary(grouping: group.records) { $0.header.type }.map { x -> (key: String, value: RecordGroup) in
                    (key: x.key, value: RecordGroup(_r, filePath, for: format, recordLevel: recordLevel, label: x.key, records: x.value)) })
            return
        }
        // read groups
        groups = [String : RecordGroup]()
        let endPosition = _r.baseStream.length
        while _r.baseStream.position < endPosition {
            let header = Header(_r, for: format, parent: nil)
            guard header.type == "GRUP" else {
                fatalError("\(header.type) not GRUP")
            }
            let nextPosition = _r.baseStream.position + UInt64(header.dataSize)
            let label = String(data: header.label!, encoding: .ascii)!
            var group = groups![label]
            if group == nil {
                group = RecordGroup(_r, filePath, for: format, recordLevel: recordLevel)
                groups![label] = group
            }
            group!.addHeader(header)
            _r.baseStream.position = nextPosition
        }
    }
}
