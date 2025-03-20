using System;
using System.Collections.Generic;
using UnityEngine;

namespace Audio.Types
{
    public class BaseAudioUtility<T> : MonoBehaviour where T : Enum
    {
        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private AudioStorage<T>[] _audioStorages = null;

        private Dictionary<string, AudioClip> _stringForAudio = new Dictionary<string, AudioClip>();

        internal float Volume { get => _audioSource.volume; set => _audioSource.volume = value; }

        internal virtual void Init()
        {
            foreach(AudioStorage<T> audioStorage in _audioStorages)
            {
                string name = audioStorage.GetType.GetType().ToString();
                _stringForAudio.Add(name, audioStorage.GetAudioClip);
            }
        }

        internal virtual void Play(T soundName)
        {
            AudioClip clip = null;
            string name = soundName.GetType().ToString();
            _stringForAudio.TryGetValue(name, out clip);
            _audioSource.clip = clip;
            _audioSource.PlayOneShot(clip);
        }

        internal virtual void Pause()
        {
            _audioSource.Pause();
        }
    }
}