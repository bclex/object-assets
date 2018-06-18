//
//  BaseSettings.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class BaseSettings {
    let _debug: DebugSettings
    let _game: GameSettings
    
    init() {
        _debug = BaseSettings.createOrOpenSection()
        _game = BaseSettings.createOrOpenSection()
    }

    public static var debug: DebugSettings { return _instance._debug }
    public static var game: GameSettings { return _instance._game }
    
    static let _instance = BaseSettings()
    static let _file = SettingsFile(filename: "settings.cfg")

    public static func save() {
        _file.save()
    }

    public static func createOrOpenSection<T>() -> T where T: ASettingsSection {
        let sectionName = "\(T.self)"
        let section: T = _file.createOrOpenSection(sectionName)
        return section 
    }
}
