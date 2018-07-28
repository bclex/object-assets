//
//  GameViewController.swift
//  TestApp
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import SceneKit
import QuartzCore
import ObjectManager

//typealias Test = TesEngineTest
typealias Test = TesPackTest

class GameViewController: NSViewController {
//    @IBOutlet weak var gameView: GameView!
    var game = Game()
    var segments = [Test()]

    override func viewDidLoad() {
        let world = SCNScene()
        
        // set the scene to the view
        guard let gameView = self.view as? GameView else {
            return
        }
        gameView.delegate = self
        gameView.scene = world
        gameView.cameraNode = CameraController.createController(world)
        gameView.allowsCameraControl = true
        gameView.autoenablesDefaultLighting = true
        gameView.allowsCameraControl = false
        
        // show statistics such as fps and timing information
        gameView.showsStatistics = true
        
        // configure the view
        gameView.backgroundColor = NSColor(calibratedRed: 0, green: 0, blue: 1, alpha: 0)
        
        game.createRandomMap()
        let tNode = TerrainNode(map: game.map)
        gameView.scene?.rootNode.addChildNode(tNode)
        
        let player = gameView.cameraNode!.cameraNode
        for segment in segments {
            segment.start(player: player)
        }
    }
    
    override func viewDidDisappear() {
        for segment in segments {
            segment.onDestroy()
        }
        segments.removeAll()
    }
}

extension GameViewController: SCNSceneRendererDelegate {
    func renderer(_ renderer: SCNSceneRenderer, updateAtTime time: TimeInterval) {
        for segment in segments {
            segment.update()
        }
    }
}
