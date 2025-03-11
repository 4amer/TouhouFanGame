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
            _particleSystem = GetComponent<ParticleSystem>();
            _playerTransform = player;
        }

        public void StartShooting()
        {
            StartBulletComponent();
        }

        public void StopShooting()
        {
            StopBulletComponent();
        }

        private void StartBulletComponent()
        {
            if (_particleSystem == null)
            {
                _particleSystem = GetComponent<ParticleSystem>();
            }
            _particleSystem.Play();
        }

        private void StopBulletComponent()
        {
            if (_particleSystem == null)
            {
                _particleSystem = GetComponent<ParticleSystem>();
            }
            _particleSystem.Stop();
        }

        public void UpdateComponent(float delta)
        {
            if (_lookAtPlayer)
            {
                Vector3 directionToPlayer = _playerTransform.position - transform.position;

                float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
            }
        }
    }

    public interface IBulletComponent
    {
        void Init(Transform player);
        void StartShooting();
        void StopShooting();
        void UpdateComponent(float delta);
    }
}
