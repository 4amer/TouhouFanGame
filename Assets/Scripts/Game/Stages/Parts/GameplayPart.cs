using Enemies;
using UnityEngine;

namespace Stages.Parts
{
    public class GameplayPart : APart
    {
        [SerializeField] private float _partTime = 10f;
        [SerializeField] private Enemy[] _enemies = new Enemy[0];

        private IEnemy[] _iEnemies = new IEnemy[0];

        public override void Init()
        {
            _iEnemies = _enemies;
        }
    }
}
