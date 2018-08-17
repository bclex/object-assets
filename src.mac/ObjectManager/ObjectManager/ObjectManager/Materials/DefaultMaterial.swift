//
//  DefaultMaterial.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation
import SceneKit
import AppKit

public class DefaultMaterial: BaseMaterial {
    public override func buildMaterialFromProperties(_ mp: MaterialProps) -> SCNMaterial {
        if let material = _existingMaterials[mp] {
            return material
        }
        // Load & cache the material.
        var material: SCNMaterial
        if mp.alphaBlended { material = buildMaterialBlended(src: mp.srcBlendMode, dst: mp.dstBlendMode) }
        else if mp.alphaTest { material = buildMaterialTested(cutoff: mp.alphaCutoff) }
        else { material = buildMaterial() }
        if mp.textures.mainFilePath != nil {
            material.diffuse.contents = _textureManager.loadTexture(mp.textures.mainFilePath!)
//            material.diffuse.contents = NSColor.red // _textureManager.loadTexture(mp.textures.mainFilePath!)
//            material.specular.contents = NSColor.white
        }
        if mp.textures.bumpFilePath != nil {
            material.normal.contents = _textureManager.loadTexture(mp.textures.bumpFilePath!)
        }
        _existingMaterials[mp] = material
        return material
    }
    
    public override func buildMaterial() -> SCNMaterial {
        return SCNMaterial()
    }
    
    public override func buildMaterialBlended(src sourceBlendMode: BlendMode, dst destinationBlendMode: BlendMode) -> SCNMaterial {
        return SCNMaterial()
    }
    
    public override func buildMaterialTested(cutoff: Float = 0.5) -> SCNMaterial {
        return SCNMaterial()
    }
}
