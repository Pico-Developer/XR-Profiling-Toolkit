/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;
using UnityEngine.XR.Interaction.Toolkit;

namespace DeveloperTech.XRProfilingToolkit.Scene
{
    /// <summary>
    /// Dynamic particle system, can be switched between different particle modes
    /// </summary>
    public class DynamicParticle : DynamicObjectBase
    {
        private enum ParticleMode
        {
            None = 0,
            Particle = 1,
            VFX = 2, // GPU simulated particles
        }

        [SerializeField]
        private ParticleSystem _particleSystem;

        [SerializeField]
        private XRBaseInteractable _interactor;

        [SerializeField]
        private VisualEffect _vfx;

        [SerializeField]
        private InteractableHinter _hinter;

        static readonly ExposedProperty aliveAttribute = "Alive";

        private ParticleSystem[] _childSystems;

        private ParticleMode _mode;

        private ParticleMode CurrentMode
        {
            get => _mode;
            set
            {
                _mode = value;
                if (value == ParticleMode.Particle)
                {
                    if (_particleSystem != null)
                    {
                        _particleSystem.Play(true);
                    }
                    if (_vfx != null)
                    {
                        _vfx.Stop();
                        _vfx.SetBool(aliveAttribute, false);
                    }
                }
                else if (value == ParticleMode.VFX)
                {
                    if (_particleSystem != null)
                    {
                        _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    }
                    if (_vfx != null)
                    {
                        _vfx.Play();
                        _vfx.SetBool(aliveAttribute, true);
                    }
                }
                else
                {
                    if (_particleSystem != null)
                    {
                        _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    }
                    if (_vfx != null)
                    {
                        _vfx.Stop();
                        _vfx.SetBool(aliveAttribute, false);
                    }
                }
                if (_hinter != null)
                {
                    // get the next particle mode
                    var enumValArray = (ParticleMode[])Enum.GetValues(_mode.GetType());
                    int nextIndex = Array.IndexOf(enumValArray, _mode) + 1;
                    var nextMode = (enumValArray.Length == nextIndex) ? enumValArray[0] : enumValArray[nextIndex];

                    // hint the user for the next particle mode
                    _hinter.UpdateHintText(nextMode.ToString());
                }
            }
        }
        
        /// <summary>
        /// Total number of particles under current particle system, also includes particles in child system
        /// Which are the particle systems in the children game objects
        /// </summary>
        public override int Complexity
        {
            get
            {
                if (CurrentMode == ParticleMode.Particle && _particleSystem != null)
                {
                    int sum = 0;
                    sum += _particleSystem.particleCount;
                    foreach (var ps in _childSystems)
                    {
                        sum += ps.particleCount;
                    }
                    return sum;
                }
            
                if (CurrentMode == ParticleMode.VFX && _vfx != null)
                {
                    return _vfx.aliveParticleCount;
                }
            
                return 0;
            }
        }

        public override int Mode
        {
            get
            {
                return (int)CurrentMode;
            }
            set
            {
                var max = Enum.GetValues(typeof(ParticleMode)).Cast<int>().Max() + 1;
                CurrentMode = (ParticleMode) (value % max);
            }
        }

        private void Start()
        {
            _interactor.activated.AddListener(OnObjectInteracted);
            _childSystems = _particleSystem.GetComponentsInChildren<ParticleSystem>(true);
        }

        void OnObjectInteracted(ActivateEventArgs args)
        {
            DynamicSystem.ChangeModeWithCommand(Mode + 1);
        }
    }
}
