using Enemies;
using Enemies.Manager;
using Game.Player.Manager;
using Stages.Manager;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Stages.Parts
{
    public class GameplayPart : APart
    {
        [SerializeField] private float _partTime = 10f;
        [SerializeField] private EntityController[] _enemies = new EntityController[0];
        //[SerializeField] private EnemyManager _enemyManager = null;

        private IEntityController[] _iEnemies = new IEntityController[0];
        private IEnemyManager _iEnemyManager = default;

        private IPlayerManagerTransform _playerTransform = null;

        private float _timeOnStart = 0;

        [Inject]
        private void Construct(IStageManagerTimer stageManagerTimer, IPlayerManagerTransform playerTransform)
        {
            _timeOnStart = stageManagerTimer.currentTime;

            _playerTransform = playerTransform;

            stageManagerTimer
                .TimeChanged
                .Subscribe(_ => TimerUpdated(_))
                .AddTo(disposable);
        }

        public override void Init()
        {
            _iEnemies = _enemies;
            Transform playerTransform = _playerTransform.PlayerTransform;
            foreach (IEntityController entityController in _iEnemies)
            {
                entityController.Init(playerTransform);
            }
        }

        public override void TimerUpdated(float time)
        {
            base.TimerUpdated(time);
            float localTime = time - _timeOnStart;
            Debug.Log(localTime);
            if (localTime >= _partTime)
            {
                Clear();
            }
        }

        public override void Clear()
        {
            foreach (IEntityController entityController in _iEnemies)
            {
                entityController.Dispose();
            }
            base.Clear();
        }
    }
}
