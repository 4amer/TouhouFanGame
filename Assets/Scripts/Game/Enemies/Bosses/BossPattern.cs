using Game.Player.Manager;
using UnityEngine;
using Zenject;

namespace Enemies.Bosses.Attack
{
    public class BossPattern : MonoBehaviour
    {
        [SerializeField] private EntityController[] _entityControllers = null;

        private Transform _playerTransform = null;

        [Inject]
        private void Construct(IPlayerManagerTransform playerManagerTransform)
        {
            _playerTransform = playerManagerTransform.PlayerTransform;
        }

        public void Init(GameObject boss)
        {
            foreach (EntityController entity in _entityControllers)
            {
                entity.Init(_playerTransform, boss);
            }
        }
    }
}
