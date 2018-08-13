//
//  CameraController.swift
//  TestApp
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Cocoa
import SceneKit
import ObjectManager

class CameraController: NSObject {
    class func createController(_ scene: SCNScene) -> CameraController {
        return CameraController(scene: scene)
    }

    let camera: SCNCamera
    let cameraNode: GameObject

    var lookNode: SCNNode = SCNNode()
    var lookPoint: float3 {
        get { return lookNode.simdPosition }
        set { lookNode.simdPosition = newValue }
    }

    var cameraPosition: float3 {
        return cameraNode.simdPosition
    }

    init(scene: SCNScene) {
        camera = SCNCamera()
        camera.automaticallyAdjustsZRange = true
        cameraNode = GameObject()
        cameraNode.simdPosition = float3(x: 0, y: 5, z: 5) //float3(x: 0, y: 50, z: 75)
        cameraNode.camera = camera
        cameraNode.constraints = [SCNLookAtConstraint(target: lookNode)]
        scene.rootNode.addChildNode(cameraNode)
        super.init()
        lookPoint = float3(x: 0, y: 0, z: 0)
    }

    let minAngle: Float = 10
    let maxAngle: Float = 85
    let minZoom: Float = 10
    let maxZoom: Float = 35000

    var angleFromFloorToCamera: Float {
        let cam = cameraNode.back
        let camProjectionOntoFloor = float3(x: cam.x, y: cam.y, z: 0)
        return float3.angle(camProjectionOntoFloor, to: cam)
    }

    func moveCamera(_ delta: float3) {
        cameraNode.simdPosition = cameraNode.simdPosition + delta
    }

    func distanceToPoint(_ point: float3) -> Float {
        return (cameraNode.simdPosition - point).magnitude
    }

    func distanceToLookPoint() -> Float {
        return distanceToPoint(lookPoint)
    }

    func repositionCamera(_ newPosition: float3) {
        moveCamera(newPosition - cameraNode.simdPosition)
    }
}
