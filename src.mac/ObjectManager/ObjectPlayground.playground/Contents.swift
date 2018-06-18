//: Playground - noun: a place where people can play

import Foundation
import ObjectManager

let assetUrl = URL(string: "game://Morrowind/Morrowind.bsa")
let dataUrl = URL(string: "game://Morrowind/Morrowind.esm")

let assetManager = AssetManager.getAssetManager(.tes)
let asset = assetManager.getAssetPack(assetUrl)
