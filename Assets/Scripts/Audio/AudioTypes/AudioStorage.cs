using System;
using UnityEngine;

namespace Audio.Types
{
    [Serializable]
    public class AudioStorage<T> where T : Enum
    {
        [SerializeField] private AudioClip audioClip;
        [SerializeField] private T _soundType;
    }
}
