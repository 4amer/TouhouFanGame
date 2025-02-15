using Game.BulletSystem.Manager;
using Game.Player.Manager;
using Stages.Manager;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class GameContext : MonoInstaller
    {
        [SerializeField] private PlayerManager _playerManager = null;
        [SerializeField] private BulletComponentManager _bulletComponentManager = null;
        [SerializeField] private StageManager _stageManager = null;
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PlayerManager>().FromInstance(_playerManager);
            Container.BindInterfacesAndSelfTo<BulletComponentManager>().FromInstance(_bulletComponentManager);
            Container.BindInterfacesAndSelfTo<StageManager>().FromInstance(_stageManager);
        }
    }
}
