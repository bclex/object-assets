//
//  ASettingsSection.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class ASettingsSection {
    public required init() {
    }
    
    @discardableResult
    public func setProperty<T>(_ storage: inout T, _ value: T) -> Bool {
        return true
    }
}
