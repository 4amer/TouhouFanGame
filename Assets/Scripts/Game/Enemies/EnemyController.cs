using System.Collections.Generic;
using BezierMovementSystem;
using Enemies.Sequences;
using Game.BulletSystem;
using Stages.Manager;
using UnityEngine;
using Zenject;

namespace Enemies
{
    public class EnemyController : MonoBehaviour, IEnemyController
    {
        [SerializeField] private EventSequence[] _eventSequence = new EventSequence[1];
        [SerializeField] private BulletComponent _bulletComponent = default;
        [SerializeField] private MovementBezierComponent _movementBezierComponent = default;

        [Space(10)]
        [Header("Enemy Object")]
        [SerializeField] private GameObject _enemyGameObject = null;

        private IBulletComponent _iBulletComponent = default;
        private IMovementBezierComponent _iMovementBezierComponent = default;

        private Queue<EventSequence> _eventSequencesQueue;
        public Queue<EventSequence> EventSequencesQueue { get => _eventSequencesQueue; }

        public void Init(Transform player)
        {
            _eventSequencesQueue = new Queue<EventSequence>(_eventSequence);   

            PrepareSequencies();

            _iMovementBezierComponent = _movementBezierComponent;
            _iMovementBezierComponent.Init(_enemyGameObject);

            _iBulletComponent = _bulletComponent;
            _iBulletComponent.Init(player);
        }

        public void UpdateEnemy(float delta)
        {
            _iBulletComponent.UpdateComponent(delta);
        }

        public void StartLookAtPlayer()
        {
            
        }

        public void StartMove()
        {
            Debug.Log("Start movement");
            _iMovementBezierComponent.StartMovement();
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
            Debug.Log("Stop movement");
            _iMovementBezierComponent.StopMovement();
        }

        public void StopShoot()
        {
            Debug.Log("Stop Shooting");
            _iBulletComponent.StopShooting();
        }

        private void PrepareSequencies()
        {
            foreach (EventSequence item in _eventSequence)
            {
                switch (item.Type)
                {
                    case Sequences.EventType.Shoot:
                        item.Event = StartShoot;
                        break;
                    case Sequences.EventType.StopShoot:
                        item.Event = StopShoot;
                        break;
                    case Sequences.EventType.LookAtPlayer:
                        item.Event = StartLookAtPlayer;
                        break;
                    case Sequences.EventType.StopLookAtPlayer:
                        item.Event = StopLookAtPlayer;
                        break;
                    case Sequences.EventType.Move:
                        item.Event = StartMove;
                        break;
                    case Sequences.EventType.StopMove:
                        item.Event = StopMove;
                        break;
                }
            }
        }
    }

    public interface IEnemyController
    {
        public Queue<EventSequence> EventSequencesQueue { get; }
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
