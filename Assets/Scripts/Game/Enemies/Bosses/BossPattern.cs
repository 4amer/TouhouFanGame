using Audio;
using Audio.Types;
using BezierMovementSystem;
using Cysharp.Threading.Tasks;
using Enemies.Manager;
using Game.Player.Manager;
using UniRx;
using UnityEngine;
using Utils;
using Zenject;

namespace Enemies.Bosses.Attack
{
    public class BossPattern : MonoBehaviour
    {
        [SerializeField] private EntityController[] _entityControllers = null;
        [SerializeField] private MovementComponent _bossMovement = null;
        [SerializeField] private float _timeForPreparePosition = 1f;

        private Transform _playerTransform = null;

        private DiContainer _diContainer = null;

        private IAudioManager _audioManager = null;

        private Utils.Timer _timeBeforeStart = null;

        private CompositeDisposable _disposable = new CompositeDisposable();

        [Inject]
        private void Construct(DiContainer diContainer, IPlayerManagerTransform playerManagerTransform, IAudioManager audioManager)
        {
            _diContainer = diContainer;
            _playerTransform = playerManagerTransform.PlayerTransform;
            _audioManager = audioManager;
        }

        public void Init(GameObject boss)
        {
            _timeBeforeStart = new Utils.Timer();
            _timeBeforeStart.duration = _timeForPreparePosition;

            _bossMovement.Init(boss);
            _bossMovement.PrepareStartPosition(_timeForPreparePosition);
            _bossMovement.MoveCompleted
                .Subscribe(_ =>
                {
                    _audioManager.Play(ESFXTypes.Explode);
                    _disposable.Clear();
                })
                .AddTo(_disposable);

            _timeBeforeStart.OnTimerFinish += () =>
            {
                foreach (EntityController entity in _entityControllers)
                {
                    _diContainer.Inject(entity);
                    entity.Init(_playerTransform, boss);
                }
            };

            _timeBeforeStart.Start();
        }

        private void MovementEnded()
        {

        }

        public void Clear()
        {
            foreach (IEntityController entityController in _entityControllers)
            {
                entityController.Dispose();
            }
        }
    }
}
