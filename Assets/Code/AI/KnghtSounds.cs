using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.AI
{
    public class KnghtSounds : EntitySounds
    {
        [SerializeField] private AudioClip _suspicionClip;
        private SuspicionController _suspicion;

        protected override void Awake()
        {
            base.Awake();

            _suspicion = GetComponent<SuspicionController>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _suspicion.OnSuspect += _OnSuspect;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _suspicion.OnSuspect -= _OnSuspect;
        }

        private void _OnSuspect()
        {
            _audio.Play(_suspicionClip);
        }
    }
}
