using Game.BulletSystem.Damage;
using Game.BulletSystem.Pool;
using Game;
using UnityEngine;
using Zenject;
using UniRx;
using Game.BulletSystem;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using DG.Tweening;

namespace Player.Shoot.Reimu
{
    public class ReimuAutoAimBulletController : MonoBehaviour
    {
        [SerializeField] private float _speed = 8f;
        [SerializeField] private float _reloadDelay = 0.2f;
        [SerializeField] private int _maxBulletAmount = 20;

        private float _bulletDamage = 0f;
        private float _bulletRange = 0f;

        private Utils.Timer timer = null;

        private bool _isShootingAllowed = false;

        private IPool<Bullet> _bulletPool = null;

        private IDamagableManager _damagableManager = null;

        private Bullet _bulletPrefab = null;

        private Queue<Bullet> _bulletsQueue = new Queue<Bullet>();

        private bool _isActive = false;

        private Dictionary<Bullet, IDamagable> _bulletToDamagable = new Dictionary<Bullet, IDamagable>();

        private CompositeDisposable _disposable = new CompositeDisposable();

        [Inject]
        private void Construct(IGameManager gameManager, IPool<Bullet> bulletPool, IDamagableManager damagableManager)
        {
            gameManager
                .Updated
                .Subscribe(_ => UpdateComponent(_))
                .AddTo(_disposable);

            gameManager
                .LateUpdated
                .Subscribe(_ => LateUpdateComponent(_))
                .AddTo(_disposable);

            _bulletPool = bulletPool;

            _damagableManager = damagableManager;
        }

        public void Init(Bullet prefab)
        {
            _bulletPrefab = prefab;

            _bulletDamage = prefab.Damage;
            _bulletRange = prefab.Range;

            timer = new Utils.Timer();
            timer.duration = _reloadDelay;
            timer.timeStep = 0.1f;
            timer.EventOnFinish = Shoot;
            timer.Start();

            for (int i = 0; _maxBulletAmount >= i; i++)
            {
                CreateBullet();
            }
        }

        public void Show()
        {
            this.gameObject.active = true;
            _isActive = true;
        }

        public void Hide()
        {
            this.gameObject.active = false;
            _isActive = false;
        }

        private void UpdateComponent(float delta)
        {
            foreach (Bullet bullet in _bulletsQueue)
            {
                MoveBullet(bullet, delta);
            }
        }

        private void LateUpdateComponent(float delta)
        {
            LookAtCamera();
            foreach(Bullet bullet in _bulletsQueue)
            {
                BulletLookAtEnemy(bullet);
            }
        }

        private void LookAtCamera()
        {
            transform.LookAt(Camera.main.transform);
        }

        private void BulletLookAtEnemy(Bullet bullet)
        {
            IDamagable damagable = GetDamagableByBullet(bullet);

            if (damagable == null) {
                bullet.transform.rotation = Quaternion.identity;
                return;
            } 
            else
            {
                Vector3 direction = _bulletToDamagable[bullet].Transform.position - bullet.transform.position;
                bullet.transform.right = direction.normalized;
            }

        }

        private void MoveBullet(Bullet bullet, float delay)
        {
            IDamagable damagable = GetDamagableByBullet(bullet);

            Vector3 bulletPosition = bullet.transform.localPosition;
            
            if (damagable == null)
            {
                bullet.transform.localPosition = bulletPosition + Vector3.right * _speed * delay;
            }
            else
            {
                Vector3 damagablePosition = damagable.Transform.position;
                float step = _speed * Time.deltaTime;
                bullet.transform.position = Vector3.MoveTowards(bulletPosition, damagablePosition, step);
                if (Vector3.Distance(bulletPosition, damagablePosition) < 1f)
                {
                    damagable.Damage(bullet.Damage);
                    _bulletPool.Release(bullet);
                }
            }
        }

        private IDamagable GetDamagableByBullet(Bullet bullet)
        {
            if (_bulletToDamagable.Count == 0) return null;
            if (_bulletToDamagable.ContainsKey(bullet) == false) return null;

            IDamagable damagable = _bulletToDamagable[bullet];

            return damagable;
        }

        private void Shoot()
        {
            if (_isShootingAllowed == false) return;
            timer.Start();
            if (_isActive == false) return;
            ReInitBullet();
        }

        public void AllowShooting()
        {
            _isShootingAllowed = true;
            Shoot();
        }

        public void StopShooting()
        {
            _isShootingAllowed = false;
        }

        private void ReInitBullet()
        {
            Bullet bullet = _bulletsQueue.Dequeue();
            Transform bulletTransform = bullet.transform;
            bulletTransform.parent = this.transform;
            bulletTransform.localPosition = Vector3.zero;
            bulletTransform.gameObject.SetActive(true);
            bulletTransform.parent = null;
            _bulletsQueue.Enqueue(bullet);
            FindTheClosiestDamagable(bullet);
        }

        private void FindTheClosiestDamagable(Bullet bullet)
        {
            Vector3 bulletPosition = bullet.transform.position;

            IDamagable[] damagables = _damagableManager.GetAllDamagables();

            float smallestDistance = 0;

            IDamagable closiestDamagable = null;

            foreach (IDamagable damagable in damagables)
            {
                if(damagable.IsVulnerable == false) continue;

                Vector3 position = damagable.Transform.position;

                float distance = Vector3.Distance(position, bulletPosition);

                if (smallestDistance == null || distance > smallestDistance)
                {
                    smallestDistance = distance;
                    closiestDamagable = damagable;
                }
            }

            if (closiestDamagable == null) return;

            if (_bulletToDamagable.ContainsKey(bullet) == false)
            {
                _bulletToDamagable.Add(bullet, closiestDamagable);
            } 
            else
            {
                _bulletToDamagable[bullet] = closiestDamagable;
            }

            closiestDamagable
                .OnDead
                .Subscribe(_ =>
                {
                    _bulletToDamagable[bullet] = null;
                })
                .AddTo(_disposable);
        }

        private void CreateBullet()
        {
            Bullet bullet = _bulletPool.Spawn(_bulletPrefab, this.transform, Vector3.zero);
            bullet.gameObject.SetActive(false);
            _bulletsQueue.Enqueue(bullet);
        }
    }
}
