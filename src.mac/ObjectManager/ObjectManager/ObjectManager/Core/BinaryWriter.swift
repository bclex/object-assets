//
//  BinaryWriter.swift
//  CocoApp
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class BinaryWriter {
    public var baseStream: BaseStream!
    
    init(_ stream: BaseStream) {
        baseStream = stream
    }
    
    deinit {
        close()
    }
    
    func close() {
        baseStream?.close()
        baseStream = nil
    }

    func write(_ value: UInt8) {
    }

    func write(_ value: Int8) {
    }

    func writeBytes(_ value: [UInt8]) {
    }

    func write(_ value: String) {
    }

    func write(_ value: Bool) {
    }

    func write(_ value: UInt16) {
    }

    func write(_ value: UInt32) {
    }

    func write(_ value: UInt64) {
    }

    func write(_ value: Int16) {
    }

    func write(_ value: Int32) {
    }

    func write(_ value: Int64) {
    }

    func write(_ value: Float) {
    }

    func write(_ value: Double) {
    }
}
