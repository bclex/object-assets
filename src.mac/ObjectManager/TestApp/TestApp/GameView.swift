//
//  GameView.swift
//  TestApp
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import ObjectManager
import SceneKit
import simd

class GameView: SCNView {
    var cameraNode: CameraController? = nil

    override func mouseDragged(with theEvent: NSEvent) {
        guard let controller = cameraNode else {
            return
        }
        let viewPoint = convert(theEvent.locationInWindow, from: nil)
        let curHit = unprojectPoint(SCNVector3(viewPoint.x, viewPoint.y, 1))
        let prevPoint = CGPoint(x: viewPoint.x - theEvent.deltaX, y: viewPoint.y + theEvent.deltaY)
        let prevHit = unprojectPoint(SCNVector3(prevPoint.x, prevPoint.y, 1))
        var delta = float3(prevHit - curHit)
        delta.y = 0
        controller.lookPoint = controller.lookPoint + delta
        controller.moveCamera(delta)
    }

    override func scrollWheel(with theEvent: NSEvent) {
        guard let camNode = cameraNode else {
            return
        }
        let distanceToLookAt = camNode.distanceToLookPoint()
        let scale = theEvent.deltaY
        if scale == 0 {
            return
        }
        var distanceToMove = Float(theEvent.deltaY)
        if (distanceToLookAt - distanceToMove) > camNode.maxZoom {
            distanceToMove = distanceToLookAt - camNode.maxZoom
        } else if (distanceToLookAt - distanceToMove) < camNode.minZoom {
            distanceToMove = distanceToLookAt - camNode.minZoom
        }
        camNode.repositionCamera(float3.moveTowards(camNode.cameraPosition, target: camNode.lookPoint, maxDistanceDelta: distanceToMove))
    }
}
