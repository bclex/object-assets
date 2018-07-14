//
//  TesCellManager.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public class TesCellManager: ICellManager {
    let _cellRadius: Int = 1 //4
    let _detailRadius: Int = 1 //3
    let _defaultLandTextureFilePath: String = "textures/_land_default.dds"

    let _asset: TesAssetPack
    let _data: TesDataPack
    let _loadBalancer: TemporalLoadBalancer
    var _cellObjects = [Vector3Int : InRangeCellInfo]()

    init(asset: TesAssetPack, data: TesDataPack, loadBalancer: TemporalLoadBalancer) {
        _asset = asset
        _data = data
        _loadBalancer = loadBalancer
    }
    
    public func getCellId(point: Vector3, world: Int) -> Vector3Int {
        return Vector3Int(
            (Float(point.x) / ConvertUtils.exteriorCellSideLengthInMeters).flooredAsInt(),
            (Float(point.z) / ConvertUtils.exteriorCellSideLengthInMeters).flooredAsInt(),
            world)
    }
    
    public func startCreatingCell(cellId: Vector3Int) -> InRangeCellInfo? {
        guard let cell = _data.findCellRecord(cellId: cellId) else {
            return nil
        }
        let cellInfo = startInstantiatingCell(cell: cell)
        _cellObjects[cellId.z != -1 ? cellId : Vector3Int.zero] = cellInfo
        return cellInfo
    }
    
    public func startCreatingCellByName(world: Int, cellId: Int, cellName: String) -> InRangeCellInfo? {
        if world != -1 {
            fatalError("world")
        }
        guard let cell = _data.findCellRecordByName(world: world, cellId: cellId, cellName: cellName) else {
            return nil
        }
        let cellInfo = startInstantiatingCell(cell: cell)
        _cellObjects[Vector3Int.zero] = cellInfo
        return cellInfo
    }
    
    public func updateCells(currentPosition: Vector3, world: Int, immediate: Bool = false, cellRadiusOverride: Int = -1) {
        let cameraCellId = getCellId(point: currentPosition, world: world)
        
        let cellRadius = cellRadiusOverride >= 0 ? cellRadiusOverride : _cellRadius
        let minCellX = cameraCellId.x - cellRadius
        let maxCellX = cameraCellId.x + cellRadius
        let minCellY = cameraCellId.y - cellRadius
        let maxCellY = cameraCellId.y + cellRadius
    
        // Destroy out of range cells.
        var outOfRangeCellIds = [Vector3Int]()
        for x in _cellObjects {
            if x.key.x < minCellX || x.key.x > maxCellX || x.key.y < minCellY || x.key.y > maxCellY {
                outOfRangeCellIds.append(x.key)
            }
        }
        for cellId in outOfRangeCellIds {
            destroyCell(cellId: cellId)
        }
        
        // Create new cells.
        for r in 0..<cellRadius {
            for x in minCellX..<maxCellX {
                for y in minCellY..<maxCellY {
                    let cellId = Vector3Int(x, y, world)
                    let cellXDistance = abs(cameraCellId.x - cellId.x)
                    let cellYDistance = abs(cameraCellId.y - cellId.y)
                    let cellDistance = max(cellXDistance, cellYDistance)
                    if cellDistance == r && _cellObjects[cellId] == nil {
                        let cellInfo = startCreatingCell(cellId: cellId)
                        if cellInfo != nil && immediate {
                            _loadBalancer.waitForTask(taskCoroutine: cellInfo!.objectsCreationCoroutine)
                        }
                    }
                }
            }
        }
        
        // Update LODs.
        for x in _cellObjects {
            let cellId = x.key
            let cellInfo = x.value
            let cellXDistance = abs(cameraCellId.x - cellId.x)
            let cellYDistance = abs(cameraCellId.y - cellId.y)
            let cellDistance = max(cellXDistance, cellYDistance)
            if cellDistance <= _detailRadius {
                if !cellInfo.objectsContainerGameObject.activeSelf {
                    cellInfo.objectsContainerGameObject.setActive(true)
                }
            }
            else {
                if cellInfo.objectsContainerGameObject.activeSelf {
                    cellInfo.objectsContainerGameObject.setActive(false)
                }
            }
        }
    }
    
    public func startInstantiatingCell(cell: CELLRecord) -> InRangeCellInfo {
        var cellObjName: String = ""
        var land: LANDRecord? = nil
        if !cell.isInterior {
            cellObjName = "cell \(cell.gridId!)"
            land = _data.findLANDRecord(cellId: cell.gridId)
        }
        else { cellObjName = cell.EDID }
        let cellObj = GameObject(name: cellObjName, tag: "Cell")
        let cellObjectsContainer = GameObject(name: "objects")
        //cellObjectsContainer.transform.parent = cellObj.transform
        let cellObjectsCreationCoroutine = instantiateCellObjectsCoroutine(cell, land, cellObj, cellObjectsContainer)
        _loadBalancer.addTask(taskCoroutine: cellObjectsCreationCoroutine)
        return InRangeCellInfo(gameObject: cellObj, objectsContainerGameObject: cellObjectsContainer, cellRecord: cell, objectsCreationCoroutine: cellObjectsCreationCoroutine)
    }
    
    func destroyCell(cellId: Vector3Int) {
        guard let cellInfo = _cellObjects[cellId] else {
            debugPrint("Tried to destroy a cell that isn't created.")
            return
        }
        _loadBalancer.cancelTask(taskCoroutine: cellInfo.objectsCreationCoroutine)
        GameObject.destroy(cellInfo.gameObject)
        _cellObjects.removeValue(forKey: cellId)
    }
    
    public func destroyAllCells() {
        for x in _cellObjects {
            _loadBalancer.cancelTask(taskCoroutine: x.value.objectsCreationCoroutine)
            GameObject.destroy(x.value.gameObject)
        }
        _cellObjects.removeAll()
    }
    
    func instantiateCellObjectsCoroutine(_ cell: CELLRecord, _ land: LANDRecord?, _ cellObj: GameObject, _ cellObjectsContainer: GameObject) -> CoTask {
        fatalError()
    }
}
