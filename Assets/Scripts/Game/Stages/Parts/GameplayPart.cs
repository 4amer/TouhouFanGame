using Enemies;
using Enemies.Manager;
using Stages.Manager;
using UniRx;
using UnityEngine;

namespace Stages.Parts
{
    public class GameplayPart : APart, IPartAction
    {
        [SerializeField] private float _partTime = 10f;
        [SerializeField] private EnemyController[] _enemies = new EnemyController[0];
        [SerializeField] private EnemyManager _enemyManager = null;

        private IEnemyController[] _iEnemies = new IEnemyController[0];
        private IEnemyManager _iEnemyManager = default;

        public Subject<float> TimerUpdatedLocal { get; set; } = new Subject<float>();

        public override void Init()
        {
            _iEnemies = _enemies;
            _iEnemyManager = _enemyManager;

            _iEnemyManager.Init(this);
        }

        public override void TimerUpdated(float time)
        {
            base.TimerUpdated(time);
            TimerUpdatedLocal.OnNext(time);
        }
    }

    public interface IPartAction
    {
        public Subject<float> TimerUpdatedLocal { get; }
    }
}
