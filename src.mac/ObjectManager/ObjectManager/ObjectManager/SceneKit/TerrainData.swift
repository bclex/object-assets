//
//  TerrainData.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import AppKit
import CoreData
import simd

public class TerrainData {
    internal let map: Map = Map()
    public let heightmapResolution: Int
    public var size: float3!
    public var heights: [Float]!
    public var splatPrototypes: [SplatPrototype]!
    public var alphamaps: [[[Float]]]?
    public var alphamapResolution: Int?
    var width: Int
    var height: Int
    
    public init(heightmapResolution: Int) {
        self.heightmapResolution = heightmapResolution
        width = 64
        height = 64
        heights = Array<Float>(repeating: 0, count: width*height)
    }
    
    public func setHeights(_ x: Int, _ y: Int, _ z: [Float]) {
        print("HERE")
        width = 64
        height = 64
        heights = z
        saveToMap(map, scale: 15)
    
    }
    func sample(_ x: Int, y: Int) -> Float {
        return heights[(y & (height - 1)) * width + (x & (width - 1))]
    }
    
    func setSample(_ x: Int, y: Int, value: Float) {
        heights[(y & (height - 1)) * width + (x & (width - 1))] = value
    }

//    func adjustHeight(_ h1: Float, h2: Float, x: Int, y: Int) -> Bool {
//        if abs(h1 - h2) > 50 {
//            setSample(x, y: y, value:  h1 + ((h1 < h2) ? 50 : -50))
//            return true
//        }
//        return false
//    }
    
    public func setAlphamaps(_ x: Int, _ y: Int, _ z: [[[Float]]]) {
        alphamaps = z
    }
    
//    func generateFromImage(_ image: NSBitmapImageRep, map: Map) {
//        for y in 0..<height {
//            for x in 0..<width {
//                if let color = image.colorAt(x: x, y: y) {
//                    if let convertedColor = color.usingColorSpace(NSColorSpace.genericGray) {
//                        let white = convertedColor.whiteComponent
//                        let val = Float(round(white*256) - 127)
//                        setSample(x, y: y, value: val)
//                    }
//                }
//            }
//        }
//        saveToMap(map)
//    }
    
    func saveToMap(_ map: Map, scale: Float = 1) {
        map.width = self.width
        map.height = self.height
        var sortedTiles = [MapNode]()
        for x in 0..<heights.count {
            let val = heights[x]
            let newTile = MapNode()
            newTile.map = map
            newTile.index = x
            newTile.height = Int(val * scale)
            sortedTiles.append(newTile)
        }
        
        for index in 0..<sortedTiles.count {
            let current = sortedTiles[index]
            let leftInds = map.moveIndex(index, dir: .left)
            var other = sortedTiles[map.indexOf(leftInds.x, leftInds.y) ]
            current.left = other
            other.right = current
            
            let upLeft = map.moveIndex(index, dir: .upLeft)
            other = sortedTiles[map.indexOf(upLeft.x, upLeft.y) ]
            current.upLeft = other
            other.downRight = current
            
            let upRight = map.moveIndex(index, dir: .upRight)
            let translated = map.indexOf(upRight.x, upRight.y)
            let upRightNode = sortedTiles[translated]
            current.upRight = upRightNode
            upRightNode.downLeft = current
        }
        map.nodes = sortedTiles
    }
}

extension TerrainData {
    public func runAlgorithm() {
        var samplesize = 32
        print("Generating with feature size:\(samplesize)")
        var scale: Float = 2
        while samplesize > 1 {
//            print("Running Diamond Square Sample Size:\(samplesize)")
            diamondSquare(samplesize, scale: scale)
            samplesize /= 2
            scale /= 2.0
        }
        saveToMap(map, scale: 15)
    }
    
    func sampleSquare(_ x: Int, y: Int, size: Int, value: Float) {
        let hs = size / 2
        let a = sample(x - hs, y: y - hs)
        let b = sample(x + hs, y: y - hs)
        let c = sample(x - hs, y: y + hs)
        let d = sample(x + hs, y: y + hs)
        setSample(x, y: y, value: ((a + b + c + d) / 4.0) + value)
    }
    
    func sampleDiamond( _ x: Int, y: Int, size: Int, value: Float) {
        let hs = size / 2
        let a = sample(x - hs, y: y)
        let b = sample(x + hs, y: y)
        let c = sample(x, y: y - hs)
        let d = sample(x, y: y + hs)
        setSample(x, y: y, value: ((a + b + c + d) / 4.0) + value)
    }
    
    func frand() -> Float {
        return Float(arc4random_uniform(3))-1
    }
    
    func diamondSquare(_ stepsize: Int, scale: Float) {
        let halfstep = stepsize / 2
        for y in stride(from: halfstep, to: height + halfstep, by: stepsize) {
            for x in stride(from: halfstep, to: width + halfstep, by: stepsize) {
                let val = frand()
                sampleSquare(x, y: y, size: stepsize, value: val * scale)
            }
        }
        for y in stride(from: 0, to: height, by: stepsize) {
            for x in stride(from: 0, to: width, by: stepsize) {
                sampleDiamond(x + halfstep, y: y, size: stepsize, value:frand() * scale)
                sampleDiamond(x, y: y + halfstep, size: stepsize, value:frand() * scale)
            }
        }
    }
}
