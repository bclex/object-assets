//
//  BsaMultiFile+Extensions.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

extension BsaMultiFile {
    public func loadTextureInfoAsync(texturePath: String, cb: @escaping (Texture2DInfo?) -> Void)  {
        guard let filePath = findTexture(texturePath) else {
            debugPrint("Could not find file '\(texturePath)' in a BSA file.")
            cb(nil)
            return
        }
        let fileData = loadFileData(filePath)
        DispatchQueue.global().async {
            let fileExtension = URL(string: filePath)!.pathExtension
            guard fileExtension.lowercased() == ".dds" else {
                fatalError("Unsupported texture type: \(fileExtension)")
            }
            let texture = DdsReader.loadDDSTexture(DataBaseStream(data: fileData))
            DispatchQueue.main.async {
                cb(texture)
            }
        }
    }

    public func loadObjectInfoAsync(filePath: String, cb: @escaping (Any) -> Void) {
        let fileUrl = URL(string: filePath)!
        let fileData = loadFileData(filePath)
        DispatchQueue.global().async {
            var file = NiFile(name: fileUrl.deletingPathExtension().lastPathComponent)
            //file.Deserialize(BinaryReader(DataBaseStream(data: fileData)))
            DispatchQueue.main.async {
                cb(file)
            }
        }
    }
    
    func findTexture(_ texturePath: String) -> String? {
        let textureUrl = URL(string: texturePath)!
        let textureName = textureUrl.deletingPathExtension().lastPathComponent
        let textureNameInTexturesDir = "textures/\(textureName)"
        var filePath = "\(textureNameInTexturesDir).dds"
        if containsFile(filePath) {
            return filePath
        }
        let texturePathWithoutExtension = "\(textureUrl.deletingLastPathComponent())\\\(textureName)"
        filePath = "\(texturePathWithoutExtension).dds"
        if containsFile(filePath) {
            return filePath
        }
        return nil
    }
}
