using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed
{
    public class AudioController : MonoBehaviour
    {
        [SerializeField, Min(1)] private int _audioSourceCount = 3;
        [SerializeField] private Transform _audioSourceParent;
        private AudioSource[] _audioSources;
        private int _currentSource = 0;

        private void Awake()
        {
            _audioSources = new AudioSource[_audioSourceCount];

            for (int i = 0; i < _audioSourceCount; i++)
            {
                var sourceObject = new GameObject("Audio Source " + i);
                sourceObject.transform.parent = _audioSourceParent;
                sourceObject.transform.localPosition = Vector3.zero;

                _audioSources[i] = sourceObject.AddComponent<AudioSource>();
                _audioSources[i].spatialBlend = 1f;
                _audioSources[i].rolloffMode = AudioRolloffMode.Linear;
                _audioSources[i].minDistance = 0f;
                _audioSources[i].maxDistance = 10f;
            }
        }

        public void Play(AudioClip clip)
        {
            _audioSources[_currentSource].Stop();
            _audioSources[_currentSource].clip = clip;
            _audioSources[_currentSource].Play();

            _currentSource = (_currentSource + 1) % _audioSourceCount;
        }
    }
}
