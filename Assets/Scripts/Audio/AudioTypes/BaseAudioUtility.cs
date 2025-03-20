using System;
using UnityEngine;

namespace Audio.Types
{
    public class BaseAudioUtility<T> : MonoBehaviour where T : Enum
    {
        [SerializeField] private AudioStorage<T>[] audioStorages = null;

        public virtual void Init()
        {

        }

        public virtual void Play(T soundName)
        {

        }
    }
}