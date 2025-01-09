using Game.BulletSystem.Manager;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game
{
    public class GameManager : MonoBehaviour, IGameManager
    {
        public Subject<float> Updated { get; set; } = new Subject<float>();

        private IBulletComponentManager _bulletComponentManager = null;

        [Inject] 
        private void Construct(IBulletComponentManager bulletComponentManager)
        {
            _bulletComponentManager = bulletComponentManager;
        }

        private void Awake()
        {
            
        }

        private void Update()
        {
            Updated.OnNext(Time.deltaTime);
        }
    }

    public interface IGameManager
    {
        public Subject<float> Updated { get; set; }
    }
}
