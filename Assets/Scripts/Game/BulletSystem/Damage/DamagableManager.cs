using System.Collections.Generic;
using Audio;
using Audio.Types;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Game.BulletSystem.Damage
{
    public class DamagableManager : IDamagableManager
    {
        private List<IDamagable> _damagables = new List<IDamagable>();

        public List<IDamagable> Damagables { get => _damagables; }

        private CompositeDisposable _disposables = new CompositeDisposable();

        [Inject] private IAudioManager _audioManager = null;

        private Utils.Timer _sfxTimer = null;

        public void AddDamagable(IDamagable damagable)
        {
            damagable
                .OnDead
                .Subscribe(_ => Dead(_))
                .AddTo(_disposables);

            damagable
                .OnDamaged
                .Subscribe(_ => PlayDamageSFX())
                .AddTo(_disposables);

            _damagables.Add(damagable);
        }

        public Vector3[] GetAllEnemiesPosition()
        {
            Vector3[] positions = new Vector3[_damagables.Count];
            for(int i = 0; i < _damagables.Count; i++)
            {
                positions[i] = _damagables[i].Transform.position;
            }
            return positions;
        }

        public float[] GetAllEnemiesRanges()
        {
            float[] ranges = new float[_damagables.Count];
            for (int i = 0; i < _damagables.Count; i++)
            {
                ranges[i] = _damagables[i].RangeToCollide;
            }
            return ranges;
        }

        public IDamagable[] GetAllDamagables()
        {
            return _damagables.ToArray();
        }

        public void RemoveDamagable(IDamagable damagable)
        {
            Dead(damagable);
        }

        private void Dead(IDamagable damagable)
        {
            if(_damagables.Contains(damagable) == false) return;
            
            _damagables.Remove(damagable);

            damagable
                .OnDead
                .Dispose();

            _audioManager.Play(ESFXTypes.EnemyDeath);

            damagable.IsVulnerable = false;
        }

        private void PlayDamageSFX()
        {
            /*if(_sfxTimer == null)
            {
                _sfxTimer = new Utils.Timer();
                _sfxTimer.duration = 0.1f;
                _sfxTimer.EventOnFinish += () => _audioManager.Play(ESFXTypes.EnemyDamaged);
            }

            if (_sfxTimer.IsPlaying) return;

            _sfxTimer.Start();*/
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }

    internal interface IDamagableManager
    {
        public List<IDamagable> Damagables { get; }
        public void AddDamagable(IDamagable damagable);
        public void RemoveDamagable(IDamagable damagable);
        public float[] GetAllEnemiesRanges();
        public Vector3[] GetAllEnemiesPosition();
        public IDamagable[] GetAllDamagables();
    }
}