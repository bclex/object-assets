//
//  SettingsFile.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public class SettingsFile {
    var _sectionCache = [String : ASettingsSection]()
    let _filename: String
    var _timer: Timer? = nil

    public init(filename: String) {
        _filename = filename
        _timer = Timer(timeInterval: TimeInterval(10000), repeats: true, block: onTimerElapsed)
    }

    public var exists: Bool { return FileManager.default.fileExists(atPath: _filename) }

    public func save() {
    }
    
    func invalidateDirty() {
    }

    func onTimerElapsed(_ timer: Timer) {
        save()
    }
    
    func createOrOpenSection<T>(_ sectionName: String) -> T where T: ASettingsSection {
        if let section = _sectionCache[sectionName] {
            return section as! T
        }
        // Load & cache the section
        let section = T()
        invalidateDirty()
        _sectionCache[sectionName] = section
        return section
    }
}

