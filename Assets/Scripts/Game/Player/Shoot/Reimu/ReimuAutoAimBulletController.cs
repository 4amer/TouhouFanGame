using Game.BulletSystem.Damage;
using Game.BulletSystem.Pool;
using Game;
using UnityEngine;
using Zenject;
using UniRx;
using Game.BulletSystem;
using System;

namespace Player.Shoot.Reimu
{
    public class ReimuAutoAimBulletController : MonoBehaviour
    {
        [SerializeField] private float _speed = 8f;
        [SerializeField] private float _reloadDelay = 0.2f;
        [SerializeField] private int _maxBulletAmount = 20;

        private IPool<Bullet> _bulletPool = null;

        private IDamagableManager _damagableManager = null;

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

        public void Show()
        {
            this.gameObject.active = true;
        }

        public void Hide()
        {
            this.gameObject.active = false;
        }

        private void UpdateComponent(float delta)
        {
        }

        private void LateUpdateComponent(float delta)
        {
            LookAtCamera();
        }

        private void LookAtCamera()
        {
            transform.LookAt(Camera.main.transform);
        }
    }
}
