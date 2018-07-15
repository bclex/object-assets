//
//  CellManager.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/29/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation
import CoreGraphics
import SceneKit

public protocol ICellRecord : IRecord {
    var isInterior: Bool {get}
    //var ambientLight: CGColor? {get}
}

public class InRangeCellInfo {
    var gameObject: GameObject
    var objectsContainerGameObject: GameObject
    var cellRecord: ICellRecord
    var objectsCreationCoroutine: CoTask
    
    init(gameObject: GameObject, objectsContainerGameObject: GameObject, cellRecord: ICellRecord, objectsCreationCoroutine: CoTask) {
        self.gameObject = gameObject
        self.objectsContainerGameObject = objectsContainerGameObject
        self.cellRecord = cellRecord
        self.objectsCreationCoroutine = objectsCreationCoroutine
    }
}

public class RefCellObjInfo {
    var refObj: Any
    var referencedRecord: IRecord?
    var modelFilePath: String?
    
    init(refObj: Any, referencedRecord: IRecord? = nil, modelFilePath: String? = nil) {
        self.refObj = refObj
        self.referencedRecord = referencedRecord
        self.modelFilePath = modelFilePath
    }
}

public protocol ICellManager {
    func getCellId(point: Vector3, world: Int) -> Vector3Int
    func startCreatingCell(cellId: Vector3Int) -> InRangeCellInfo?
    func startCreatingCellByName(world: Int, cellId: Int, cellName: String) -> InRangeCellInfo?
    func updateCells(currentPosition: Vector3, world: Int, immediate: Bool, cellRadiusOverride: Int)
    func destroyAllCells()
}
