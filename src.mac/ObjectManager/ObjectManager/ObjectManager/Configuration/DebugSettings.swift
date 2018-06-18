//
//  DebugSettings.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class DebugSettings: ASettingsSection {
    required public init() {
    }
    
    private var _logPackets = false
    public var logPackets: Bool {
        get { return _logPackets }
        set(value) { setProperty(&_logPackets, value) }
    }
    private var _showFps = true
    public var showFps: Bool {
        get { return _showFps }
        set(value) { setProperty(&_showFps, value) }
    }
    private var _isConsoleEnabled = true
    public var isConsoleEnabled: Bool {
        get { return _isConsoleEnabled }
        set(value) { setProperty(&_isConsoleEnabled, value) }
    }
}
