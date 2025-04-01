using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.BulletSystem.Damage
{
    public class DamagableManager : IDamagableManager
    {
        private List<IDamagable> _damagables = new List<IDamagable>();

        public List<IDamagable> Damagables { get => _damagables; }

        private CompositeDisposable _disposables = new CompositeDisposable();

        public void AddDamagable(IDamagable damagable)
        {
            damagable
                .OnDead
                .Subscribe(_ => Dead(_))
                .AddTo(_disposables);

            _damagables.Add(damagable);

            damagable.IsVulnerable = true;
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

        private void Dead(IDamagable damagable)
        {
            _damagables.Remove(damagable);

            damagable
                .OnDead
                .Dispose();

            damagable.IsVulnerable = false;
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
        public float[] GetAllEnemiesRanges();
        public Vector3[] GetAllEnemiesPosition();
    }
}