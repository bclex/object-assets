//
//  TesDataPackTest.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation
import SceneKit
import simd

public class TesTerrainTest: GameSegment {
    
    var Asset: IAssetPack? = nil
    var Data: IDataPack? = nil
    var cellManager: ICellManager!
    var loadBalancer: TemporalLoadBalancer!
    var _currentCell: ICellRecord? = nil
    
    public init() {
    }
    
    public func start(rootNode: SCNNode, player: GameObject) {
        let assetUrl = URL(string: "game://Morrowind/Morrowind.bsa")
        let dataUrl = URL(string: "game://Morrowind/Morrowind.esm")

        let assetManager = AssetManager.getAssetManager(.tes)
        Asset = assetManager.getAssetPack(assetUrl)
        Data = assetManager.getDataPack(dataUrl)
        loadBalancer = TemporalLoadBalancer()
        cellManager = assetManager.getCellManager(rootNode: rootNode, asset: Asset!, data: Data!, loadBalancer: loadBalancer)!
        
        let newX = 23 * ConvertUtils.exteriorCellSideLengthInMeters
        let newZ = (-5 * ConvertUtils.exteriorCellSideLengthInMeters) + 100
        spawnLand(position: float3(newX, 2.30, newZ))
    }
    
    public func spawnLand(position: float3) {
        let cellId = cellManager.getCellId(point: position, world: 0)
        self._currentCell = Data!.findCellRecord(cellId)
        assert(_currentCell != nil)
        if let cellInfo = cellManager.startCreatingCell(cellId: cellId) {
            loadBalancer.waitForTask(taskCoroutine: cellInfo.objectsCreationCoroutine)
        }
    }

    public func onDestroy() {
        if Asset != nil {
            Asset!.close()
            Asset = nil
        }
        if Data != nil {
            Data!.close()
            Data = nil
        }
    }
    
    public func update() {
    }
}
