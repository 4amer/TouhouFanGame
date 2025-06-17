using Game.BulletSystem.Manager;
using Game.Player.Manager;
using Services.SceneLoaderC;
using Stages.Manager;
using UniRx;
using UnityEngine;
using Zenject;

namespace BootstrapService
{
    public class GameBootstrap : MonoBehaviour
    {
        private IPlayerManager _playerManager = null;
        private IStageManager _stageManager = null;

        private CompositeDisposable _disposable = new CompositeDisposable();

        [Inject]
        private void Constract(IPlayerManager playerManager, IStageManager stageManager, ISceneLoaderActions sceneLoaderActions)
        {
            _playerManager = playerManager;
            _stageManager = stageManager;

            sceneLoaderActions
                .SceneEndLoad
                .Subscribe(_ => Init())
                .AddTo(_disposable);
        }

        private void Init()
        {
            _playerManager.Init();
            _stageManager.Init();
            //_bulletComponentManager.Init();
        }

        private void OnDestroy()
        {
            _disposable?.Clear();
            _disposable?.Dispose();
        }
    }
}
