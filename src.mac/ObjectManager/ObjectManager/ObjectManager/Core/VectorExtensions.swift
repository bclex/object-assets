//
//  VectorExtensions.swift
//  ObjectManager
//
//  Created by David on 18-06-16.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation
import SceneKit

public struct Vector2Int {
    public let x: Int
    public let y: Int
    
    init(_ x: Int, _ y: Int) {
        self.x = x
        self.y = y
    }
}

public struct Vector3Int: Hashable {
    public let x: Int
    public let y: Int
    public let z: Int
    
    init(_ x: Int, _ y: Int, _ z: Int) {
        self.x = x
        self.y = y
        self.z = z
    }
}

extension Vector3 {
    static func + (vector: Vector3, scalar: Float) -> Vector3 {
        return Vector3(vector.x + CGFloat(scalar), vector.y + CGFloat(scalar), vector.z + CGFloat(scalar))
    }
    
    static func - (vector: Vector3, scalar: Float) -> Vector3 {
        return Vector3(vector.x - CGFloat(scalar), vector.y - CGFloat(scalar), vector.z - CGFloat(scalar))
    }
    
    static func * (vector: Vector3, scalar: Float) -> Vector3 {
        return Vector3(vector.x * CGFloat(scalar), vector.y * CGFloat(scalar), vector.z * CGFloat(scalar))
    }
    
    static func / (vector: Vector3, scalar: Float) -> Vector3 {
        return Vector3(vector.x / CGFloat(scalar), vector.y / CGFloat(scalar), vector.z / CGFloat(scalar))
    }
}
