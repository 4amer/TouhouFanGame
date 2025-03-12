using BezierMovementSystem;
using Enemies.Manager;
using Game.Player.Manager;
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

        private Utils.Timer _timeBeforeStart = null;

        [Inject]
        private void Construct(DiContainer diContainer, IPlayerManagerTransform playerManagerTransform)
        {
            _diContainer = diContainer;
            _playerTransform = playerManagerTransform.PlayerTransform;
        }

        public void Init(GameObject boss)
        {
            _timeBeforeStart = new Utils.Timer();
            _timeBeforeStart.duration = _timeForPreparePosition;

            _bossMovement.Init(boss);
            _bossMovement.PrepareStartPosition(_timeForPreparePosition);

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

        public void Clear()
        {
            foreach (IEntityController entityController in _entityControllers)
            {
                entityController.Dispose();
            }
        }
    }
}
