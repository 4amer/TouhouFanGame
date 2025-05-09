using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UI;
using UI.Windows;
using UniRx;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Zenject;

namespace Services.SceneLoaderC
{
    public class SceneLoader : ISceneLoader, ISceneLoaderActions
    {
        public Subject<Unit> SceneStartLoad { get; set; } = new Subject<Unit>();
        public Subject<float> SceneLoadUpdated { get; set; } = new Subject<float>();
        public Subject<Unit> SceneEndLoad { get; set; } = new Subject<Unit>();

        private AsyncOperationHandle<SceneInstance> _sceneHandle;

        private IUIManager _uIManager = null;

        private const float _delayToStart = 2f;

        [Inject]
        private void Construct(IUIManager uIManager)
        {
            _uIManager = uIManager;
        }

        public void LoadScene(string sceneKey)
        {
            ShowLoadingWindow();
            SetupDelayTimer(() =>
            {
                LoadSceneAsync(sceneKey);
            });
        }

        public async UniTask LoadSceneAsync(string sceneKey)
        {
            var handle = Addressables.LoadSceneAsync(
                sceneKey,
                LoadSceneMode.Single,
                activateOnLoad: false
            );

            SceneStartLoad?.OnNext(Unit.Default);

            while (handle.PercentComplete < 0.9f)
            {
                SceneLoadUpdated?.OnNext(handle.PercentComplete);
                await UniTask.Yield();
            }

            await UniTask.Delay(5000);

            var activationOp = handle.Result.ActivateAsync();
            activationOp.allowSceneActivation = true;
            await activationOp;

            SceneEndLoad?.OnNext(Unit.Default);
        }

        public void UnloadScene()
        {
            if (_sceneHandle.IsValid())
            {
                Addressables.UnloadSceneAsync(_sceneHandle);
            }
        }

        private void SetupDelayTimer(Action actionOnFinish)
        {
            Utils.Timer timer = new Utils.Timer();
            timer.duration = _delayToStart;
            timer.OnTimerFinish += () =>
            {
                actionOnFinish?.Invoke();
            };
            timer.Start();
        }

        private void ShowLoadingWindow()
        {
            AWindow<LoadingWindowData> aWindow = _uIManager.GetWindow<LoadingWindow>();
            _uIManager.Show(aWindow);
        }
    }

    public interface ISceneLoaderActions
    {
        public Subject<Unit> SceneStartLoad { get; set; }
        public Subject<float> SceneLoadUpdated { get; set; }
        public Subject<Unit> SceneEndLoad { get; set; }
    }

    public interface ISceneLoader
    {
        public void LoadScene(string sceneName);
        public void UnloadScene();
    }
}
