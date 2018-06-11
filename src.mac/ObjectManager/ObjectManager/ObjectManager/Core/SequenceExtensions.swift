//
//  SequenceExtensions.swift
//  ObjectManager
//
//  Created by Sky Morey on 6/5/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//
    
public extension Sequence where Element: FloatingPoint 
    public func getExtrema() -> (min: Element, max: Element) {
        var extrema = (min: Element.infinty, max: -Element.infinty)
        for element in self {
            extrema.min = Swift.min(min, element)
            extrema.max = Swift.max(max, element)
        }
        return extrema
    }
}

public extension Sequence where Element: Numeric 
    public static func flip2DSubArrayVertically(offsetBy: Int, rows: Int, bytesPerRow: Int) {
        assert(offsetBy >= 0 && rows >= 0 && bytesPerRow >= 0 && (offsetBy + (rows * bytesPerRow)) <= self.count)
        var tmpRow = [Element](); tmpRow.reserveCapacity(bytesPerRow)
        let lastRowIndex = rows - 1
        for rowIndex in 0..<(rows / 2) {
            let otherRowIndex = lastRowIndex - rowIndex
            let rowStartIndex = self.index(self.startIndex, offsetBy: offsetBy + (rowIndex * bytesPerRow))
            let rowRange = rowStartIndex..<self.index(rowStartIndex, offsetBy: bytesPerRow)
            let otherRowStartIndex = self.index(self.startIndex, offsetBy: offsetBy + (otherRowIndex * bytesPerRow))
            let otherRowRange = otherRowStartIndex..<self.index(otherRowStartIndex, offsetBy: bytesPerRow)
            //
            tmpRow = self[otherRowRange] // other -> tmp
            self[otherRowRange] = self[rowRange] // row -> other
            self[rowRange] = tmpRow // tmp -> row
        }
    }
}
