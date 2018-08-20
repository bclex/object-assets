//
//  BaseEngine.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation
import SceneKit
import simd

public class BaseEngine {
    static let desiredWorkTimePerFrame = 1.0 / 200
    static let cellRadiusOnLoad: Int32 = 2
    public static var instance: BaseEngine? = nil

    public let assetManager: IAssetManager
    public let asset: IAssetPack
    public let data: IDataPack
    public let cellManager: ICellManager
    public let loadBalancer: TemporalLoadBalancer

    convenience init(rootNode: SCNNode, assetManager: IAssetManager, asset: URL, data: URL) {
        self.init(rootNode: rootNode, assetManager: assetManager, asset: assetManager.getAssetPack(asset)!, data: assetManager.getDataPack(data)!)
    }
    init(rootNode: SCNNode, assetManager: IAssetManager, asset: IAssetPack, data: IDataPack) {
        self.assetManager = assetManager
        self.asset = asset
        self.data = data
        loadBalancer = TemporalLoadBalancer()
        cellManager = assetManager.getCellManager(rootNode: rootNode, asset: asset, data: data, loadBalancer: loadBalancer)!
        
        // ambient
        //        let ambientLightNode = SCNNode()
        //        ambientLightNode.light = SCNLight()
        //        ambientLightNode.light!.type = .ambient
        //        ambientLightNode.light!.color = NSColor(white: 0.67, alpha: 1.0)
        //        rootNode.addChildNode(ambientLightNode)
        
        // sun
        //        let omniLightNode = SCNNode()
        //        omniLightNode.light = SCNLight()
        //        omniLightNode.light!.type = .omni
        //        omniLightNode.light!.color = NSColor(white: 0.75, alpha: 1.0)
        //        omniLightNode.simdPosition = float3(0, 50, 50)
        //        rootNode.addChildNode(omniLightNode)
    }

    // MARK: Player Spawn

    var _currentWorld: Int32 = 0
    var _currentCell: ICellRecord? = nil
    var _playerCamera: GameObject? = nil

    @discardableResult
    func createPlayer(player: GameObject, position: float3, playerCamera: inout GameObject?) -> GameObject {
//        let player = GameObject.find(withTag: "Player")
//        if player == nil {
////            player = GameObject.instantiate(playerPrefab)
////            player!.name = "Player"
//        }
        player.simdPosition = position
        playerCamera = player
        return player
     }
    
    public func spawnPlayer(player: GameObject, position: float3) {
        let cellId = cellManager.getCellId(point: position, world: _currentWorld)
        _currentCell = data.findCellRecord(cellId)
        assert(_currentCell != nil)
        createPlayer(player: player, position: position, playerCamera: &_playerCamera)
        if let cellInfo = cellManager.startCreatingCell(cellId: cellId) {
            loadBalancer.waitForTask(taskCoroutine: cellInfo.objectsCreationCoroutine)
        }
        if cellId.z != -1 { onExteriorCell(cell: _currentCell!) }
        else { onInteriorCell(cell: _currentCell!) }
    }
    
    public func spawnPlayerAndUpdate(player: GameObject, position: float3) {
        let cellId = cellManager.getCellId(point: position, world: _currentWorld)
        _currentCell = data.findCellRecord(cellId)
        assert(_currentCell != nil)
        createPlayer(player: player, position: position, playerCamera: &_playerCamera)
        cellManager.updateCells(currentPosition: _playerCamera!.simdPosition, world: _currentWorld, immediate: true, cellRadiusOverride: BaseEngine.cellRadiusOnLoad)
        if cellId.z != -1 { onExteriorCell(cell: _currentCell!) }
        else { onInteriorCell(cell: _currentCell!) }
    }

    func onExteriorCell(cell: ICellRecord) {
    }

    func onInteriorCell(cell: ICellRecord) {
    }

    // MARK: Update
    func update() {
        // The current cell can be null if the player is outside of the defined game world
        if _currentCell == nil || !_currentCell!.isInterior {
            cellManager.updateCells(currentPosition: _playerCamera!.simdPosition, world: _currentWorld,
                immediate: false, cellRadiusOverride: -1)
        }
        loadBalancer.runTasks(desiredWorkTime: BaseEngine.desiredWorkTimePerFrame)
    }
}
