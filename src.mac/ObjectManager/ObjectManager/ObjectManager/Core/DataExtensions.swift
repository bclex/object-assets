//
//  DataExtensions.swift
//  CocoApp
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation
import Compression

public extension Data {
    public func inflate(size: Int, withOffset: Int = 0) -> Data? {
        return self.withUnsafeBytes { (sourcePtr: UnsafePointer<UInt8>) -> Data? in
            return perform(operation: COMPRESSION_STREAM_DECODE, algorithm: COMPRESSION_ZLIB, source: sourcePtr + withOffset, sourceSize: count - withOffset, destSize: size)
        }
    }
    
    public func lzmaDecompress(size: Int, withOffset: Int = 0) -> Data? {
        return self.withUnsafeBytes { (sourcePtr: UnsafePointer<UInt8>) -> Data? in
            return perform(operation: COMPRESSION_STREAM_DECODE, algorithm: COMPRESSION_LZMA, source: sourcePtr + withOffset, sourceSize: count - withOffset, destSize: size)
        }
    }
}

fileprivate func perform(operation: compression_stream_operation, algorithm: compression_algorithm, source: UnsafePointer<UInt8>, sourceSize: Int, destSize: Int) -> Data? {
    guard operation == COMPRESSION_STREAM_ENCODE || sourceSize > 0 else { return nil }
    
    let streamBase = UnsafeMutablePointer<compression_stream>.allocate(capacity: 1)
    defer { streamBase.deallocate() }
    var stream = streamBase.pointee
    
    let status = compression_stream_init(&stream, operation, algorithm)
    guard status != COMPRESSION_STATUS_ERROR else { return nil }
    defer { compression_stream_destroy(&stream) }
    
    let bufferSize = Swift.max(Swift.min(destSize, 64 * 1024), 64)
    let buffer = UnsafeMutablePointer<UInt8>.allocate(capacity: bufferSize)
    defer { buffer.deallocate() }
    
    stream.dst_ptr  = buffer
    stream.dst_size = bufferSize
    stream.src_ptr  = source
    stream.src_size = sourceSize
    
    var r = Data()
    let flags = Int32(operation == COMPRESSION_STREAM_DECODE ? 0 : COMPRESSION_STREAM_FINALIZE.rawValue)
    
    while true {
        switch compression_stream_process(&stream, flags) {
        case COMPRESSION_STATUS_OK:
            //guard stream.dst_size == 0 else { return nil }
            r.append(buffer, count: stream.dst_ptr - buffer)
            stream.dst_ptr = buffer
            stream.dst_size = bufferSize
        case COMPRESSION_STATUS_ERROR:
            return nil
        case COMPRESSION_STATUS_END:
            r.append(buffer, count: stream.dst_ptr - buffer)
            return r
        default:
            fatalError("Error with compression")
        }
    }
}
