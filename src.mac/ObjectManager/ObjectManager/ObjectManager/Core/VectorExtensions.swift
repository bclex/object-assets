//
//  VectorExtensions.swift
//  ObjectManager
//
//  Created by David on 18-06-16.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation
import SceneKit

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
