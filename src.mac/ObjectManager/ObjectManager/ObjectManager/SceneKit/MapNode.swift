//
//  MapNode.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation
import SceneKit
import simd

// swiftlint:disable variable_name
let TR_W: GLfloat = 53
let TR_H: GLfloat = 29
let HEIGHT_FACTOR: GLfloat = 5
// swiftlint:enable variable_name

var colorsMap = [String: [NSColor]]()

func colorsFromPallet(_ palletName: NSString) -> [NSColor] {
    if let colors = colorsMap[palletName as String] {
        return colors
    } else {
        var loadedColors = [NSColor()]
        if let image = NSImage(named:palletName as String) {
            if let bitmap = NSBitmapImageRep(data:image.tiffRepresentation!) {
                for x in 0..<Int(bitmap.size.width) {
                    if let color = bitmap.colorAt(x: x, y: 0) {
                        if let convertedColor = color.usingColorSpace(NSColorSpace.genericRGB) {
                            loadedColors.append(convertedColor)
                        }
                    }
                }
            }
        }
        colorsMap[palletName as String] = loadedColors
        return loadedColors
    }
}

@objc(MapNode)
public class MapNode: NSObject {
    public override var description: String { return "[index:\(index) height:\(height)]" }
    public override var debugDescription: String { return description }
    
    var height: Int = 0
    var index: Int = 0
    var map: Map? = nil

    var left: MapNode? = nil
    var upLeft: MapNode? = nil
    var upRight: MapNode? = nil
    var right: MapNode? = nil
    var downLeft: MapNode? = nil
    var downRight: MapNode?  = nil

    var calculatedPosition: float3? = nil
    var position: float3 {
        if let calculatedPosition = calculatedPosition {
            return calculatedPosition
        }

        let pos = map!.translateIndex(Int(self.index))
        calculatedPosition = float3(x: GLfloat(pos.x) * TR_W + (pos.y % 2 == 0 ? 0 : TR_W * 0.5), y: GLfloat( self.height) * HEIGHT_FACTOR, z: GLfloat(pos.y) * TR_H)

        return calculatedPosition!
    }

    var calculatedVert: Vertex? = nil
    var vertex: Vertex {
        if let calculatedVert = calculatedVert {
            return calculatedVert
        }

        let position = self.position
        let normal = self.normal
        _ = float2(0, 0)
        let colors = colorsFromPallet("terrainColors")

        var r: CGFloat = 1
        var g: CGFloat = 1
        var b: CGFloat = 1
        var a: CGFloat = 1

        let colorIndex: Int = max(min(Int(127+self.height), colors.count-1), 0)
        let color = colors[colorIndex]
        if let converted = color.usingColorSpace(NSColorSpace.genericRGB) {
            converted.getRed(&r, green: &g, blue: &b, alpha: &a)
        }

        calculatedVert = Vertex(position: position, normal: normal/*, tcoord: tCoord*/, color: float3(Float(r), Float(g), Float(b)))
        return calculatedVert!
    }
//
//    //             A_____ B____ X
//    //            /\    /\    /
//    //           /  \  /  \  /
//    //        C /____\/_D__\/ E
//    //          \    /\    /
//    //           \  /  \  /
//    //         Y  \/_F__\/ G
//
    var calculatedNorm: float3? = nil
    var normal: float3 {
        if let calculatedNorm = calculatedNorm {
            return calculatedNorm
        }

        // Calculates the average normal of all the triangles surrounding it.
        var triNorms = [float3]()

        var A = self.upRight!
        var C = self.right!

        var normal = calculateVectorNormal(A.position, B: self.position, C: C.position)

        if self.index < self.right!.index {
            triNorms.append(normal)

            A = self.right!
            C = self.downRight!
            normal = calculateVectorNormal(A.position, B: self.position, C: C.position)
            triNorms.append(normal)
        }

        if self.index < self.upLeft!.index {
            A = self.upLeft!
            C = self.upRight!
            normal = calculateVectorNormal(A.position, B: self.position, C: C.position)
            triNorms.append(normal)
        }

        if self.left!.index < self.index {
            A = self.left!
            C = self.upLeft!
            normal = calculateVectorNormal(A.position, B: self.position, C: C.position)
            triNorms.append(normal)

            A = self.downLeft!
            C = self.left!
            normal = calculateVectorNormal(A.position, B: self.position, C: C.position)
            triNorms.append(normal)
        }

        if self.index > self.map!.width {
            A = self.downRight!
            C = self.downLeft!
            normal = calculateVectorNormal(A.position, B: self.position, C: C.position)
            triNorms.append(normal)
        }

        var x: Float = 0.0
        var y: Float = 0.0
        var z: Float = 0.0

        for norm in triNorms {
            x = x + Float(norm.x)
            y = y + Float(norm.y)
            z = z + Float(norm.z)
        }

        x /= Float(triNorms.count)
        y /= Float(triNorms.count)
        z /= Float(triNorms.count)
        var sum = float3(x, y, z)
        sum = simd_normalize(sum)

        //             A_____ B____ X
        //            /\    /\    /
        //           /  \  /  \  /
        //        C /____\/_D__\/ E
        //          \    /\    /
        //           \  /  \  /
        //         Y  \/_F__\/ G

        calculatedNorm = sum
        return calculatedNorm!
    }

    var upTriangle: [Vertex] {
        let A = downRight!.vertex
        let B = vertex
        let C = downLeft!.vertex

        /* A.tcoord = Float2(s: 1, t: tileIndex)
        B.tcoord = Float2(s: 0, t: tileIndex + stepVal)
        C.tcoord = Float2(s: 0, t: tileIndex)*/
        return [A, B, C]
    }

    var downTriangle: [Vertex] {
        let A = right!.vertex
        let B = vertex
        let C = downRight!.vertex
        /*        A.tcoord = Float2(s: 1, t: tileIndex + stepVal)
        B.tcoord = Float2(s: 0, t: tileIndex + stepVal)
        C.tcoord = Float2(s: 1, t: tileIndex)*/
        return [A, B, C]
    }
}
