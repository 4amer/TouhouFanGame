using Game.BulletSystem.Manager;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Game
{
    public class GameManager : MonoBehaviour, IGameManager, IGameManagerInit
    {
        public Subject<float> Updated { get; set; } = new Subject<float>();
        public Subject<float> LateUpdated { get; set; } = new Subject<float>();
        public Subject<float> FixedUpdated { get; set; } = new Subject<float>();

        public void Init()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            Updated.OnNext(Time.deltaTime);
        }

        private void LateUpdate()
        {
            LateUpdated.OnNext(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            FixedUpdated.OnNext(Time.deltaTime);
        }
    }

    internal interface IGameManagerInit
    {
        public void Init();
    }

    public interface IGameManager
    {
        public Subject<float> Updated { get; set; }
        public Subject<float> LateUpdated { get; set; }
        public Subject<float> FixedUpdated { get; set; }
    }
}
