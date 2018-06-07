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

    var _currentCell: ICellRecord? = nil
    // var _playerTransform: Transform 
    // var _playerComponent: PlayerComponent
    // var _playerCameraObj: GameObject

    // func createPlayer(playerPrefab: GameObject, position: Vector3, out GameObject playerCamera) -> GameObject {
    //     var player = GameObject.FindWithTag("Player");
    //     if (player == null)
    //     {
    //         player = GameObject.Instantiate(playerPrefab);
    //         player.name = "Player";
    //     }
    //     player.transform.position = position;
    //     _playerTransform = player.GetComponent<Transform>();
    //     var cameraInPlayer = player.GetComponentInChildren<Camera>();
    //     if (cameraInPlayer == null)
    //         throw new InvalidOperationException("Player:Camera missing");
    //     playerCamera = cameraInPlayer.gameObject;
    //     _playerComponent = player.GetComponent<PlayerComponent>();
    //     //_underwaterEffect = playerCamera.GetComponent<UnderwaterEffect>();
    //     return player;
    // }

    // public void SpawnPlayerInside(GameObject playerPrefab, string interiorCellName, Vector3 position)
    // {
    //     _currentCell = Data.FindInteriorCellRecord(interiorCellName);
    //     Debug.Assert(_currentCell != null);
    //     CreatePlayer(playerPrefab, position, out _playerCameraObj);
    //     var cellInfo = CellManager.StartCreatingInteriorCell(interiorCellName);
    //     LoadBalancer.WaitForTask(cellInfo.ObjectsCreationCoroutine);
    //     OnInteriorCell(_currentCell);
    // }

    // public void SpawnPlayerInside(GameObject playerPrefab, Vector2i gridCoords, Vector3 position)
    // {
    //     _currentCell = Data.FindInteriorCellRecord(gridCoords);
    //     Debug.Assert(_currentCell != null);
    //     CreatePlayer(playerPrefab, position, out _playerCameraObj);
    //     var cellInfo = CellManager.StartCreatingInteriorCell(gridCoords);
    //     LoadBalancer.WaitForTask(cellInfo.ObjectsCreationCoroutine);
    //     OnInteriorCell(_currentCell);
    // }

    // public void SpawnPlayerOutside(GameObject playerPrefab, Vector2i gridCoords, Vector3 position)
    // {
    //     _currentCell = Data.FindExteriorCellRecord(gridCoords);
    //     Debug.Assert(_currentCell != null);
    //     CreatePlayer(playerPrefab, position, out _playerCameraObj);
    //     var cellInfo = CellManager.StartCreatingExteriorCell(gridCoords);
    //     LoadBalancer.WaitForTask(cellInfo.ObjectsCreationCoroutine);
    //     OnExteriorCell(_currentCell);
    // }

    // public void SpawnPlayerOutside(GameObject playerPrefab, Vector3 position)
    // {
    //     var cellIndices = CellManager.GetExteriorCellIndices(position);
    //     _currentCell = Data.FindExteriorCellRecord(cellIndices);
    //     CreatePlayer(playerPrefab, position, out _playerCameraObj);
    //     CellManager.UpdateExteriorCells(_playerCameraObj.transform.position, true, CellRadiusOnLoad);
    //     OnExteriorCell(_currentCell);
    // }

    func onExteriorCell(CELL: ICellRecord) {
    }

    func onInteriorCell(CELL: ICellRecord) {
    }

    // MARK: Update
    func update() {
        // // The current cell can be null if the player is outside of the defined game world
        // if _currentCell == nil || !_currentCell.isInterior {
        //     cellManager.updateExteriorCells(_playerCameraObj.transform.position)
        // }
        // loadBalancer.RunTasks(desiredWorkTimePerFrame)
    }
}
