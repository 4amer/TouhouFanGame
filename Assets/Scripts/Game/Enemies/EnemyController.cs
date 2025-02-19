using System.Collections.Generic;
using Enemies.Sequences;
using Game.BulletSystem;
using Stages.Manager;
using UnityEngine;
using Zenject;

namespace Enemies
{
    public class EnemyController : MonoBehaviour, IEnemyController
    {
        [SerializeField] private ShootSequence[] _shootSequence = new ShootSequence[1];
        [SerializeField] private BulletComponent _bulletComponent = default;

        private IBulletComponent _iBulletComponent = default;

        private Queue<ShootSequence> _shootSequencesQueue;
        public Queue<ShootSequence> ShootSequencesQueue { get => _shootSequencesQueue; }

        public void Init(Transform player)
        {
            _shootSequencesQueue = new Queue<ShootSequence>(_shootSequence);   

            PrepareSequencies();

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
            Debug.Log("Shooting");
            _iBulletComponent.StartShooting();
        }

        public void StopLookAtPlayer()
        {
            
        }

        public void StopMove()
        {
            
        }

        public void StopShoot()
        {
            Debug.Log("Stop Shooting");
            _iBulletComponent.StopShooting();
        }

        private void PrepareSequencies()
        {
            foreach (ShootSequence item in _shootSequence)
            {
                switch (item.Type)
                {
                    case ShootEventType.Shoot:
                        item.Event = StartShoot;
                        break;
                    case ShootEventType.StopShoot:
                        item.Event = StopShoot;
                        break;
                    case ShootEventType.LookAtPlayer:
                        item.Event = StartLookAtPlayer;
                        break;
                    case ShootEventType.StopLookAtPlayer:
                        item.Event = StopLookAtPlayer;
                        break;
                }
            }
        }
    }

    public interface IEnemyController
    {
        public Queue<ShootSequence> ShootSequencesQueue { get; }
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
