using UnityEngine;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _UIAudio = null;
        [SerializeField] private AudioSource _Sounds = null;
        [SerializeField] private AudioSource _Music = null;

        [SerializeField] private float _baseVolume = 0.5f;

        public void Init()
        {
            _UIAudio.volume = _baseVolume;
            _Sounds.volume = _baseVolume;
            _Music.volume = _baseVolume;
        }
    }
}
