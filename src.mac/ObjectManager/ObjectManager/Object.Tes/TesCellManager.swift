//
//  TesCellManager.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation
import simd

public class TesCellManager: ICellManager {
    let _cellRadius: Int32 = 1 //1 //4
    let _detailRadius = 1 //1 //3
    let _defaultLandTextureFilePath = "textures/_land_default.dds"

    let _asset: TesAssetPack
    let _data: TesDataPack
    let _loadBalancer: TemporalLoadBalancer
    var _cellObjects = [int3 : InRangeCellInfo]()

    init(asset: TesAssetPack, data: TesDataPack, loadBalancer: TemporalLoadBalancer) {
        _asset = asset
        _data = data
        _loadBalancer = loadBalancer
    }
    
    public func getCellId(point: float3, world: Int32) -> int3 {
        return int3(
            (Float(point.x) / ConvertUtils.exteriorCellSideLengthInMeters).flooredAsInt32(),
            (Float(point.z) / ConvertUtils.exteriorCellSideLengthInMeters).flooredAsInt32(),
            world)
    }
    
    public func startCreatingCell(cellId: int3) -> InRangeCellInfo? {
        guard let cell = _data.findCellRecord(cellId: cellId) else {
            return nil
        }
        let cellInfo = startInstantiatingCell(cell: cell)
        _cellObjects[cellId.z != -1 ? cellId : int3.zero] = cellInfo
        return cellInfo
    }
    
    public func startCreatingCellByName(world: Int32, cellId: Int, cellName: String) -> InRangeCellInfo? {
        if world != -1 {
            fatalError("world")
        }
        guard let cell = _data.findCellRecordByName(world: world, cellId: cellId, cellName: cellName) else {
            return nil
        }
        let cellInfo = startInstantiatingCell(cell: cell)
        _cellObjects[int3.zero] = cellInfo
        return cellInfo
    }
    
    public func updateCells(currentPosition: float3, world: Int32, immediate: Bool = false, cellRadiusOverride: Int32 = -1) {
        let cameraCellId = getCellId(point: currentPosition, world: world)
        
        let cellRadius = cellRadiusOverride >= 0 ? cellRadiusOverride : _cellRadius
        let minCellX = cameraCellId.x - cellRadius
        let maxCellX = cameraCellId.x + cellRadius
        let minCellY = cameraCellId.y - cellRadius
        let maxCellY = cameraCellId.y + cellRadius
    
        // Destroy out of range cells.
        var outOfRangeCellIds = [int3]()
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
                    let cellId = int3(x, y, world)
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
    
    func destroyCell(cellId: int3) {
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
    
    // https://www.uraimo.com/2015/11/12/experimenting-with-swift-2-sequencetype-generatortype/
    func instantiateCellObjectsCoroutine(_ cell: CELLRecord, _ land: LANDRecord?, _ cellObj: GameObject, _ cellObjectsContainer: GameObject) -> CoTask {
        var state = 0
        var refCellObjInfos: [RefCellObjInfo]!
        var instantiateLANDTaskEnumerator: AnyIterator<Any>!
        var refCellObjInfoEnumerator: IndexingIterator<[RefCellObjInfo]>!
        return CoTask(AnyIterator { ()->Int? in
            switch state {
            case 0:
                // Start pre-loading all required textures for the terrain.
                guard let land = land else {
                    state += 1
                    return state
                }
                let landTextureFilePaths = self.getLANDTextureFilePaths(land)
                if landTextureFilePaths != nil {
                    for landTextureFilePath in landTextureFilePaths! {
                        self._asset.preloadTextureAsync(texturePath: landTextureFilePath)
                    }
                }
                state += 1
            case 1:
                // Extract information about referenced objects.
                refCellObjInfos = self.getRefCellObjInfos(cell)
                state += 1
            case 2:
                // Start pre-loading all required files for referenced objects. The NIF manager will load the textures as well.
                for refCellObjInfo in refCellObjInfos {
                    if refCellObjInfo.modelFilePath != nil {
//                        _asset.preloadObjectAsync(refCellObjInfo.modelFilePath)
                    }
                }
                state += 1
            case 3:
                // Instantiate terrain.
                guard land != nil else {
                    state += 3
                    return state
                }
                instantiateLANDTaskEnumerator = self.instantiateLANDCoroutine(land!, cellObj).co
                state += 1
            case 4:
                // Run the LAND instantiation coroutine.
                while instantiateLANDTaskEnumerator.next() != nil {
                    // Yield every time InstantiateLANDCoroutine does to avoid doing too much work in one frame.
                    return state
                }
                state += 1
            case 5:
                // Yield after InstantiateLANDCoroutine has finished to avoid doing too much work in one frame.
                state += 1
            case 6:
                // Instantiate objects.
                if refCellObjInfoEnumerator == nil { refCellObjInfoEnumerator = refCellObjInfos.makeIterator() }
                while true {
                    guard let refCellObjInfo = refCellObjInfoEnumerator.next() else {
                        break
                    }
                    self.instantiateCellObject(cell, cellObjectsContainer, refCellObjInfo)
                    return state
                }
                return nil
            default: return nil
            }
            return state
        })
    }
    
    func getRefCellObjInfos(_ cell: CELLRecord) -> [RefCellObjInfo] {
        guard _data.format == .TES3 else {
            return [RefCellObjInfo]()
        }
        let refCellObjInfos = cell.refObjs.map { ref -> RefCellObjInfo in
            // Get the record the RefObjDataGroup references.
            let refObj: CELLRecord.RefObj = ref
            let record = self._data._MANYsById[refObj.EDID]
            guard record != nil else {
                return RefCellObjInfo(refObj: ref)
            }
            var modelFilePath = (record as? IHaveMODL)?.MODL?.value
            // If the model file name is valid, store the model file path.
            if modelFilePath != nil {
                modelFilePath = "meshes\\\(modelFilePath!)"
            }
            return RefCellObjInfo(refObj: ref, referencedRecord: record, modelFilePath: modelFilePath)
        }
        return refCellObjInfos
    }

    func instantiateCellObject(_ cell: CELLRecord, _ parent: GameObject, _ refCellObjInfo: RefCellObjInfo) {
    }
    
    func instantiateLight(_ ligh: LIGHRecord, indoors: Bool) -> GameObject {
        return GameObject()
    }
    
    func postProcessInstantiatedCellObject(_ gameObject: GameObject, _ refCellObjInfo: RefCellObjInfo) {
    }
    
//    func processObjectType<RecordType>(gameObject: GameObject, info: RefCellObjInfo, tag: String) where RecordType: Record {
//    }
    
    func getLANDTextureFilePaths(_ land: LANDRecord) -> [String]? {
        // Don't return anything if the LAND doesn't have height data or texture data.
        guard land.VTEX != nil else {
            return nil
        }
        var textureFilePaths = [String]()
        for i in Set(land.VTEX!.textureIndicesT3) {
            let textureIndex = Int64(i) - 1
            if textureIndex < 0 {
                textureFilePaths.append(_defaultLandTextureFilePath)
                continue
            }
            let ltex = _data.findLTEXRecord(index: textureIndex)!
            let textureFilePath = ltex.ICON!
            textureFilePaths.append(textureFilePath)
        }
        return textureFilePaths
    }

    func instantiateLANDCoroutine(_ land: LANDRecord, _ parent: GameObject) -> CoTask {
        var state = 0
        let LAND_SIDELENGTH_IN_SAMPLES = 65
        var heights: [[Float]]!
        var extrema: (min: Float, max: Float)!
        var splatPrototypes: [SplatPrototype]!
        var textureIndicesT3: [UInt16]!
        var textureIndicesT3Enumerator: IndexingIterator<[UInt16]>!
        var texInd2SplatInd: [Int64 : Int]!
        var alphaMap: [[[Float]]]!
        return CoTask(AnyIterator { ()->Int? in
            switch state {
            case 0:
                // Don't create anything if the LAND doesn't have height data.
                if land.VTEX == nil || land.VHGT == nil {
                    return nil
                }
                // Return before doing any work to provide an IEnumerator handle to the coroutine.
                state += 1
            case 1:
                heights = Array(repeating: Array(repeating: Float(0),
                    count: LAND_SIDELENGTH_IN_SAMPLES), count: LAND_SIDELENGTH_IN_SAMPLES)
                // Read in the heights in Morrowind units.
                let VHGTIncrementToUnits: Float = 8
                var rowOffset = land.VHGT!.referenceHeight
                for y in 0..<LAND_SIDELENGTH_IN_SAMPLES {
                    rowOffset += Float(land.VHGT!.heightData[y * LAND_SIDELENGTH_IN_SAMPLES])
                    heights[y][0] = rowOffset * VHGTIncrementToUnits
                    var colOffset = rowOffset
                    for x in 1..<LAND_SIDELENGTH_IN_SAMPLES {
                        colOffset += Float(land.VHGT!.heightData[(y * LAND_SIDELENGTH_IN_SAMPLES) + x])
                        heights[y][x] = colOffset * VHGTIncrementToUnits
                    }
                }
                // Change the heights to percentages.
                extrema = heights.getExtrema()
                for y in 0..<LAND_SIDELENGTH_IN_SAMPLES {
                    for x in 0..<LAND_SIDELENGTH_IN_SAMPLES {
                        heights[y][x] = Utils.changeRange(x: heights[y][x], min0: extrema.min, max0: extrema.max, min1: 0, max1: 1)
                    }
                }
                
                // Texture the terrain.
                splatPrototypes = [SplatPrototype]()
                let LAND_TEXTUREINDICES = 256
                textureIndicesT3 = land.VTEX?.textureIndicesT3 ?? Array(repeating: UInt16(0), count: LAND_TEXTUREINDICES)
                textureIndicesT3Enumerator = textureIndicesT3.makeIterator()
                // Create splat prototypes.
                texInd2SplatInd = [Int64 : Int]()
                state += 1
            case 2:
                while true {
                    guard let i = textureIndicesT3Enumerator.next() else {
                        break
                    }
                    let textureIndex = Int64(i) - 1
                    guard texInd2SplatInd[textureIndex] == nil else {
                        continue
                    }
                    // Load terrain texture.
                    var textureFilePath: String
                    if textureIndex < 0 {
                        textureFilePath = self._defaultLandTextureFilePath
                    }
                    else {
                        let ltex = self._data.findLTEXRecord(index: textureIndex)!
                        textureFilePath = ltex.ICON!
                    }
                    let texture = self._asset.loadTexture(texturePath: textureFilePath)
                    // Create the splat prototype.
                    let splat = SplatPrototype(
                        texture: texture,
                        smoothness: 0,
                        metallic: 0,
                        tileSize: int2(6, 6))
                    // Update collections.
                    let splatIndex = splatPrototypes.count
                    splatPrototypes.append(splat)
                    texInd2SplatInd[textureIndex] = splatIndex
                    // Yield after loading each texture to avoid doing too much work on one frame.
                    return state
                }
                state += 1
            case 3:
                // Create the alpha map.
                let VTEX_ROWS = 16
                let VTEX_COLUMNS = VTEX_ROWS
                alphaMap = Array(repeating: Array(repeating: Array(repeating: Float(0),
                    count: splatPrototypes.count), count: VTEX_COLUMNS), count: VTEX_ROWS)
                for y in 0..<VTEX_ROWS{
                    let yMajor = y / 4
                    let yMinor = y - (yMajor * 4)
                    for x in 0..<VTEX_COLUMNS {
                        let xMajor = x / 4
                        let xMinor = x - (xMajor * 4)
                        let texIndex = Int64(textureIndicesT3[(yMajor * 64) + (xMajor * 16) + (yMinor * 4) + xMinor]) - 1
                        if texIndex >= 0 { let splatIndex = texInd2SplatInd![texIndex]!; alphaMap[y][x][splatIndex] = 1 }
                        else { alphaMap[y][x][0] = 1 }
                    }
                }
                // Yield before creating the terrain GameObject because it takes a while.
                state += 1
            case 4:
                // Create the terrain.
                let heightRange = extrema.max - extrema.min
                let terrainPosition = float3(ConvertUtils.exteriorCellSideLengthInMeters * Float(land.gridId.x), extrema.min / ConvertUtils.meterInUnits, ConvertUtils.exteriorCellSideLengthInMeters * Float(land.gridId.y))
                let heightSampleDistance = ConvertUtils.exteriorCellSideLengthInMeters / Float(LAND_SIDELENGTH_IN_SAMPLES - 1)
                _ = GameObjectUtils.createTerrain(offset: -1, heightPercents: heights, maxHeight: heightRange / ConvertUtils.meterInUnits, heightSampleDistance: heightSampleDistance, splatPrototypes: splatPrototypes, alphaMap: alphaMap, position: terrainPosition)
//                terrain.transform.parent = parent.transform
//                terrain.isStatic = true
                return nil
            default: return nil
            }
            return state
        })
    }
}
