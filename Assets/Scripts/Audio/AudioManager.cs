using System;
using System.Collections.Generic;
using Audio.Types;
using Audio.Types.Music;
using Audio.Types.SFX;
using Audio.Types.UI;
using Unity.VisualScripting;
using UnityEngine;

namespace Audio
{
    public class AudioManager : MonoBehaviour, IAudioManagerInit, IAudioManager
    {
        [SerializeField] private UIAudioUtility _UIAudioUtility = null;
        [SerializeField] private SFXUtility _SFXUtility = null;
        [SerializeField] private MusicUtility _MusicUtility = null;

        [Range(0, 1)]
        [SerializeField] private float _baseVolume = 0.5f;

        private Dictionary<string, object> keyValueAudio = new Dictionary<string, object>(); 

        public void Init()
        {
            _UIAudioUtility.Init();
            _SFXUtility.Init();
            _MusicUtility.Init();

            _UIAudioUtility.Volume = _baseVolume;
            _SFXUtility.Volume = _baseVolume;
            _MusicUtility.Volume = _baseVolume;

            keyValueAudio.Add(_UIAudioUtility.GetType().BaseType.ToString(), _UIAudioUtility);
            keyValueAudio.Add(_SFXUtility.GetType().BaseType.ToString(), _SFXUtility);
            keyValueAudio.Add(_MusicUtility.GetType().BaseType.ToString(), _MusicUtility);
        }

        public void Play<E>(E audioType) where E : Enum
        {
            BaseAudioUtility<E> baseAudio = GetBaseAudio(audioType);
            baseAudio.Play(audioType);
        }

        public void PlayLoop<E>(E audioType) where E : Enum
        {
            BaseAudioUtility<E> baseAudio = GetBaseAudio(audioType);
            baseAudio.PlayLoop(audioType);
        }

        public void Pause<E>(E audioType) where E : Enum
        {
            BaseAudioUtility<E> baseAudio = GetBaseAudio(audioType);
            baseAudio.Pause();
        }

        public void Stop<E>(E audioType) where E : Enum
        {
            BaseAudioUtility<E> baseAudio = GetBaseAudio(audioType);
            baseAudio.Stop();
        }

        private BaseAudioUtility<E> GetBaseAudio<E>(E audioType) where E : Enum
        {
            string audioName = typeof(BaseAudioUtility<E>).ToString();
            return keyValueAudio[audioName] as BaseAudioUtility<E>;
        }
    }

    internal interface IAudioManager
    {
        public void Play<E>(E audioType) where E : Enum;
        public void PlayLoop<E>(E audioType) where E : Enum;
        public void Pause<E>(E audioType) where E : Enum;
        public void Stop<E>(E audioType) where E : Enum;
    }

    internal interface IAudioManagerInit
    {
        public void Init();
    }
}
