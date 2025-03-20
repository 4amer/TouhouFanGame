using System;
using UnityEngine;

namespace Audio.Types
{
    [Serializable]
    public class AudioStorage<T> where T : Enum
    {
        [SerializeField] private AudioClip _audioClip;
        [SerializeField] private T _soundType;

        public AudioClip GetAudioClip => _audioClip;
        public T GetType => _soundType;
    }
}
