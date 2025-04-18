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

        public void StartLookAtPlayer()
        {
            _lookAtPlayer = true;
        }

        public void StopLookAtPlayer()
        {
            _lookAtPlayer = false;
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

                transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }
        }
    }

    public interface IBulletComponent
    {
        public void Init(Transform player);
        public void StartShooting();
        public void StopShooting();
        public void StartLookAtPlayer();
        public void StopLookAtPlayer();
        public void UpdateComponent(float delta);
    }
}
