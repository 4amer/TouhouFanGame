using Services.GSMC;
using UnityEngine;
using Services.SceneLoaderC;
using Zenject;
using Game;
using UI;
using Audio;
using UnityEngine.InputSystem;
using Services.Money;

namespace Installers
{
    public class ProjectContext : MonoInstaller
    {
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private UIManager _uIManager;
        [SerializeField] private AudioManager _audioManager;
        [SerializeField] private PlayerInput _playerInput;
        public override void InstallBindings()
        {
            BindServices();
            BindPlayerInput();
            BindManagers();
        }

        private void BindServices()
        {
            Container.BindInterfacesAndSelfTo<GSM>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SceneLoader>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MoneyService>().AsSingle().NonLazy();
        }

        private void BindPlayerInput()
        {
            Container.BindInterfacesAndSelfTo<PlayerInput>()
                .FromComponentInNewPrefab(_playerInput)
                .AsSingle();
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
