using System.Threading.Tasks;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Services.SceneLoaderC
{
    public class SceneLoader : ISceneLoader
    {
        public Subject<Unit> SceneStartLoad { get; set; } = new Subject<Unit>();
        public Subject<float> SceneLoadUpdated { get; set; } = new Subject<float>();
        public Subject<Unit> SceneEndLoad { get; set; } = new Subject<Unit>();

        public void LoadScene(string sceneName)
        {
            LoadSceneAsync(sceneName);
            Debug.Log("Test 1");
        }

        private async Task LoadSceneAsync(string sceneName)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;

            SceneStartLoad?.OnNext(Unit.Default);

            while (!operation.isDone)
            {
                float progress = (operation.progress / 0.9f);
                SceneLoadUpdated?.OnNext(progress);
                Debug.Log(progress);
                if (progress >= 0.9f)
                {
                    operation.allowSceneActivation = true;
                }

                await Task.Yield();
            }

            SceneEndLoad?.OnNext(Unit.Default);
        }
    }

    public interface ISceneLoader
    {
        public Subject<Unit> SceneStartLoad { get; set; }
        public Subject<float> SceneLoadUpdated { get; set; }
        public Subject<Unit> SceneEndLoad { get; set; }

        public void LoadScene(string sceneName);
    }
}
