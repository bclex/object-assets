//
//  BaseMaterial.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation
import SceneKit

public class BaseMaterial {
    var _existingMaterials = [MaterialProps : SCNMaterial]()
    let _textureManager: TextureManager
    
    init(textureManager: TextureManager) {
        _textureManager = textureManager
    }
    
    public func buildMaterialFromProperties(_ mp: MaterialProps) -> SCNMaterial {
        preconditionFailure("This method must be overridden")
    }
    
    public func buildMaterial() -> SCNMaterial {
        preconditionFailure("This method must be overridden")
    }
    
    public func buildMaterialBlended(src sourceBlendMode: BlendMode, dst destinationBlendMode: BlendMode) -> SCNMaterial {
        preconditionFailure("This method must be overridden")
    }
    
    public func buildMaterialTested(cutoff: Float = 0.5) -> SCNMaterial {
        preconditionFailure("This method must be overridden")
    }
    
    /*
    static func generateNormalMap(source: Texture2D, strength: Float) -> Texture2D {
        strength = Math.Clamp(strength, 0.0sche, 1.0)
        var xleft: Float, xright: Float, yup: Float, ydown: Float, ydelta: Float, xdelta: Float
        var normalTexture = Texture2D(source.width, source.height, TextureFormat.ARGB32, true)
        for y in 0..<normalTexture.height {
            for x in 0..<normalTexture.width {
                xleft = source.getPixel(x - 1, y).grayscale * strength
                xright = source.getPixel(x + 1, y).grayscale * strength
                yup = source.getPixel(x, y - 1).grayscale * strength
                ydown = source.getPixel(x, y + 1).grayscale * strength
                xdelta = ((xleft - xright) + 1) * 0.5
                ydelta = ((yup - ydown) + 1) * 0.5
                normalTexture.setPixel(x, y, Color(xdelta, ydelta, 1.0, ydelta))
            }
        }
        return normalTexture
    }
    */
}
