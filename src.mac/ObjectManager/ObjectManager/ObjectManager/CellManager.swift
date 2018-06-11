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
    var ambientLight: CGColor? {get}
}

public class InRangeCellInfo {
    var gameObject: SCNNode
    var objectsContainerGameObject: SCNNode
    var cellRecord: ICellRecord
    var objectsCreationCoroutine: AnyIterator<Any>
    
    init(gameObject: SCNNode, objectsContainerGameObject: SCNNode, cellRecord: ICellRecord, objectsCreationCoroutine: AnyIterator<Any>) {
        self.gameObject = gameObject
        self.objectsContainerGameObject = objectsContainerGameObject
        self.cellRecord = cellRecord
        self.objectsCreationCoroutine = objectsCreationCoroutine
    }
}

public class RefCellObjInfo {
    var refObj: Any
    var referencedRecord: IRecord
    var modelFilePath: String
    
    init(refObj: Any, referencedRecord: IRecord, modelFilePath: String) {
        self.refObj = refObj
        self.referencedRecord = referencedRecord
        self.modelFilePath = modelFilePath
    }
}

public protocol ICellManager {
    //func getExteriorCellIndices(point: Vector3) -> Vector2
    //func startCreatingExteriorCell(cellIndices: Vector2) -> InRangeCellInfo
    //func startCreatingInteriorCell(cellName: String) -> InRangeCellInfo
    //func startCreatingInteriorCell(gridCoords: Vector2) -> InRangeCellInfo
    //func updateExteriorCells(currentPosition: Vector3, immediate: Bool, cellRadiusOverride: Int)
    //func destroyAllCells()
}
