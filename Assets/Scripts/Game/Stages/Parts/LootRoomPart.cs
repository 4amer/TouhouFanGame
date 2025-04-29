using Cysharp.Threading.Tasks;
using Enemies;
using Game.BulletSystem.Damage;
using Game.Player.Manager;
using UniRx;
using UnityEditor.Rendering;
using UnityEngine;
using Zenject;

namespace Stages.Parts
{
    public class LootRoomPart : PassivePart
    {
        [SerializeField] private EntityController[] _entityControllers = new EntityController[1];
        [SerializeField] private float _timeToBeat = 20f;

        private int _enemyCounter = 0;

        private Transform _playerTransform = null;

        private Utils.Timer _timer = new Utils.Timer();

        private CompositeDisposable _disposable = new CompositeDisposable();    

        [Inject]
        private void Construct(IPlayerManagerTransform playerManagerTransform)
        {
            _playerTransform = playerManagerTransform.PlayerTransform;
        }

        public override void Init()
        {
            base.Init();


            foreach (EntityController entityController in _entityControllers)
            {
                entityController.Init(_playerTransform);
                entityController
                    .OnDead
                    .Subscribe(_ => EnemyBeaten(_))
                    .AddTo(_disposable);
            }

            _timer = new Utils.Timer();
            _timer.duration = _timeToBeat;
            _timer.OnTimerFinish += () =>
            {
                Clear();
            };
            _timer.Start();
        }

        private void EnemyBeaten(IDamagable damagable) 
        {
            _enemyCounter++;
            if(_enemyCounter >= _entityControllers.Length)
            {
                _timer.Reset();
                _timer.duration = 2f;
                _timer.Start();
            }
        }

        public override void Clear()
        {
            base.Clear();
            foreach (EntityController entityController in _entityControllers)
            {
                entityController.Dispose();
            }
            _disposable?.Clear();
        }
    }
}
