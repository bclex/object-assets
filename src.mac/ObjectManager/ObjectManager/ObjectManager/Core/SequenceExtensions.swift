//
//  SequenceExtensions.swift
//  ObjectManager
//
//  Created by Sky Morey on 6/5/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public extension Sequence where Element: FloatingPoint {
    public func getExtrema() -> (min: Element, max: Element) {
        var extrema = (min: Element.infinity, max: -Element.infinity)
        for element in self {
            extrema.min = Swift.min(extrema.min, element)
            extrema.max = Swift.max(extrema.max, element)
        }
        return extrema
    }
}

public extension Sequence where Iterator.Element == [Float] {
    public func getExtrema() -> (min: Float, max: Float) {
        var extrema = (min: Float.infinity, max: -Float.infinity)
        for (_, element) in self.enumerated() {
            for element2 in element {
                extrema.min = Swift.min(extrema.min, element2)
                extrema.max = Swift.max(extrema.max, element2)
            }
        }
        return extrema
    }
}

public class SequenceUtils {
    public static func flip2DSubArrayVertically(_ data: inout Data, offsetBy: Int, rows: Int, bytesPerRow: Int) {
        assert(offsetBy >= 0 && rows >= 0 && bytesPerRow >= 0 && (offsetBy + (rows * bytesPerRow)) <= data.count)
        var tmpRow = [UInt8](); tmpRow.reserveCapacity(bytesPerRow)
        let lastRowIndex = rows - 1
        for rowIndex in 0..<(rows / 2) {
            let otherRowIndex = lastRowIndex - rowIndex
            let rowStartIndex = data.index(data.startIndex, offsetBy: offsetBy + (rowIndex * bytesPerRow))
            let rowRange = rowStartIndex..<data.index(rowStartIndex, offsetBy: bytesPerRow)
            let otherRowStartIndex = data.index(data.startIndex, offsetBy: offsetBy + (otherRowIndex * bytesPerRow))
            let otherRowRange = otherRowStartIndex..<data.index(otherRowStartIndex, offsetBy: bytesPerRow)
            //
            tmpRow.replaceSubrange(tmpRow.startIndex..<tmpRow.endIndex, with: data[otherRowRange]) // other -> tmp
            data.replaceSubrange(otherRowRange, with: data[rowRange]) // row -> other
            data.replaceSubrange(rowRange, with: tmpRow) // tmp -> row
        }
    }
}

