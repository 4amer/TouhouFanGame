using Game.Player.Manager;
using UnityEngine;
using Zenject;

namespace Enemies.Bosses.Attack
{
    public class BossAttack : MonoBehaviour
    {
        [SerializeField] private EntityController[] _entityControllers = null;

        private Transform _playerTransform = null;

        [Inject]
        private void Construct(IPlayerManagerTransform playerManagerTransform)
        {
            _playerTransform = playerManagerTransform.PlayerTransform;
        }

        public void Init()
        {
            foreach (EntityController entity in _entityControllers)
            {
                entity.Init(_playerTransform);
            }
        }
    }
}
