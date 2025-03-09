using System.Collections.Generic;
using BezierMovementSystem;
using Enemies.Sequences;
using Game.BulletSystem;
using Stages.Manager;
using UnityEngine;
using Zenject;

namespace Enemies
{
    public class EntityController : MonoBehaviour, IEntityController
    {
        [SerializeField] private bool _isSequenceCycled = false;
        [SerializeField] private EventSequence[] _eventSequence = new EventSequence[1];
        [SerializeField] private BulletComponent[] _bulletComponents = default;
        [SerializeField] private MovementBezierComponent _movementBezierComponent = default;

        [Space(10)]
        [Header("Enemy Object")]
        [SerializeField] private GameObject _enemyGameObject = null;

        private IBulletComponent[] _iBulletComponents = default;
        private IMovementBezierComponent _iMovementBezierComponent = default;

        private Queue<EventSequence> _eventSequencesQueue;
        public bool IsSequenceCycled { get => _isSequenceCycled; }
        public Queue<EventSequence> EventSequencesQueue { get => _eventSequencesQueue; }

        public void Init(Transform player, GameObject entity = null)
        {
            if (entity != null) _enemyGameObject = entity;

            _eventSequencesQueue = new Queue<EventSequence>(_eventSequence);   

            PrepareSequencies();

            _iMovementBezierComponent = _movementBezierComponent;
            _iMovementBezierComponent?.Init(_enemyGameObject);

            _iBulletComponents = _bulletComponents;
            foreach (IBulletComponent bulletComponent in _iBulletComponents)
            {
                bulletComponent.Init(player);
            }
        }

        public void UpdateEnemy(float delta)
        {
            foreach (IBulletComponent bulletComponent in _iBulletComponents)
            {
                bulletComponent.UpdateComponent(delta);
            }
        }

        public void StartLookAtPlayer()
        {
            
        }

        public void StartMove()
        {
            Debug.Log("Start movement");
            _iMovementBezierComponent?.StartMovement();
        }

        public void StartShoot()
        {
            Debug.Log("Shooting");
            foreach (IBulletComponent bulletComponent in _iBulletComponents)
            {
                bulletComponent.StartShooting();
            }
        }

        public void StopLookAtPlayer()
        {
            
        }

        public void StopMove()
        {
            Debug.Log("Stop movement");
            _iMovementBezierComponent?.StopMovement();
        }

        public void StopShoot()
        {
            Debug.Log("Stop Shooting");
            foreach (IBulletComponent bulletComponent in _iBulletComponents)
            {
                bulletComponent.StopShooting();
            }
        }

        public void RestoreSequence()
        {
            PrepareSequencies();
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

    public interface IEntityController
    {
        public bool IsSequenceCycled { get; }
        public Queue<EventSequence> EventSequencesQueue { get; }
        public void Init(Transform player, GameObject entity = null);
        public void UpdateEnemy(float delta);
        public void StartMove();
        public void StopMove();
        public void StartShoot();
        public void StopShoot();
        public void StartLookAtPlayer();
        public void StopLookAtPlayer();
        public void RestoreSequence();
    }
}
