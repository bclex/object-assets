//
//  TemporalLoadBalancer.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public struct CoTask: Equatable {
    public static func ==(lhs: CoTask, rhs: CoTask) -> Bool {
        return lhs.id == rhs.id
    }
    
    private static var seed: Int = 0
    public let id: Int
    public let co: AnyIterator<Any>
    
    init(_ co: AnyIterator<Any>) {
        CoTask.seed = CoTask.seed &+ 1
        id = CoTask.seed
        self.co = co
    }
}

public class TemporalLoadBalancer {
    var _tasks = [CoTask]()

    public func addTask(taskCoroutine: CoTask) -> CoTask {
        _tasks.append(taskCoroutine)
        return taskCoroutine
    }

    public func cancelTask(taskCoroutine: CoTask) {
        guard let idx = _tasks.index(of: taskCoroutine) else {
            return
        }
        _tasks.remove(at: idx)
    }

    public func runTasks(desiredWorkTime: Double) {
        assert(desiredWorkTime >= 0, "desiredWorkTime must be greater than 0")
        guard _tasks.count != 0 else {
            return
        }
        let start = Date()
        // Run the tasks.
        repeat {
            if _tasks.first!.co.next() == nil { // Try to execute an iteration of a task. Remove the task if it's execution has completed.
                _tasks.removeFirst()
            }
        } while _tasks.count > 0 && Date().timeIntervalSince(start) < desiredWorkTime
    }

    public func waitForTask(taskCoroutine: CoTask) {
        //assert(_tasks.contains{$0 == taskCoroutine})
        while taskCoroutine.co.next() != nil { }
        guard let idx = _tasks.index(of: taskCoroutine) else {
            return
        }
        _tasks.remove(at: idx)
    }

    public func waitForAllTasks() {
        for task in _tasks {
            while task.co.next() != nil { }
        }
        _tasks.removeAll()
    }
}
