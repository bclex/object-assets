//
//  GameSegment.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public protocol GameSegment {
    func start(player: GameObject)
    func onDestroy()
    func update()
}
