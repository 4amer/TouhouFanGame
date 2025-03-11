using Enemies.Manager;
using Game.Player.Manager;
using UnityEngine;
using Zenject;

namespace Enemies.Bosses.Attack
{
    public class BossPattern : MonoBehaviour
    {
        [SerializeField] private EntityController[] _entityControllers = null;

        private Transform _playerTransform = null;

        private DiContainer _diContainer = null;

        [Inject]
        private void Construct(DiContainer diContainer, IPlayerManagerTransform playerManagerTransform)
        {
            _diContainer = diContainer;
            _playerTransform = playerManagerTransform.PlayerTransform;
        }

        public void Init(GameObject boss)
        {
            foreach (EntityController entity in _entityControllers)
            {
                _diContainer.Inject(entity);
                entity.Init(_playerTransform, boss);
            }
        }

        public void Clear()
        {
            foreach (IEntityController entityController in _entityControllers)
            {
                entityController.Dispose();
            }
        }
    }
}
