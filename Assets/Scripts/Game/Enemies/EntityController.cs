using System.Collections.Generic;
using BezierMovementSystem;
using Enemies.Sequences;
using Game;
using Game.BulletSystem;
using Game.Player.Manager;
using Stages.Manager;
using UniRx;
using UnityEngine;
using Zenject;

namespace Enemies
{
    public class EntityController : MonoBehaviour, IEntityController
    {
        [SerializeField] private bool _isSequenceCycled = false;
        [SerializeField] private EventSequence[] _eventSequence = new EventSequence[1];
        [SerializeField] private BulletComponent[] _bulletComponents = default;
        [SerializeField] private MovementComponent _movementBezierComponent = default;

        [Space(10)]
        [Header("Enemy Object")]
        [SerializeField] private GameObject _enemyGameObject = null;

        private IBulletComponent[] _iBulletComponents = default;
        private IMovementBezierComponent _iMovementBezierComponent = default;

        private Queue<EventSequence> _eventSequencesQueue;
        public bool IsSequenceCycled { get => _isSequenceCycled; }
        public Queue<EventSequence> EventSequencesQueue { get => _eventSequencesQueue; }

        private float _timeShift = 0f;
        private float _lastTimeEvent = 0f;

        private CompositeDisposable _disposable = new CompositeDisposable();

        [Inject]
        private void Construct(IStageManagerTimer stageManagerTimer, IGameManager gameManager)
        {
            _timeShift = stageManagerTimer.currentTime;

            stageManagerTimer
                .TimeChanged
                .Subscribe(_ => TimerUpdated(_))
                .AddTo(_disposable);

            foreach (IBulletComponent bulletComponent in _bulletComponents)
            {
                gameManager
                    .Updated
                    .Subscribe(_ => bulletComponent.UpdateComponent(_))
                    .AddTo(_disposable);
            }

        }
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
            _eventSequencesQueue = new Queue<EventSequence>(_eventSequence);
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

        private void TimerUpdated(float time)
        {
            if (EventSequencesQueue.Count == 0 && IsSequenceCycled)
            {
                RestoreSequence();
                _lastTimeEvent = 0;
                _timeShift = time - 0.1f;
            }

            float localTime = time - _timeShift;

            while (EventSequencesQueue.Count > 0)
            {
                EventSequence currentSequence = EventSequencesQueue.Peek();
                float scheduledTime = _lastTimeEvent + currentSequence.Delay;

                if (scheduledTime <= localTime)
                {
                    currentSequence.Event.Invoke();
                    _lastTimeEvent = scheduledTime;
                    EventSequencesQueue.Dequeue();
                }
                else
                {
                    break;
                }
            }
        }
        public void Dispose()
        {
            StopShoot();
            StopMove();
            _disposable.Dispose();
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
        public void Dispose();
    }
}
