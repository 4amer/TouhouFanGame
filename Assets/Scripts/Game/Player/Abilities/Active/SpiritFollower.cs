using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.BulletSystem;
using Game.BulletSystem.Damage;
using Game.BulletSystem.Pool;
using Game.Player.Ability.Active;
using Game.Player.Manager;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game.Player.Ability.Active
{
    public class SpiritFollower : BaseActiveAbility
    {
        private PlayerInput _playerInput = null;
        private Transform _playerTransform = null;

        [SerializeField] private float _onDefaultPosition = 1f;
        [SerializeField] private Vector3 _shiftFromPlayer = Vector3.zero;

        [Space(10)]
        [Header("Shoot Setup")]
        [SerializeField] private float _speed = 8f;
        [SerializeField] private float _reloadDelay = 0.2f;
        [SerializeField] private int _maxBulletAmount = 20;
        [Space(5)]
        [Header("Bullet")]
        [SerializeField] private Bullet _bulletPrefab = null;

        private float _bulletDamage = 0f;
        private float _bulletRange = 0f;

        private IDamagableManager _damagableManager = null;

        private Queue<Bullet> _bulletQueue = new Queue<Bullet>();

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private Utils.Timer _timer = new Utils.Timer();

        IPool<Bullet> _bulletPool = null;

        private bool _isShootingAllowed = false;

        private CompositeDisposable _disposable = new CompositeDisposable();

        [Inject]
        private void Construct(PlayerInput playerInput, IPlayerManagerTransform playerManagerTransform
            , IGameManager gameManager, IPool<Bullet> bulletPool, IDamagableManager damagableManager)
        {
            _bulletPool = bulletPool;
            _playerInput = playerInput;
            _playerTransform = playerManagerTransform.PlayerTransform;

            _damagableManager = damagableManager;

            gameManager
                .Updated
                .Subscribe(_ => UpdateComponent(_))
                .AddTo(_disposable);
        }

        public override void Init()
        {
            base.Init();

            _bulletDamage = _bulletPrefab.Damage;
            _bulletRange = _bulletPrefab.Range;

            _timer = new Utils.Timer();
            _timer.duration = _reloadDelay;
            _timer.timeStep = 0.1f;
            _timer.EventOnFinish = Shoot;
            _timer.Start();

            for (int i = 0; _maxBulletAmount >= i; i++)
            {
                CreateBullet();
            }

            transform.localPosition = _shiftFromPlayer;
        }

        private void Stay()
        {
            _cancellationTokenSource.Cancel();
            Vector3 position = transform.position;
            transform.parent = null;
            transform.position = position;
        }

        private void MoveToPlayer()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            transform.parent = _playerTransform;
            Debug.Log(_playerTransform.position);
            DoMoveToPoint(transform.localPosition, _shiftFromPlayer, _onDefaultPosition, _cancellationTokenSource.Token);
        }

        private void UpdateComponent(float delta)
        {
            foreach (Bullet bullet in _bulletQueue)
            {
                MoveBullet(bullet, delta);
                IDamagable[] damagables = _damagableManager.GetAllDamagables();
                foreach (IDamagable damagable in damagables)
                {
                    Vector3 bulletPositino = bullet.transform.position;
                    Vector3 enemyPositino = damagable.Transform.position;
                    float distance = Vector3.Distance(enemyPositino, bulletPositino);

                    distance -= bullet.Range / 2;

                    if (_bulletRange >= distance)
                    {
                        damagable.Damage(bullet.Damage);
                        bullet.PlayDestroyParticle();
                        bullet.HideSprite();
                    }
                }
            }
        }

        private void MoveBullet(Bullet bullet, float delay)
        { 
            Vector3 bulletPosition = bullet.transform.localPosition;
            bullet.transform.localPosition = bulletPosition + Vector3.right * _speed * delay;
        }

        private void Shoot()
        {
            if (_isShootingAllowed == false) return;
            _timer.Start();
            ReInitBullet();
        }

        public override void NormalShoot()
        {
            base.NormalShoot();
            _isShootingAllowed = true;
            Shoot();
        }

        public void StopShoot()
        {
            _isShootingAllowed = false;
        }

        private void ReInitBullet()
        {
            Bullet bullet = _bulletQueue.Dequeue();
            Transform bulletTransform = bullet.transform;
            bulletTransform.localPosition = Vector3.zero;
            bulletTransform.gameObject.SetActive(true);
            bullet.ShowSprite();
            _bulletQueue.Enqueue(bullet);
        }

        private void CreateBullet()
        {
            Bullet bullet = _bulletPool.Spawn(_bulletPrefab, this.transform, Vector3.zero);
            bullet.gameObject.SetActive(false);
            _bulletQueue.Enqueue(bullet);
        }

        private void OnEnable()
        {
            InputActionMap map = _playerInput.actions.FindActionMap("GamePlay");
            InputAction shoot = map["Shoot"];
            InputAction shift = map["SlowMovement"];

            shoot.started += ctx => NormalShoot();
            shoot.canceled += ctx => StopShoot();

            shift.started += ctx => Stay();
            shift.canceled += ctx => MoveToPlayer();
        }

        private void OnDisable()
        {
            InputActionMap map = _playerInput.actions.FindActionMap("GamePlay");
            InputAction shoot = map["Shoot"];
            InputAction shift = map["SlowMovement"];

            shoot.started -= ctx => NormalShoot();
            shoot.canceled -= ctx => StopShoot();

            shift.started -= ctx => Stay();
            shift.canceled -= ctx => MoveToPlayer();
        }

        private async UniTask DoMoveToPoint(Vector3 startPosition, Vector3 endPosition, float time, CancellationToken token)
        {
            try
            {
                float changeableTimer = 0f;

                while (changeableTimer < time)
                {
                    float t = changeableTimer / time;

                    Vector3 smoothPosition = Vector3.LerpUnclamped(startPosition, endPosition, t);

                    transform.localPosition = smoothPosition;

                    await UniTask.Yield(token);

                    changeableTimer += Time.deltaTime;
                }
            }
            catch (OperationCanceledException)
            {

            }
        }
    }
}
