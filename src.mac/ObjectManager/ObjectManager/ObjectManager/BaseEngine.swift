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

    var _currentWorld: Int!
    var _currentCell: ICellRecord? = nil
    // var _playerTransform: Transform 
    // var _playerComponent: PlayerComponent
    var _playerCameraObj: GameObject!

    @discardableResult
    func createPlayer(playerPrefab: GameObject, position: Vector3, playerCamera: inout GameObject) -> GameObject {
        let player = GameObject.find(withTag: "Player")
        if player == nil {
//            player = GameObject.instantiate(playerPrefab)
//            player!.name = "Player"
        }
        player!.position = position
        
//         _playerTransform = player.GetComponent<Transform>()
//         var cameraInPlayer = player.GetComponentInChildren<Camera>();
//         if (cameraInPlayer == null)
//             throw new InvalidOperationException("Player:Camera missing");
//         playerCamera = cameraInPlayer.gameObject;
//         _playerComponent = player.GetComponent<PlayerComponent>();
//         //_underwaterEffect = playerCamera.GetComponent<UnderwaterEffect>();
        return player!
     }
    
    public func spawnPlayer(playerPrefab: GameObject, gridId: Vector3Int, position: Vector3) {
        _currentWorld = gridId.z
        _currentCell = data.findCellRecord(gridId)
        assert(_currentCell != nil)
        createPlayer(playerPrefab: playerPrefab, position: position, playerCamera: &_playerCameraObj)
        if let cellInfo = cellManager.startCreatingCell(cellId: gridId) {
            loadBalancer.waitForTask(taskCoroutine: cellInfo.objectsCreationCoroutine)
        }
        if gridId.z != -1 { onExteriorCell(cell: _currentCell!) }
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
            cellManager.updateCells(currentPosition: _playerCameraObj!.position, world: _currentWorld,
                immediate: false, cellRadiusOverride: -1)
        }
        loadBalancer.runTasks(desiredWorkTime: BaseEngine.desiredWorkTimePerFrame)
    }
}
