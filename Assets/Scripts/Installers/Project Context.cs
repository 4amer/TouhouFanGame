using Services.GSMC;
using UnityEngine;
using Services.SceneLoaderC;
using Zenject;
using Game;
using UI;
using Audio;

namespace Installers
{
    public class ProjectContext : MonoInstaller
    {
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private UIManager _uIManager;
        [SerializeField] private AudioManager _audioManager;
        public override void InstallBindings()
        {
            BindServices();
            BindManagers();
        }

        private void BindServices()
        {
            Container.BindInterfacesAndSelfTo<GSM>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SceneLoader>().AsSingle().NonLazy();
        }

        private void BindManagers()
        {
            Container.BindInterfacesAndSelfTo<GameManager>()
                .FromComponentInNewPrefab(_gameManager)
                .AsSingle();

            Container.BindInterfacesAndSelfTo<UIManager>()
                .FromComponentInNewPrefab(_uIManager)
                .AsSingle();
            
            Container.BindInterfacesAndSelfTo<AudioManager>()
                .FromComponentInNewPrefab(_audioManager)
                .AsSingle();
        }
    }
}
