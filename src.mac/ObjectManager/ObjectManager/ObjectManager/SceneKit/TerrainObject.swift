//
//  TerrainNode.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import SceneKit

public class TerrainObject: GameObject {
    let data: TerrainData
    var nodes: SCNNode? = nil
    
    enum RenderType {
        case strips, triangles, flattened
    }
    
    init(_ data: TerrainData, name: String, tag: String? = nil) {
        self.data = data
        super.init()
        data.runAlgorithm()
//        reloadGeometry()
    }
    
    required init?(coder aDecoder: NSCoder) {
        self.data = TerrainData(heightmapResolution: 0)
        super.init(coder: aDecoder)
    }
    
    public override func observeValue(forKeyPath keyPath: String?, of object: Any?, change: [NSKeyValueChangeKey : Any]?, context: UnsafeMutableRawPointer?) {
        if keyPath == "layers" {
            //self.reloadGeometry()
        }
        else {
            super.observeValue(forKeyPath: keyPath, of: object, change: change, context: context)
        }
    }
    
    public func reloadGeometry() {
        nodes = nil
        loadGeometry(.flattened)
    }
    
    func loadGeometry(_ type: RenderType) {
        var rootNode: SCNNode? = nil
        let terrainMaterial = SCNMaterial()
        let map = data.map
        switch type {
        case .strips:
            var vertecies = [Vertex]()
            var triangles = [CInt]()
            let nodes = map.nodes
            for tile in nodes {
                vertecies.append(tile.vertex)
                triangles.append(CInt(tile.index))
                let index = tile.index + Int(map.width)
                triangles.append(CInt(index))
                if map.translateIndex(Int(tile.index)).x == map.width - 1 {
                    triangles.append(CInt(tile.index + map.width))
                    triangles.append(CInt(tile.index + 1))
                }
            }
            let geometry = createStripGeometry(vertecies, triangles: triangles)
            geometry.materials = [terrainMaterial]
            self.geometry = geometry
        case .triangles:
            rootNode = SCNNode()
            genTris(rootNode!)
        case .flattened:
            rootNode = SCNNode()
            genTris(rootNode!)
            rootNode = rootNode?.flattenedClone()
        }
        if let root = rootNode {
            root.name = "terrain"
            self.addChildNode(root)
        }
    }
    
    func genTris(_ root: SCNNode) {
        let terrainMaterial = SCNMaterial()
        let map = data.map
        let nodes = map.nodes
        for tile in nodes {
            let pos = map.translateIndex(tile.index)
            if pos.x < (map.width - 1) && pos.y > 0 {
                var verts = tile.upTriangle
                var triangles: [CInt] = [0, 2, 1]
                
                let geometry = createTriangleGeometry(verts, triangles: triangles)
                geometry.materials = [terrainMaterial]
                let childNode = SCNNode(geometry: geometry)
                root.addChildNode(childNode)
                
                verts = tile.downTriangle
                triangles = [0, 2, 1]
                let downGeometry = createTriangleGeometry(verts, triangles: triangles)
                
                downGeometry.materials = [terrainMaterial]
                let downTriangle = SCNNode(geometry: downGeometry)
                
                root.addChildNode(downTriangle)
            }
        }
    }
}
