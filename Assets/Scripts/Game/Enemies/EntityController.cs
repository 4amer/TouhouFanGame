using System.Collections.Generic;
using BezierMovementSystem;
using Enemies.Bosses.HP;
using Enemies.Drop;
using Enemies.Sequences;
using Game;
using Game.BulletSystem;
using Game.BulletSystem.Damage;
using Game.Player.Manager;
using Stages.Manager;
using UniRx;
using UnityEngine;
using Zenject;

namespace Enemies
{
    public class EntityController : MonoBehaviour, IEntityController, IDamagable
    {
        [Header("Common")]
        [SerializeField] private float _HP = 1f;

        [Space(10)]
        [Header("Actions")]
        [SerializeField] private bool _isSequenceCycled = false;
        [SerializeField] private EventSequence[] _eventSequence = new EventSequence[1];
        [SerializeField] private BulletComponent[] _bulletComponents = default;
        [SerializeField] private MovementComponent _movementBezierComponent = default;

        [Space(10)]
        [Header("Enemy Object")]
        [SerializeField] private GameObject _enemyGameObject = null;
        private IDamagable _enemyDamagable = null;

        [Space(10)]
        [Header("Other Components")]
        [SerializeField] private HealthController _healthController = null;
        [SerializeField] private DropController _dropController = null;

        private IBulletComponent[] _iBulletComponents = default;
        private IMovementBezierComponent _iMovementBezierComponent = default;

        private IDamagableManager _damagableManager = null;

        private Queue<EventSequence> _eventSequencesQueue;
        public bool IsSequenceCycled { get => _isSequenceCycled; }
        public Queue<EventSequence> EventSequencesQueue { get => _eventSequencesQueue; }
        public Subject<IDamagable> OnDead { get; set; } = new Subject<IDamagable>();
        public Subject<float> OnDamaged { get; set; } = new Subject<float>();

        public Transform Transform => transform;


        public float RangeToCollide => 1f;

        public bool IsVulnerable { get; set; } = false;

        private float _timeShift = 0f;
        private float _lastTimeEvent = 0f;

        private CompositeDisposable _disposable = new CompositeDisposable();

        [Inject]
        private void Construct(IStageManagerTimer stageManagerTimer, IGameManager gameManager, IDamagableManager damagableManager)
        {
            _damagableManager = damagableManager;

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
        public void Init(Transform player, IDamagable entity = null)
        {
            _damagableManager.AddDamagable(this);
            _dropController?.Init(this);

            if (entity != null)
            {
                _enemyGameObject = entity.Transform.gameObject;
                _enemyDamagable = entity;
            } 
            else
            {
                _enemyDamagable = this;
            }

            _eventSequencesQueue = new Queue<EventSequence>(_eventSequence);

            PrepareSequencies();

            _iMovementBezierComponent = _movementBezierComponent;
            _iMovementBezierComponent?.Init(_enemyGameObject.transform);

            _iBulletComponents = _bulletComponents;
            foreach (IBulletComponent bulletComponent in _iBulletComponents)
            {
                bulletComponent.Init(player);
            }

            SetupHealthController();
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
            foreach (IBulletComponent bulletComponent in _bulletComponents)
            {
                bulletComponent?.StartLookAtPlayer();
            }
        }

        public void StartMove()
        {
            _iMovementBezierComponent?.StartMovement();
        }

        public void StartShoot()
        {
            foreach (IBulletComponent bulletComponent in _iBulletComponents)
            {
                bulletComponent.StartShooting();
            }
        }

        public void StopLookAtPlayer()
        {
            foreach (IBulletComponent bulletComponent in _bulletComponents)
            {
                bulletComponent?.StopLookAtPlayer();
            }
        }

        public void StopMove()
        {
            _iMovementBezierComponent?.StopMovement();
        }

        public void StopShoot()
        {
            foreach (IBulletComponent bulletComponent in _iBulletComponents)
            {
                bulletComponent.StopShooting();
            }
        }

        public void Invulnerable()
        {
            IsVulnerable = false;
            _enemyDamagable.IsVulnerable = false;
        }

        public void Vulnerable()
        {
            IsVulnerable = true;
            _enemyDamagable.IsVulnerable = true;
        }

        public void RestoreSequence()
        {
            _eventSequencesQueue = new Queue<EventSequence>(_eventSequence);
            PrepareSequencies();
        }

        private void SetupHealthController()
        {
            _healthController?.Init(this, _HP);

            _healthController?
                .OnDead
                .Subscribe(_ => Dead())
                .AddTo(_disposable);
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
                    case Sequences.EventType.Vulnerable:
                        item.Event = Vulnerable;
                        break;
                    case Sequences.EventType.Invulnerable:
                        item.Event = Invulnerable;
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

        private void Hide()
        {
            gameObject.active = false;
        }

        private void StopControll()
        {
            StopShoot();
            StopMove();
            StopLookAtPlayer();
            Hide();
        }

        private void Dead()
        {
            StopControll();
            OnDead?.OnNext(this);
        }

        public void Dispose()
        {
            StopControll();
            _damagableManager.RemoveDamagable(this);
            _disposable?.Clear();
        }

        public void Damage(float damage)
        {
            OnDamaged?.OnNext(damage);
        }

        private void OnDisable()
        {
            _disposable?.Clear();
        }
    }

    public interface IEntityController
    {
        public bool IsSequenceCycled { get; }
        public Queue<EventSequence> EventSequencesQueue { get; }
        public void Init(Transform player, IDamagable entity = null);
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
