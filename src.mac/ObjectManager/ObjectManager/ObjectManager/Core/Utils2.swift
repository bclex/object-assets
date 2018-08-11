//
//  Utils.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation
import SceneKit
import simd

public extension SCNNode {
    var forward: float3 {
        return self.simdRotation * float3.forward
    }
    var right: float3 {
        return self.simdRotation * float3.right
    }
    var left: float3 {
        return self.simdRotation * float3.left
    }
    var back: float3 {
        return self.simdRotation * float3.back
    }
    var up: float3 {
        return self.simdRotation * float3.up
    }
    var down: float3 {
        return self.simdRotation * float3.down
    }
}

func degToRad(_ deg: Float) -> Float {
    return (deg / 180.0) * Float(Double.pi)
}

let rad2Deg = Float(180.0 / .pi)
let deg2Rad = Float(.pi / 180.0)

func clamp<T: Comparable>(_ val: T, min: T, max: T) -> T {
    return val < min ? min : (val > max ? max:val)
}

struct Vertex {
    var position: float3
    var normal: float3
//    var tcoord: float2
    var color: float3

    mutating func setNormal(_ newNorm: float3) {
        normal = newNorm
    }
    mutating func setColor(_ newColor: NSColor) {
        var r: CGFloat = 1
        var g: CGFloat = 1
        var b: CGFloat = 1
        var a: CGFloat = 1

        newColor.getRed(&r, green: &g, blue: &b, alpha: &a)
        color.x = GLfloat(r)
        color.y = GLfloat(g)
        color.z = GLfloat(b)
    }
}
//
//var vertices: [Vertex] = [ /* ... vertex data ... */ ]
func createStripGeometry(_ vertices: [Vertex], triangles: [CInt]) -> SCNGeometry {

    let data = Data(bytes: UnsafeRawPointer(vertices), count: vertices.count * MemoryLayout<Vertex>.size)

    let vertexSource = SCNGeometrySource(data: data,
        semantic: SCNGeometrySource.Semantic.vertex,
        vectorCount: vertices.count,
        usesFloatComponents: true,
        componentsPerVector: 3,
        bytesPerComponent: MemoryLayout<Float>.size,
        dataOffset: 0, // position is first member in Vertex
        dataStride: MemoryLayout<Vertex>.size)

    let normalSource = SCNGeometrySource(data: data,
        semantic: SCNGeometrySource.Semantic.normal,
        vectorCount: vertices.count,
        usesFloatComponents: true,
        componentsPerVector: 3,
        bytesPerComponent: MemoryLayout<Float>.size,
        dataOffset: MemoryLayout<float3>.size, // one Float3 before normal in Vertex
        dataStride: MemoryLayout<Vertex>.size)

//    let tcoordSource = SCNGeometrySource(data: data,
//        semantic: SCNGeometrySourceSemanticTexcoord,
//        vectorCount: vertices.count,
//        floatComponents: true,
//        componentsPerVector: 2,
//        bytesPerComponent: sizeof(GLfloat),
//        dataOffset: 2 * sizeof(Float3), // 2 Float3s before tcoord in Vertex
//        dataStride: sizeof(Vertex))
    let colorSource = SCNGeometrySource(data: data,
        semantic: SCNGeometrySource.Semantic.color,
        vectorCount: vertices.count,
        usesFloatComponents: true,
        componentsPerVector: 3,
        bytesPerComponent: MemoryLayout<Float>.size,
        dataOffset: (2 * MemoryLayout<float3>.size), // 2 Float3s before tcoord in Vertex
        dataStride: MemoryLayout<Vertex>.size)

    let triData = Data(bytes: UnsafeRawPointer(triangles), count: MemoryLayout<CInt>.size*triangles.count)

    let geometryElement = SCNGeometryElement(data: triData, primitiveType: .triangleStrip, primitiveCount: vertices.count*2, bytesPerIndex: MemoryLayout<CInt>.size)

    return SCNGeometry(sources: [vertexSource, normalSource, /*tcoordSource,*/colorSource], elements: [geometryElement])
}

//var vertices: [Vertex] = [ /* ... vertex data ... */ ]
func createTriangleGeometry(_ vertices: [Vertex], triangles: [CInt]) -> SCNGeometry {
    let data = Data(bytes: UnsafeRawPointer(vertices), count: vertices.count * MemoryLayout<Vertex>.size)

    let vertexSource = SCNGeometrySource(data: data,
        semantic: SCNGeometrySource.Semantic.vertex,
        vectorCount: vertices.count,
        usesFloatComponents: true,
        componentsPerVector: 3,
        bytesPerComponent: MemoryLayout<Float>.size,
        dataOffset: 0, // position is first member in Vertex
        dataStride: MemoryLayout<Vertex>.size)

    let normalSource = SCNGeometrySource(data: data,
        semantic: SCNGeometrySource.Semantic.normal,
        vectorCount: vertices.count,
        usesFloatComponents: true,
        componentsPerVector: 3,
        bytesPerComponent: MemoryLayout<Float>.size,
        dataOffset: MemoryLayout<float3>.size, // one Float3 before normal in Vertex
        dataStride: MemoryLayout<Vertex>.size)

    //    let tcoordSource = SCNGeometrySource(data: data,
    //        semantic: SCNGeometrySourceSemanticTexcoord,
    //        vectorCount: vertices.count,
    //        floatComponents: true,
    //        componentsPerVector: 2,
    //        bytesPerComponent: sizeof(Float),
    //        dataOffset: 2 * sizeof(float3), // 2 Float3s before tcoord in Vertex
    //        dataStride: sizeof(Vertex))
    let colorSource = SCNGeometrySource(data: data,
        semantic: SCNGeometrySource.Semantic.color,
        vectorCount: vertices.count,
        usesFloatComponents: true,
        componentsPerVector: 3,
        bytesPerComponent: MemoryLayout<Float>.size,
        dataOffset: (2 * MemoryLayout<float3>.size), // 2 Float3s before tcoord in Vertex
        dataStride: MemoryLayout<Vertex>.size)

    let triData = Data(bytes: UnsafeRawPointer(triangles), count: MemoryLayout<CInt>.size*triangles.count)

    let geometryElement = SCNGeometryElement(data: triData, primitiveType: .triangles, primitiveCount: 1, bytesPerIndex: MemoryLayout<CInt>.size)

    return SCNGeometry(sources: [vertexSource, normalSource, /*tcoordSource,*/colorSource], elements: [geometryElement])
}

public func calculateVectorNormal(_ A: float3, B: float3, C: float3) -> float3 {
    let AB = A - B
    let CB = C - B
    var cross = simd_cross(AB, CB)
    cross = simd_normalize(cross)
    return cross
}
