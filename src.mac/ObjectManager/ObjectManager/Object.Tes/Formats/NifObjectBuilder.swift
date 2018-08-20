//
//  NifObjectBuilder.swift
//  ObjectManager
//
//  Created by Sky Morey on 6/5/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import SceneKit

public class NifObjectBuilder {
    let _file: NiFile
    let _materialManager: MaterialManager
    let _markerLayer: Int

    public init(file: NiFile, materialManager: MaterialManager, markerLayer: Int) {
        _file = file
        _materialManager = materialManager
        _markerLayer = markerLayer
    }

    public func buildObject() -> GameObject {
        assert(_file.footer.roots.count > 0)

        // NIF files can have any number of root NiObjects.
        // If there is only one root, instantiate that directly.
        // If there are multiple roots, create a container GameObject and parent it to the roots.
        if _file.footer.roots.count == 1 {
            let rootNiObject = _file.blocks[Int(_file.footer.roots[0])]
            var gameObject = instantiateRootNiObject(rootNiObject)
            // If the file doesn't contain any NiObjects we are looking for, return an empty GameObject.
            if gameObject == nil {
                debugPrint("\(_file.name) resulted in a null GameObject when instantiated.")
                gameObject = GameObject(name: _file.name)
            }
            // If gameObject != null and the root NiObject is an NiNode, discard any transformations (Morrowind apparently does).
            else if rootNiObject is NiNode {
                gameObject!.simdPosition = float3.zero
                gameObject!.simdRotation = simd_quatf.identity
                gameObject!.simdScale = float3.one
            }
            return gameObject! //.flattenedClone()
        }
        else {
            debugPrint("\(_file.name) has multiple roots.")
            let gameObject = GameObject(name: _file.name)
            for rootRef in _file.footer.roots {
                let child = instantiateRootNiObject(_file.blocks[Int(rootRef)])
                if child != nil {
                    gameObject.addChildNode(child!)
                }
            }
            return gameObject
        }
    }

    func instantiateRootNiObject(_ obj: NiObject) -> GameObject? {
        let gameObject = instantiateNiObject(obj)
        var shouldAddMissingColliders = false
        var isMarker = true
        processExtraData(obj, &shouldAddMissingColliders, &isMarker)
        if _file.name.count > 0 && NifObjectBuilder.isMarkerFileName(_file.name) {
            shouldAddMissingColliders = false
            isMarker = true
        }
        // Add colliders to the object if it doesn't already contain one.
//        if shouldAddMissingColliders && gameObject.getComponentInChildren<Collider>() == nil {
////            GameObjectUtils.addMissingMeshCollidersRecursively(gameObject)
//        }
        if isMarker {
//            GameObjectUtils.setLayerRecursively(gameObject, _markerLayer)
        }
        return gameObject
    }

    func processExtraData(_ obj: NiObject, _ shouldAddMissingColliders: inout Bool, _ isMarker: inout Bool) {
        shouldAddMissingColliders = true
        isMarker = false
        guard let objNET = obj as? NiObjectNET else {
            return
        }
        var extraData = objNET.extraData.value >= 0 ? _file.blocks[Int(objNET.extraData.value)] as? NiExtraData : nil
        while extraData != nil {
            if let strExtraData = extraData as? NiStringExtraData {
                if strExtraData.str == "NCO" || strExtraData.str == "NCC" {
                    shouldAddMissingColliders = false
                }
                else if strExtraData.str == "MRK" {
                    shouldAddMissingColliders = false
                    isMarker = true
                }
            }
            // Move to the next NiExtraData.
            if extraData!.nextExtraData.value >= 0 { extraData = _file.blocks[Int(extraData!.nextExtraData.value)] as? NiExtraData }
            else { extraData = nil }
        }
    }

    func instantiateNiObject(_ obj: NiObject) -> GameObject? {
        let objType = type(of: obj)
        if objType == NiNode.self { return instantiateNiNode(obj as! NiNode) }
        else if objType == NiBSAnimationNode.self { return instantiateNiNode(obj as! NiNode) }
        else if objType == NiTriShape.self { return instantiateNiTriShape(obj as! NiTriShape, true, false) }
        else if objType == RootCollisionNode.self { return instantiateRootCollisionNode(obj as! RootCollisionNode) }
        else if objType == NiTextureEffect.self { return nil }
        else if objType == NiBSAnimationNode.self { return nil }
        else if objType == NiBSParticleNode.self { return nil }
        else if objType == NiRotatingParticles.self { return nil }
        else if objType == NiAutoNormalParticles.self { return nil }
        else if objType == NiBillboardNode.self { return nil }
        else { fatalError("Tried to instantiate an unsupported NiObject (\(objType).") }
    }

    func instantiateNiNode(_ node: NiNode) -> GameObject {
        let obj = GameObject(name: node.name)
        for childIndex in node.children {
            // NiNodes can have child references < 0 meaning null.
            if !childIndex.isNull {
                let child = instantiateNiObject(_file.blocks[Int(childIndex.value)])
                if child != nil {
                    obj.addChildNode(child!)
                }
            }
        }
        applyNiAVObject(node, obj)
        return obj
    }

    func instantiateNiTriShape(_ triShape: NiTriShape, _ visual: Bool, _ collidable: Bool) -> GameObject {
//        var game = BaseSettings.game
        assert(visual || collidable)
        let mesh = niTriShapeDataToMesh(_file.blocks[Int(triShape.data.value)] as! NiTriShapeData)
        let obj = GameObject(name: triShape.name)
        if visual {
            obj.geometry = mesh
            let materialProps = niAVObjectPropertiesToMaterialProperties(triShape)
            obj.geometry!.materials = [_materialManager.buildMaterialFromProperties(materialProps)]
            if triShape.flags.contains(.hidden) {
                obj.isHidden = true
            }
//            obj.isStatic = true
        }
        if collidable {
            obj.physicsBody = SCNPhysicsBody(type: .static, shape: nil)
//            obj.AddComponent<MeshCollider>().sharedMesh = mesh
//            if game.kinematicRigidbodies {
//                obj.AddComponent<Rigidbody>().isKinematic = true
//            }
        }
        applyNiAVObject(triShape, obj)
        return obj
    }

    func instantiateRootCollisionNode(_ collisionNode: RootCollisionNode) -> GameObject {
        let obj = GameObject(name: "Root Collision Node");
        for childIndex in collisionNode.children {
            // NiNodes can have child references < 0 meaning null.
            if !childIndex.isNull {
                addColliderFromNiObject(_file.blocks[Int(childIndex.value)], obj)
            }
        }
        applyNiAVObject(collisionNode, obj)
        return obj
    }

    func applyNiAVObject(_ niAVObject: NiAVObject, _ obj: GameObject) {
        obj.simdPosition = NifUtils.nifPointToUnityPoint(niAVObject.translation)
        obj.simdRotation = NifUtils.nifRotationMatrixToUnityQuaternion(niAVObject.rotation).vector
        obj.simdScale = niAVObject.scale * float3.one
    }

    func niTriShapeDataToMesh(_ data: NiTriShapeData) -> SCNGeometry {
         let verticesCount = data.vertices.count
        // vertex positions
        let vertices = data.vertices.map { NifUtils.nifPointToUnityPoint($0) }
        var geometrySources = [SCNGeometrySource(
            data: Data(bytes: UnsafeRawPointer(vertices), count: verticesCount * MemoryLayout<Float3>.size),
            semantic: SCNGeometrySource.Semantic.vertex,
            vectorCount: verticesCount,
            usesFloatComponents: true,
            componentsPerVector: 3,
            bytesPerComponent: MemoryLayout<Float>.size,
            dataOffset: 0,
            dataStride: MemoryLayout<Float3>.size)]
        // vertex normals
        var normals: [Float3]? = nil
        if data.hasNormals {
            normals = data.normals.map { NifUtils.nifVectorToUnityVector($0) }
            geometrySources.append(SCNGeometrySource(
                data: Data(bytes: UnsafeRawPointer(normals!), count: verticesCount * MemoryLayout<Float3>.size),
                semantic: SCNGeometrySource.Semantic.normal,
                vectorCount: verticesCount,
                usesFloatComponents: true,
                componentsPerVector: 3,
                bytesPerComponent: MemoryLayout<Float>.size,
                dataOffset: 0,
                dataStride: MemoryLayout<Float3>.size))
        }
        // vertex UV coordinates
        var uvs: [float2]? = nil
        if data.hasUV {
            uvs = data.uvSets[0]
            geometrySources.append(SCNGeometrySource(
                data: Data(bytes: UnsafeRawPointer(uvs!), count: verticesCount * MemoryLayout<float2>.size),
                semantic: SCNGeometrySource.Semantic.texcoord,
                vectorCount: verticesCount,
                usesFloatComponents: true,
                componentsPerVector: 2,
                bytesPerComponent: MemoryLayout<Float>.size,
                dataOffset: 0,
                dataStride: MemoryLayout<float2>.size))
        }
        // triangle vertex indices
        let trianglesCount = Int(data.numTrianglePoints)
        var triangles = [Int32](); triangles.reserveCapacity(trianglesCount)
        for i in 0..<data.triangles.count {
            // Reverse triangle winding order.
            triangles.append(Int32(data.triangles[i].v1))
            triangles.append(Int32(data.triangles[i].v3))
            triangles.append(Int32(data.triangles[i].v2))
        }
        let geometryElements = [SCNGeometryElement(indices: triangles, primitiveType: .triangles)]
//        let geometryElements = [SCNGeometryElement(
//            data: Data(bytes: UnsafeRawPointer(triangles), count: triangles.count * MemoryLayout<Int32>.size),
//            primitiveType: .triangles,
//            primitiveCount: verticesCount,
//            bytesPerIndex: MemoryLayout<Int32>.size)]

//        if !data.hasNormals {
//            mesh.recalculateNormals()
//        }
//        mesh.recalculateBounds()
        return SCNGeometry(sources: geometrySources, elements: geometryElements)
    }

    static func getBytes<T>(value: T) -> [UInt8] {
        var value = value
        return withUnsafeBytes(of: &value) { Array($0) }
    }
    
    func niAVObjectPropertiesToMaterialProperties(_ obj: NiAVObject) -> MaterialProps {
        // Find relevant properties.
        var texturingProperty: NiTexturingProperty? = nil
        //var materialProperty: NiMaterialProperty? = nil
        var alphaProperty: NiAlphaProperty? = nil
        for propRef in obj.properties {
            let prop = _file.blocks[Int(propRef.value)]
            if prop is NiTexturingProperty { texturingProperty = prop as? NiTexturingProperty }
            //else if prop is NiMaterialProperty { materialProperty = prop as! NiMaterialProperty }
            else if prop is NiAlphaProperty { alphaProperty = prop as? NiAlphaProperty }
        }

        // Create the material properties.
        var mp = MaterialProps()
        if alphaProperty != nil {
            let flags = UInt(alphaProperty!.flags)
            let srcbm = NifObjectBuilder.getBytes(value: flags >> 1)[0] & 15
            let dstbm = NifObjectBuilder.getBytes(value: flags >> 5)[0] & 15
            mp.zwrite = NifObjectBuilder.getBytes(value: flags >> 15)[0] == 1 // smush
            if Utils.containsBitFlags(flags, 0x01) { // if flags contain the alpha blend flag at bit 0 in byte 0
                mp.alphaBlended = true
                mp.srcBlendMode = figureBlendMode(srcbm)
                mp.dstBlendMode = figureBlendMode(dstbm)
            }
            else if Utils.containsBitFlags(flags, 0x100) { // if flags contain the alpha test flag
                mp.alphaTest = true
                mp.alphaCutoff = Float(alphaProperty!.threshold) / 255
            }
        }
        else {
            mp.alphaBlended = false
            mp.alphaTest = false
        }
        // Apply textures.
        if texturingProperty != nil { mp.textures = configureTextureProperties(texturingProperty!) }
        return mp
    }

    func configureTextureProperties(_ ntp: NiTexturingProperty) -> MaterialTextures {
        var tp = MaterialTextures()
        if ntp.textureCount < 1 { return tp }
        if ntp.baseTexture != nil {
            let src = _file.blocks[Int(ntp.baseTexture!.source.value)] as! NiSourceTexture; tp.mainFilePath = src.fileName }
        if ntp.darkTexture != nil {
            let src = _file.blocks[Int(ntp.darkTexture!.source.value)] as! NiSourceTexture; tp.darkFilePath = src.fileName }
        if ntp.detailTexture != nil {
            let src = _file.blocks[Int(ntp.detailTexture!.source.value)] as! NiSourceTexture; tp.detailFilePath = src.fileName }
        if ntp.glossTexture != nil {
            let src = _file.blocks[Int(ntp.glossTexture!.source.value)] as! NiSourceTexture; tp.glossFilePath = src.fileName }
        if ntp.glowTexture != nil {
            let src = _file.blocks[Int(ntp.glowTexture!.source.value)] as! NiSourceTexture; tp.glowFilePath = src.fileName }
        if ntp.bumpMapTexture != nil {
            let src = _file.blocks[Int(ntp.bumpMapTexture!.source.value)] as! NiSourceTexture; tp.bumpFilePath = src.fileName }
        return tp
    }

    func figureBlendMode(_ b: UInt8) -> BlendMode {
        return BlendMode(rawValue: min(b, 10))!
    }

    func figureTestMode(_ b: UInt8) -> MatTestMode {
        return MatTestMode(rawValue: min(b, 7))!
    }

    func addColliderFromNiObject(_ niObject: NiObject, _ gameObject: GameObject) {
        if niObject is NiTriShape {
//            var colliderObj = instantiateNiTriShape(niObject as! NiTriShape, false, true)
//            colliderObj.transform.SetParent(gameObject.transform, false)
        }
        else if niObject is AvoidNode { }
        else { debugPrint("Unsupported collider NiObject: \(type(of: niObject))") }
    }

    static func isMarkerFileName(_ name: String) -> Bool {
        let lowerName = name.lowercased()
        return lowerName == "marker_light" ||
                lowerName == "marker_north" ||
                lowerName == "marker_error" ||
                lowerName == "marker_arrow" ||
                lowerName == "editormarker" ||
                lowerName == "marker_creature" ||
                lowerName == "marker_travel" ||
                lowerName == "marker_temple" ||
                lowerName == "marker_prison" ||
                lowerName == "marker_radius" ||
                lowerName == "marker_divine" ||
                lowerName == "editormarker_box_01"
    }
}
