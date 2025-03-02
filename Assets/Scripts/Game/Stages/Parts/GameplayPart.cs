using Enemies;
using Enemies.Manager;
using Stages.Manager;
using UniRx;
using UnityEngine;
using Zenject;

namespace Stages.Parts
{
    public class GameplayPart : APart
    {
        [SerializeField] private float _partTime = 10f;
        [SerializeField] private EnemyController[] _enemies = new EnemyController[0];
        [SerializeField] private EnemyManager _enemyManager = null;

        private IEnemyController[] _iEnemies = new IEnemyController[0];
        private IEnemyManager _iEnemyManager = default;

        private float _timeOnStart = 0;

        [Inject]
        private void Construct(IStageManagerTimer stageManagerTimer)
        {
            _timeOnStart = stageManagerTimer.currentTime;

            stageManagerTimer
                .TimeChanged
                .Subscribe(_ => TimerUpdated(_))
                .AddTo(_disposable);
        }

        public override void Init()
        {
            _iEnemies = _enemies;
            _iEnemyManager = _enemyManager;

            _iEnemyManager.Init();
        }

        public override void TimerUpdated(float time)
        {
            base.TimerUpdated(time);
            float localTime = time - _timeOnStart;
            if (localTime >= _partTime)
            {
                PartClear?.OnNext(this);
            }
        }
    }
}
