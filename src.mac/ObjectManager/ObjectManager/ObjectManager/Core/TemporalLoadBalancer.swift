//
//  TemporalLoadBalancer.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public class TemporalLoadBalancer {
    var _tasks = [AnyIterator<Any>]()
    var _start = NSDate()

    public func addTask(taskCoroutine: AnyIterator<Any>) {
        _tasks.append(taskCoroutine)
        return taskCoroutine
    }

    public func cancelTask(taskCoroutine: AnyIterator<Any>) {
        _tasks.remove(taskCoroutine)
    }

    public func runTasks(desiredWorkTime: Float) {
        assert(desiredWorkTime >= 0, "desiredWorkTime must be greater than 0")
        guard _tasks.count != 0 else {
            return
        }
        _start = NSDate()
        // Run the tasks.
        repeat {
            if _tasks.first!.next() == nil { // Try to execute an iteration of a task. Remove the task if it's execution has completed.
                _tasks.removeFirst()
            }
        } while _tasks.count > 0 && NSDate().timeIntervalSinceDate(start) < desiredWorkTime;
    }

    public func waitForTask(taskCoroutine: AnyIterator<Any>) {
        //assert(_tasks.contains{$0 == taskCoroutine})
        while taskCoroutine.next() != nil { }
        _tasks.Remove(taskCoroutine)
    }

    public func waitForAllTasks() {
        for task in _tasks {
            while task.next() != nil { }
        }
        _tasks.clear()
    }
}
