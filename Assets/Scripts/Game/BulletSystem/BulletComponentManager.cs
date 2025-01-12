using Game.Player.Manager;
using UniRx;
using UnityEngine;
using Zenject;


namespace Game.BulletSystem.Manager
{
    public class BulletComponentManager : MonoBehaviour, IBulletComponentManager
    {
        [SerializeField] private BulletComponent BulletComponent;

        private Transform _playersTransform = null;
        private IGameManager _gameManager = null;

        private CompositeDisposable _disposable = new CompositeDisposable();

        [Inject]
        private void Constract(IPlayerManager playerManager, IGameManager gameManager)
        {
            _playersTransform = playerManager.Player.transform;
            _gameManager = gameManager;
        }

        public void Init()
        {
            BulletComponent.Init(_playersTransform);

            _gameManager.Updated
                .Subscribe(_ => BulletComponent.UpdateComponent(_))
                .AddTo(_disposable);
        }
    }
    public interface IBulletComponentManager
    {
        void Init();
    }
}
