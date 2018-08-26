//
//  BsaMultiFile+Extensions.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

extension BsaMultiFile {
    public func loadTextureInfoAsync(texturePath: String) -> Task<Texture2DInfo?> {
        guard let filePath = findTexture(texturePath) else {
            debugPrint("Could not find file '\(texturePath)' in a BSA file.")
            return Task(value: nil)
        }
        let fileData = loadFileData(filePath)
        var task = Task<Texture2DInfo?>()
        DispatchQueue.global().async {
            debugPrint("\(filePath)")
            let fileExtension = URL(string: filePath)!.pathExtension
            guard fileExtension.lowercased() == "dds" else {
                fatalError("Unsupported texture type: \(fileExtension)")
            }
            let r = BinaryReader(DataBaseStream(data: fileData))
            defer { r.close() }
            let texture = DdsReader.loadDDSTexture(r)
            task.callback(texture)
        }
        return task
    }

    public func loadObjectInfoAsync(filePath: String) -> Task<Any> {
        
        let fileData = loadFileData(filePath)
        var task = Task<Any>()
        DispatchQueue.global().async {
            debugPrint("\(filePath)")
            let fileUrl = URL(string: filePath.replacingOccurrences(of: "\\", with: "/"))!
            let r = BinaryReader(DataBaseStream(data: fileData))
            defer { r.close() }
            let file = NiFile(r, name: fileUrl.deletingPathExtension().lastPathComponent)
            task.callback(file)
        }
        return task
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
