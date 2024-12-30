using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Services.SceneLoader
{
    public class SceneLoader : ISceneLoader
    {
        public Subject<Unit> SceneStartLoad { get; set; } = new Subject<Unit>();
        public Subject<Unit> SceneLoadUpdated { get; set; } = new Subject<Unit>();
        public Subject<Unit> SceneEndLoad { get; set; } = new Subject<Unit>();

        public void LoadScene(string sceneName)
        {
            LoadSceneAsync(sceneName);
        }

        private async Task LoadSceneAsync(string sceneName)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

            SceneStartLoad?.OnNext(Unit.Default);

            while (operation!.isDone)
            {
                SceneLoadUpdated?.OnNext(Unit.Default);
                Debug.Log(operation.progress);
            }

            SceneEndLoad?.OnNext(Unit.Default);
        }
    }

    public interface ISceneLoader
    {
        public Subject<Unit> SceneStartLoad { get; set; }
        public Subject<Unit> SceneLoadUpdated { get; set; }
        public Subject<Unit> SceneEndLoad { get; set; }

        public void LoadScene(string sceneName);
    }
}
