//
//  GameSegment.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import SceneKit

public protocol GameSegment {
    func start(rootNode: SCNNode, player: GameObject)
    func onDestroy()
    func update()
}
