//
//  GameObject.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import SceneKit

public typealias SplatPrototype = (
    texture: Texture2D,
    smoothness: Int,
    metallic: Int,
    tileSize: int2)

public class GameObject: SCNNode {
    var tag: String? = nil
    
    public static func find(withTag: String) -> GameObject? {
        return nil
    }
    
    public static func destroy(_ obj: GameObject) {
    }
    
    public var activeSelf: Bool { return false }
    public func setActive(_ active: Bool) {
    }
    
    public convenience init(name: String, tag: String? = nil) {
        self.init()
        self.name = name
        self.tag = tag
    }
    
    public static func instantiate(_ obj: GameObject) -> GameObject {
        return obj.clone()
    }
}
