using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
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

        public void LoadScene(string sceneKey)
        {
            LoadSceneAsync(sceneKey);
        }

        public async UniTask LoadSceneAsync(string sceneKey)
        {
            // 1. Загружаем сцену в фоне БЕЗ активации
            var handle = Addressables.LoadSceneAsync(
                sceneKey,
                LoadSceneMode.Single,
                activateOnLoad: false // Важно!
            );

            SceneStartLoad?.OnNext(Unit.Default);

            // 2. Ждем полной загрузки (до 90%)
            while (handle.PercentComplete < 0.9f)
            {
                SceneLoadUpdated?.OnNext(handle.PercentComplete);
                await UniTask.Yield();
            }

            var activationOp = handle.Result.ActivateAsync();
            activationOp.allowSceneActivation = false;

            SceneLoadUpdated?.OnNext(1f);

            await UniTask.Delay(5000);

            activationOp.allowSceneActivation = true;
            await activationOp; // Ждем реальной активации

            SceneEndLoad?.OnNext(Unit.Default);
        }

        public void UnloadScene()
        {
            if (_sceneHandle.IsValid())
            {
                Addressables.UnloadSceneAsync(_sceneHandle);
            }
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
