/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

namespace DeveloperTech.XRProfilingToolkit.Automation
{
    /// <summary>
    /// Singleton class that runs the automation commands in a command queue in sequence
    /// </summary>
    public class CommandRunner : MonoBehaviour
    {
        /// <summary>
        /// The status of the command runner
        /// </summary>
        public enum Status
        {
            /// <summary>
            /// The command runner is idling.
            /// </summary>
            Idle,
            
            /// <summary>
            /// The command runner is running commands.
            /// </summary>
            Running,
            
            /// <summary>
            /// The command running is paused.
            /// </summary>
            Paused,
        }

        private CommandBase _currentCommand;
        private readonly Queue<CommandBase> _commandQueue = new Queue<CommandBase>();

        private Coroutine _commandCoroutine;

        private bool _paused = false;

        private static CommandRunner _instance;

        private string _id;

        private float _stopTime;

        public Action<Status> OnStatusChanged;

        public bool IsRunning => _currentCommand != null && !_paused;

        /// <summary>
        /// Whether it is currently in a profiling session.
        /// </summary>
        public bool IsProfilingSession => SerializedCommandQueue != null;

        private bool _commandQueueIsOnDisk;
        private CommandQueue _serializedCommandQueue;

        private CommandQueue SerializedCommandQueue
        {
            get
            {
                if (_serializedCommandQueue == null)
                {
                    _serializedCommandQueue = ScriptableObject.CreateInstance<CommandQueue>();
                    _commandQueueIsOnDisk = CommandSerializer.DeserializeCommandQueue(ref _serializedCommandQueue);
                }

                return _commandQueueIsOnDisk ? _serializedCommandQueue : null;
            }
        }

        /// <summary>
        /// Load the automation commands defined in the <see cref="CommandQueue"/>
        /// </summary>
        public void Load(CommandQueue cq)
        {
            Clear();
            _id = cq.id;
            _stopTime = cq.stopTime;
            foreach (var command in cq.commands)
            {
                _commandQueue.Enqueue(command);
            }
        }

        /// <summary>
        /// Clears the command queue.
        /// </summary>
        public void Clear()
        {
            _commandQueue.Clear();
        }

        /// <summary>
        /// Runs the commands in the command queue.
        /// </summary>
        public void Run()
        {
            _paused = false;
            OnStatusChanged?.Invoke(Status.Running);
            _commandCoroutine = StartCoroutine(RunCommandsCoroutine());
        }

        /// <summary>
        /// Pauses the current command, if the current command can not be paused, will pause after the current one finishes.
        /// </summary>
        public void Pause()
        {
            _paused = true;
            OnStatusChanged?.Invoke(Status.Paused);
            _currentCommand?.Pause();
            XRProfilingToolkitLogger.Log("XRProfilingToolkit paused");
        }

        /// <summary>
        /// Resumes the paused command.
        /// </summary>
        public void Resume()
        {
            _paused = false;
            OnStatusChanged?.Invoke(Status.Running);
            _currentCommand?.Resume();
        }

        private IEnumerator RunCommandsCoroutine()
        {
            yield return new WaitForSeconds(1);
            XRProfilingToolkitLogger.Log($"XRProfilingToolkit starting, id: {_id}");
            if (_stopTime > 0f)
            {
                StartCoroutine(StopInSec(_stopTime));
            }
            while (_commandQueue.Count > 0)
            {
                while (_paused)
                {
                    yield return null;
                }
                _currentCommand = _commandQueue.Dequeue();
                yield return _currentCommand.Run();
            }
            _currentCommand = null;
            XRProfilingToolkitLogger.Log("XRProfilingToolkit finished");
            OnStatusChanged?.Invoke(Status.Idle);
        }

        private IEnumerator StopInSec(float stopTimeInSec) {
            XRProfilingToolkitLogger.Log($"Pausing in {stopTimeInSec} seconds");
            yield return new WaitForSeconds(stopTimeInSec);
            Pause();
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (SerializedCommandQueue == null)
            {
                OnStatusChanged?.Invoke(Status.Idle);
            }
            else
            {
                Load(SerializedCommandQueue);
                Run();
            }
        }

        private void OnDestroy()
        {
            // make sure won't be stuck in the middle of something, scene loading for example
            _currentCommand?.Resume();
            if (_commandCoroutine != null)
            {
                StopCoroutine(_commandCoroutine);
                OnStatusChanged?.Invoke(Status.Idle);
            }
            Clear();
            Destroy(_serializedCommandQueue);
        }
    }
}
