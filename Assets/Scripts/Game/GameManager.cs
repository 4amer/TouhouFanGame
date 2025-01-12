using Game.BulletSystem.Manager;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game
{
    public class GameManager : MonoBehaviour, IGameManager
    {
        public Subject<float> Updated { get; set; } = new Subject<float>();

        public void Init()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            Updated.OnNext(Time.deltaTime);
        }
    }

    public interface IGameManager
    {
        public void Init();
        public Subject<float> Updated { get; set; }
    }
}
