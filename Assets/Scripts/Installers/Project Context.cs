using Services.GSMC;
using UnityEngine;
using Services.SceneLoaderC;
using Zenject;
using Game;

namespace Installers
{
    public class ProjectContext : MonoInstaller
    {
        [SerializeField] private GameManager _gameManager;
        public override void InstallBindings()
        {
            BindServices();
            BindGameManager();
        }

        private void BindServices()
        {
            Container.BindInterfacesAndSelfTo<GSM>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SceneLoader>().AsSingle().NonLazy();
        }

        private void BindGameManager()
        {
            Container.BindInterfacesAndSelfTo<GameManager>()
                .FromComponentInNewPrefab(_gameManager)
                .AsSingle();
        }
    }
}
