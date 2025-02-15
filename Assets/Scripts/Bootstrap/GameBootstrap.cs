using Game.BulletSystem.Manager;
using Game.Player.Manager;
using Stages.Manager;
using UnityEngine;
using Zenject;

namespace BootstrapService
{
    public class GameBootstrap : MonoBehaviour
    {
        private IPlayerManager _playerManager = null;
        private IStageManager _stageManager = null;

        [Inject]
        private void Constract(IPlayerManager playerManager, IStageManager stageManager)
        {
            _playerManager = playerManager;
            _stageManager = stageManager;
        }

        private void Awake()
        {
            _playerManager.Init();
            _stageManager.Init();
            //_bulletComponentManager.Init();
        }
    }
}
