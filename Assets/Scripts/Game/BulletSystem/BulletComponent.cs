using System;
using UniRx;
using UnityEngine;

namespace Game.BulletSystem
{
    [RequireComponent(typeof(ParticleSystem))]
    public class BulletComponent : MonoBehaviour, IBulletComponent
    {
        [Header("ParticleSystem")]
        [SerializeField] private ParticleSystem _particleSystem = null;

        [Space(10)]
        [Header("FollowPlayer")]
        [SerializeField] private bool _lookAtPlayer = false;

        private Transform _playerTransform = null;

        public void Init(Transform player)
        {
            _playerTransform = player;
            StartBulletComponent();
        }

        private void StartBulletComponent()
        {
            if (_particleSystem == null)
            {
                _particleSystem = GetComponent<ParticleSystem>();
            }
            _particleSystem.Play();
        }
        public void UpdateComponent(float fixedDelta)
        {
            if (_lookAtPlayer)
            {
                Vector3 lookAtPosition = _playerTransform.position - transform.position;
                transform.rotation = Quaternion.Euler(0f, lookAtPosition.y, 0f);
            }
        }
    }

    public interface IBulletComponent
    {
        void Init(Transform player);
        void UpdateComponent(float fixedDelta);
    }
}
