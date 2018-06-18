//
//  GameSettings.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class GameSettings: ASettingsSection {
    // GLOBAL
    private var _gameId: String = ""
    public var gameId: String {
        get { return _gameId }
        set(value) { setProperty(&_gameId, value) }
    }
    private var _dataDirectory: String? = nil
    public var dataDirectory: String? {
        get { return _dataDirectory }
        set(value) { setProperty(&_dataDirectory, value) }
    }
    private var _kinematicRigidbodies = true
    public var kinematicRigidbodies: Bool {
        get { return _kinematicRigidbodies }
        set(value) { setProperty(&_kinematicRigidbodies, value) }
    }
    private var _playMusic = false
    public var playMusic: Bool {
        get { return _playMusic }
        set(value) { setProperty(&_playMusic, value) }
    }
    private var _enableLog = false
    public var enableLog: Bool {
        get { return _enableLog }
        set(value) { setProperty(&_enableLog, value) }
    }

    // RENDERING
    private var _materialType: MaterialType = .standard
    public var materialType: MaterialType {
        get { return _materialType }
        set(value) { setProperty(&_materialType, value) }
    }
//    private var _renderPath: RenderingPath = .forward;
//    public var renderPath: RenderingPath {
//        get { return _renderPath }
//        set(value) { setProperty(&_renderPath, value) }
//    }
    private var _cameraFarClip: Float = 500.0
    public var cameraFarClip: Float {
        get { return _cameraFarClip }
        set(value) { setProperty(&_cameraFarClip, value) }
    }
    private var _waterBackSideTransparent = false
    public var waterBackSideTransparent: Bool
    {
        get { return _waterBackSideTransparent }
        set(value) { setProperty(&_waterBackSideTransparent, value) }
    }

    // LIGHTING
    private var _ambientIntensity: Float = 1.5
    public var ambientIntensity: Float {
        get { return _ambientIntensity }
        set(value) { setProperty(&_ambientIntensity, value) }
    }
    private var _renderSunShadows = false
    public var renderSunShadows: Bool {
        get { return _renderSunShadows }
        set(value) { setProperty(&_renderSunShadows, value) }
    }
    private var _renderLightShadows = false
    public var renderLightShadows: Bool {
        get { return _renderLightShadows }
        set(value) { setProperty(&_renderLightShadows, value) }
    }
    private var _renderExteriorCellLights = false
    public var renderExteriorCellLights: Bool {
        get { return _renderExteriorCellLights }
        set(value) { setProperty(&_renderExteriorCellLights, value) }
    }
    private var _animateLights = false
    public var animateLights: Bool {
        get { return _animateLights }
        set(value) { setProperty(&_animateLights, value) }
    }
    private var _dayNightCycle = false
    public var dayNightCycle: Bool {
        get { return _dayNightCycle }
        set(value) { setProperty(&_dayNightCycle, value) }
    }
    private var _generateNormalMap = true
    public var generateNormalMap: Bool {
        get { return _generateNormalMap }
        set(value) { setProperty(&_generateNormalMap, value) }
    }
    private var _normalGeneratorIntensity: Float = 0.75
    public var normalGeneratorIntensity: Float {
        get { return _normalGeneratorIntensity }
        set(value) { setProperty(&_normalGeneratorIntensity, value) }
    }

    // EFFECTS
    // UI
    // PREFABS

    // DEBUG
    private var _creaturesEnabled = false
    public var creaturesEnabled: Bool {
        get { return _creaturesEnabled }
        set(value) { setProperty(&_creaturesEnabled, value) }
    }
    private var _npcsEnabled = false
    public var npcsEnabled: Bool {
        get { return _npcsEnabled }
        set(value) { setProperty(&_npcsEnabled, value) }
    }
}
