using Game.BulletSystem;
using UnityEngine;

namespace Enemies
{
    public class Enemy : MonoBehaviour, IEnemy
    {
        [SerializeField] private float _durationBeforeMove = 5f;
        [SerializeField] private float _durationBeforeShoot = 6f;

        [SerializeField] private float _movementSpeed = 1f;

        [SerializeField] private BulletComponent _bulletComponent = default;

        [SerializeField] private IBulletComponent _iBulletComponent = default;
        public float DurationBeforeStart { get => _durationBeforeMove; }
        public float DurationBeforeShoot { get => _durationBeforeShoot; }

        public void Init(Transform player)
        {
            _iBulletComponent = _bulletComponent;
            _iBulletComponent.Init(player);
        }

        public void UpdateEnemy(float delta)
        {

        }

        public void StartLookAtPlayer()
        {
            
        }

        public void StartMove()
        {
            
        }

        public void StartShoot()
        {
            
        }

        public void StopLookAtPlayer()
        {
            
        }

        public void StopMove()
        {
            
        }

        public void StopShoot()
        {
            
        }
    }

    public interface IEnemy
    {
        public float DurationBeforeStart { get; }
        public float DurationBeforeShoot { get; }
        public void Init(Transform player);
        public void UpdateEnemy(float delta);
        public void StartMove();
        public void StopMove();
        public void StartShoot();
        public void StopShoot();
        public void StartLookAtPlayer();
        public void StopLookAtPlayer();
    }
}
