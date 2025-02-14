using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Zenject;

namespace Services.SceneLoaderC
{
    public class SceneLoader : ISceneLoader
    {
        public Subject<Unit> SceneStartLoad { get; set; } = new Subject<Unit>();
        public Subject<float> SceneLoadUpdated { get; set; } = new Subject<float>();
        public Subject<Unit> SceneEndLoad { get; set; } = new Subject<Unit>();

        private AsyncOperationHandle<SceneInstance> _sceneHandle;

        public void LoadScene(string sceneKey)
        {
            LoadSceneAsync(sceneKey);
        }

        private async Task LoadSceneAsync(string sceneKey)
        {
            _sceneHandle = Addressables.LoadSceneAsync(sceneKey, LoadSceneMode.Single, activateOnLoad: false);

            SceneStartLoad?.OnNext(Unit.Default);

            while (!_sceneHandle.IsDone)
            {
                float progress = _sceneHandle.PercentComplete;
                SceneLoadUpdated?.OnNext(progress);
                Debug.Log($"Loading progress: {progress:P}");

                await Task.Yield();
            }

            if (_sceneHandle.Status == AsyncOperationStatus.Succeeded)
            {
                await _sceneHandle.Result.ActivateAsync();
                SceneEndLoad?.OnNext(Unit.Default);
            } 
            else
            {
                Debug.LogError($"Failed to load scene: {sceneKey}");
            }
        }

        public void UnloadScene()
        {
            if (_sceneHandle.IsValid())
            {
                Addressables.UnloadSceneAsync(_sceneHandle);
            }
        }
    }

    public interface ISceneLoader
    {
        public Subject<Unit> SceneStartLoad { get; set; }
        public Subject<float> SceneLoadUpdated { get; set; }
        public Subject<Unit> SceneEndLoad { get; set; }

        public void LoadScene(string sceneName);
        public void UnloadScene();
    }
}
