//
//  Task.swift
//  ObjectManager
//
//  Created by Sky Morey on 6/5/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

// https://stackoverflow.com/questions/42484281/waiting-until-the-task-finishes/42484670
public class Task<T> {
    var value: T? = nil
    var group: DispatchGroup? = nil

    init() {
        group = DispatchGroup()
        group!.enter()
    }
    init(value: T) {
        self.value = value
    }

    public var result: T {
        if group != nil {
            group!.wait()
        }
        return value!
    }

    public func callback(_ value: T) {
        self.value = value
        group!.leave()
        group = nil
    }

    public func wait() {
        if group != nil {
            group!.wait()
        }
    }
}
