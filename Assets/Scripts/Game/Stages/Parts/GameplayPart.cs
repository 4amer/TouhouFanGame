using Enemies;
using Enemies.Manager;
using UnityEngine;

namespace Stages.Parts
{
    public class GameplayPart : APart
    {
        [SerializeField] private float _partTime = 10f;
        [SerializeField] private Enemy[] _enemies = new Enemy[0];
        [SerializeField] private EnemyManager _enemyManager = null;

        private IEnemy[] _iEnemies = new IEnemy[0];
        private IEnemyManager _iEnemyManager = default;

        public override void Init()
        {
            _iEnemies = _enemies;
            _iEnemyManager = _enemyManager;
        }

        public override void TimerUpdated(float time)
        {
            base.TimerUpdated(time);

        }
    }
}
