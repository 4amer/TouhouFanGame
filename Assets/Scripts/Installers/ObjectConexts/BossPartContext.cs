using Enemies.Bosses;
using Enemies.Bosses.HP;
using Enemies.Bosses.SpellCards;
using Enemies.Bosses.Timer;
using UnityEngine;
using Zenject;

namespace Installers.ObjectContext
{
    public class BossPartContext : MonoInstaller
    {
        [SerializeField] private TimerCotroller _timerController = null;
        [SerializeField] private BaseBoss _baseBoss = null;
        [SerializeField] private HealthController _healthController = null;
        [SerializeField] private SpellCardManager _spellCardManager = null;
        public override void InstallBindings()
        {
            BindBoss();
            BindControllers();
        }

        private void BindBoss()
        {
            Container.BindInterfacesAndSelfTo<BaseBoss>().FromInstance(_baseBoss).AsSingle().NonLazy();
        }

        private void BindControllers()
        {
            Container.BindInterfacesAndSelfTo<TimerCotroller>().FromInstance(_timerController).AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<HealthController>().FromInstance(_healthController).AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SpellCardManager>().FromInstance(_spellCardManager).AsSingle().NonLazy();
        }
    }
}
