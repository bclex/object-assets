//
//  MaterialManager.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

//public enum MatTestMode { Always, Less, LEqual, Equal, GEqual, Greater, NotEqual, Never }

import SceneKit

public enum MaterialType {
    case standard
}

public enum BlendMode: UInt8 {
    case one
}

public enum MatTestMode: UInt8 {
    case one
}

public struct MaterialTextures {
    public var mainFilePath: String?
    public var darkFilePath: String?
    public var detailFilePath: String?
    public var glossFilePath: String?
    public var glowFilePath: String?
    public var bumpFilePath: String?
}

public struct MaterialProps: Hashable {
    public static func ==(lhs: MaterialProps, rhs: MaterialProps) -> Bool {
        return lhs.textures.mainFilePath == rhs.textures.mainFilePath
    }
    
    public var hashValue: Int { return 0 }
    public var textures: MaterialTextures!
    public var alphaBlended: Bool!
    public var srcBlendMode: BlendMode!
    public var dstBlendMode: BlendMode!
    public var alphaTest: Bool!
    public var alphaCutoff: Float!
    public var zwrite: Bool!
}

public class MaterialManager {
    var _material: BaseMaterial

    private(set) public var textureManager: TextureManager

    init(textureManager: TextureManager) {
        self.textureManager = textureManager
        let game = BaseSettings.game
        switch game.materialType {
        case .standard: _material = DefaultMaterial(textureManager: textureManager)
        }
    }
   
    public func buildMaterialFromProperties(_ mp: MaterialProps) -> SCNMaterial {
        return _material.buildMaterialFromProperties(mp)
    }

    func buildMaterial() -> SCNMaterial {
        return _material.buildMaterial()
    }

    func buildMaterialBlended(src sourceBlendMode: BlendMode, dst destinationBlendMode: BlendMode) -> SCNMaterial {
        return _material.buildMaterialBlended(src: sourceBlendMode, dst: destinationBlendMode)
    }

    func buildMaterialTested(cutoff: Float = 0.5) -> SCNMaterial {
        return _material.buildMaterialTested(cutoff: cutoff)
    }
}
