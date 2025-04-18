using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Audio.Types
{
    public class BaseAudioUtility<T> : MonoBehaviour where T : Enum
    {
        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private AudioStorage<T>[] _audioStorages = null;

        private Dictionary<T, AudioClip> _stringForAudio = new Dictionary<T, AudioClip>();

        internal float Volume { get => _audioSource.volume; set => _audioSource.volume = value; }

        internal virtual void Init()
        {
            foreach(AudioStorage<T> audioStorage in _audioStorages)
            {
                T name = audioStorage.GetSoundType;
                _stringForAudio.Add(name, audioStorage.GetAudioClip);
            }
        }

        internal virtual void Play(T soundName)
        {
            _audioSource.loop = false;
            AudioClip clip = FindAudioClip(soundName);
            _audioSource.clip = clip;
            _audioSource.PlayOneShot(clip);
        }

        internal virtual void PlayLoop(T soundName)
        {
            AudioClip clip = FindAudioClip(soundName);
            _audioSource.clip = clip;
            _audioSource.loop = true;
            _audioSource.Play();
        }

        internal virtual void Pause()
        {
            _audioSource.Pause();
        }

        internal virtual void Stop()
        {
            _audioSource.Stop();
        }

        private AudioClip FindAudioClip(T cliptoplay)
        {
            AudioClip clip = null;
            _stringForAudio.TryGetValue(cliptoplay, out clip);
            return clip;
        }
    }
}