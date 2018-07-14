//
//  GameObject.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import SceneKit

public class GameObject: SCNNode {
    public static func find(withTag: String) -> GameObject? {
        return nil
    }
    
    public static func destroy(_ obj: GameObject) {
    }
    
    public var activeSelf: Bool { return false }
    public func setActive(_ active: Bool) {
    }
    
    convenience init(name: String, tag: String? = nil) {
        self.init()
    }
}
