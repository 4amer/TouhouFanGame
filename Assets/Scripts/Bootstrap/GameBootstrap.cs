using Game.BulletSystem.Manager;
using Game.Player.Manager;
using UnityEngine;
using Zenject;

namespace BootstrapService
{
    public class GameBootstrap : MonoBehaviour
    {
        private IPlayerManager _playerManager = null;
        private IBulletComponentManager _bulletComponentManager = null;


        [Inject]
        private void Constract(IPlayerManager playerManager, IBulletComponentManager bulletComponentManager)
        {
            _playerManager = playerManager;
            _bulletComponentManager = bulletComponentManager;
        }

        private void Awake()
        {
            _playerManager.Init();
            _bulletComponentManager.Init();
        }
    }
}
