using Enemies.Drop;
using Game.BackGround.Manager;
using Game.BulletSystem.Damage;
using Game.BulletSystem.Manager;
using Game.BulletSystem.Pool;
using Game.Player.Ability;
using Game.Player.Manager;
using Services.Money;
using Stages.Manager;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Installers
{
    public class GameContext : MonoInstaller
    {
        [SerializeField] private PlayerManager _playerManager = null;
        [SerializeField] private BulletComponentManager _bulletComponentManager = null;
        [SerializeField] private StageManager _stageManager = null;
        [SerializeField] private BulletPool _bulletPool = null;
        [SerializeField] private DropItemPool _dropItemPool = null;
        [SerializeField] private BGManager _bgManager = null;
        [SerializeField] private AbilityManager _abilityManager = null;
        public override void InstallBindings()
        {
            BindManagers();
            BindPool();
        }

        private void BindManagers()
        {
            Container.BindInterfacesAndSelfTo<PlayerManager>().FromInstance(_playerManager);
            Container.BindInterfacesAndSelfTo<BulletComponentManager>().FromInstance(_bulletComponentManager);
            Container.BindInterfacesAndSelfTo<StageManager>().FromInstance(_stageManager);
            Container.BindInterfacesAndSelfTo<BGManager>().FromInstance(_bgManager);
            Container.BindInterfacesAndSelfTo<AbilityManager>().FromInstance(_abilityManager);

            Container.BindInterfacesAndSelfTo<DamagableManager>().AsSingle().NonLazy();
        }

        private void BindPool()
        {
            Container.BindInterfacesAndSelfTo<BulletPool>().FromInstance(_bulletPool);
            Container.BindInterfacesAndSelfTo<DropItemPool>().FromInstance(_dropItemPool);
        }
    }
}
