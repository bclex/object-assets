//
//  VectorExtensions.swift
//  ObjectManager
//
//  Created by David on 18-06-16.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation
import SceneKit
import simd

// MARK Vector2

public typealias Vector2Int = (x: Int, y: Int)

// MARK Vector3

public struct Vector3Int: Hashable, CustomDebugStringConvertible, CustomStringConvertible {
    public let x: Int
    public let y: Int
    public let z: Int
    public static let zero = Vector3Int(0, 0, 0)
    
    init(_ x: Int, _ y: Int, _ z: Int) {
        self.x = x
        self.y = y
        self.z = z
    }
    
    public var description: String { return "[X:\(x) Y:\(y) Z:\(z)]" }
    public var debugDescription: String { return description }
}

public typealias Vector3Float = (x: Float, y: Float, z: Float)
public typealias Vector3Int8 = (x: UInt8, y: UInt8, z: UInt8)

public typealias Vector3 = SCNVector3

public extension float3 {
    static var zero: float3 { return float3(0, 0, 0) }
    static var up: float3 { return float3(0, 1, 0) }
    static var right: float3 { return float3(1, 0, 0) }
    static var left: float3 { return float3(-1, 0, 0) }
    static var down: float3 { return float3(0, -1, 0) }
    static var forward: float3 { return float3(0, 0, 1) }
    static var back: float3 { return float3(0, 0, -1) }
    static var one: float3 { return float3(1, 1, 1) }
}

public extension simd_quatf {
    static var identity: float4 { return float4(0, 0, 0, 1) }
}

public extension SCNVector3 {
    static var up: SCNVector3 { return Vector3(x: 0, y: 1, z: 0) }
    static var right: SCNVector3 { return Vector3(x: 1, y: 0, z: 0) }
    static var left: SCNVector3 { return Vector3(x: -1, y: 0, z: 0) }
    static var down: SCNVector3 { return Vector3(x: 0, y: -1, z: 0) }
    static var forward: SCNVector3 { return Vector3(x: 0, y: 0, z: 1) }
    static var back: SCNVector3 { return Vector3(x: 0, y: 0, z: -1) }
    
    internal init(from: Float3, to: Float3) {
        self.init()
        self.x = CGFloat(to.x - from.x)
        self.y = CGFloat(to.y - from.y)
        self.z = CGFloat(to.z - from.z)
    }
    
    init(from point: Float3) {
        self.init()
        self.x = CGFloat(point.x)
        self.y = CGFloat(point.y)
        self.z = CGFloat(point.z)
    }
    
    internal func toFloat3() -> Float3 {
        return Float3(x: GLfloat(self.x), y: GLfloat(self.y), z: GLfloat(self.z))
    }
    
    func cross(_ vector: SCNVector3) -> SCNVector3 {
        return SCNVector3Make(y * vector.z - z * vector.y, z * vector.x - x * vector.z, x * vector.y - y * vector.x)
    }
    
    var length: Double {
        let sqrX = self.x * self.x
        let sqrY = self.y * self.y
        let sqrZ = self.z * self.z
        return sqrt(Double(sqrX + sqrY + sqrZ))
    }
    
    var point: NSPoint {
        return NSPoint(x: x, y: y)
    }
    
    var normalized: SCNVector3 {
        let len = CGFloat(length)
        if len != 0 {
            return Vector3(x: self.x/len, y: self.y/len, z: self.z/len)
        }
        return self
    }
    
    var magnitude: CGFloat {
        return sqrt(Vector3.dot(self, b: self))
    }
    
    func copy() -> SCNVector3 {
        return SCNVector3(x: x, y: y, z: z)
    }
    
    static func angle(_ from: SCNVector3, to: SCNVector3) -> CGFloat {
        let clamped = clamp(dot(from.normalized, b: to.normalized), min: -1, max: 1)
        return acos(clamped) * CGFloat(180.0 / .pi)
    }
    
    static func dot(_ a: SCNVector3, b: SCNVector3) -> CGFloat {
        return (a.x*b.x) + (a.y*b.y) + (a.z*b.z)
    }
    
    static func moveTowards( _ current: SCNVector3, target: SCNVector3, maxDistanceDelta: CGFloat) -> SCNVector3 {
        let a = target - current
        let magnitude = a.magnitude
        if magnitude <= maxDistanceDelta || magnitude == 0 {
            return target
        }
        return current + (a / magnitude * maxDistanceDelta)
    }
}

extension SCNVector3: CustomDebugStringConvertible {
    public var debugDescription: String {
        return "[x:\(x) y:\(y) z:\(z)]"
    }
}

public func + (left: Vector3, right: Vector3) -> Vector3 {
    return Vector3(x: left.x + right.x, y: left.y + right.y, z: left.z + right.z)
}

public func + (a: Vector3, d: CGFloat) -> Vector3 {
    return Vector3(x: a.x + d, y: a.y + d, z: a.z + d)
}

public func - (left: Vector3, right: Vector3) -> Vector3 {
    return Vector3(x: left.x-right.x, y: left.y-right.y, z: left.z-right.z)
}

public func - (a: Vector3, d: CGFloat) -> Vector3 {
    return Vector3(x: a.x - d, y: a.y - d, z: a.z - d)
}

public func * (a: Vector3, d: CGFloat) -> Vector3 {
    return Vector3(x: a.x * d, y: a.y * d, z: a.z * d)
}

public func / (a: Vector3, d: CGFloat) -> Vector3 {
    return Vector3(x: a.x / d, y: a.y / d, z: a.z / d)
}

// MARK Vector4

public typealias Vector4 = SCNVector4

public func * (rotation: Vector4, point: Vector3) -> Vector3 {
    let num = rotation.x * 2
    let num2 = rotation.y * 2
    let num3 = rotation.z * 2
    let num4 = rotation.x * num
    let num5 = rotation.y * num2
    let num6 = rotation.z * num3
    let num7 = rotation.x * num2
    let num8 = rotation.x * num3
    let num9 = rotation.y * num3
    let num10 = rotation.w * num
    let num11 = rotation.w * num2
    let num12 = rotation.w * num3
    let result = Vector3(x:(1 - (num5 + num6)) * point.x + (num7 - num12) * point.y + (num8 + num11) * point.z,
                         y:(num7 + num12) * point.x + (1 - (num4 + num6)) * point.y + (num9 - num10) * point.z,
                         z:(num8 - num11) * point.x + (num9 + num10) * point.y + (1 - (num4 + num5)) * point.z)
    return result
}

// MARK Quadtrination
//
//public extension Quaternion {
//    public static func angleAxis(_ f: Float, _ v: Vector3) -> Vector3 {
//        return Vector3()
//    }
//}
