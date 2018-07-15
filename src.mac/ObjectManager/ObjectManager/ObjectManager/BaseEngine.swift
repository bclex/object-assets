//
//  BaseEngine.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public class BaseEngine {
    static let desiredWorkTimePerFrame = 1.0 / 200
    static let cellRadiusOnLoad = 2
    public static var instance: BaseEngine? = nil

    public let assetManager: IAssetManager
    public let asset: IAssetPack
    public let data: IDataPack
    public let cellManager: ICellManager
    public let loadBalancer: TemporalLoadBalancer

    convenience init(assetManager: IAssetManager, asset: URL, data: URL) {
        self.init(assetManager: assetManager, asset: assetManager.getAssetPack(asset)!, data: assetManager.getDataPack(data)!)
    }
    init(assetManager: IAssetManager, asset: IAssetPack, data: IDataPack) {
        self.assetManager = assetManager
        self.asset = asset
        self.data = data
        loadBalancer = TemporalLoadBalancer()
        cellManager = assetManager.getCellManager(asset: asset, data: data, loadBalancer: loadBalancer)!
    }

    // MARK: Player Spawn

    var _currentWorld = 0
    var _currentCell: ICellRecord? = nil
    var _playerCamera: GameObject? = nil

    @discardableResult
    func createPlayer(player: GameObject, position: Vector3, playerCamera: inout GameObject?) -> GameObject {
//        let player = GameObject.find(withTag: "Player")
//        if player == nil {
////            player = GameObject.instantiate(playerPrefab)
////            player!.name = "Player"
//        }
        player.position = position
        playerCamera = player
        return player
     }
    
    public func spawnPlayer(player: GameObject, position: Vector3) {
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
    
    public func spawnPlayerAndUpdate(player: GameObject, position: Vector3) {
        let cellId = cellManager.getCellId(point: position, world: _currentWorld)
        _currentCell = data.findCellRecord(cellId)
        assert(_currentCell != nil)
        createPlayer(player: player, position: position, playerCamera: &_playerCamera)
        cellManager.updateCells(currentPosition: _playerCamera!.position, world: _currentWorld, immediate: true, cellRadiusOverride: BaseEngine.cellRadiusOnLoad)
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
            cellManager.updateCells(currentPosition: _playerCamera!.position, world: _currentWorld,
                immediate: false, cellRadiusOverride: -1)
        }
        loadBalancer.runTasks(desiredWorkTime: BaseEngine.desiredWorkTimePerFrame)
    }
}
