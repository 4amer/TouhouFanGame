using Cysharp.Threading.Tasks;
using Game.Player.Ability;
using Game.Player.Money;
using Player.Collision;
using Player.Health;
using Player.Movement;
using Player.Shoot;
using UI;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game.Player.Manager
{
    public class PlayerManager : MonoBehaviour, IPlayerManager, IPlayerManagerTransform, IPlayerManagerActions
    {
        [SerializeField] private GameObject _player = null;

        [SerializeField] private PlayerMovement _playerMovemnt = null;
        [SerializeField] private PlayerHealth _playerHealth = null;
        [SerializeField] private PlayerCollision _playerCollision = null;
        [SerializeField] private PlayerShoot _playerShoot = null;
        [SerializeField] private PlayerMoney _playerMoney = null;

        [Space(10)]
        [Header("Managers")]
        [SerializeField] private AbilityManager _abilityManager = null;
        public GameObject Player { get { return _player; } }
        public Transform PlayerTransform { get => _player.transform; }

        public CompositeDisposable _disposable = new CompositeDisposable();

        public IUIManager _uIManager = null;

        public Subject<UniRx.Unit> OnPlayerDead { get; set; } = new Subject<UniRx.Unit>();

        [Inject]
        private void Construct(IUIManager uIManager)
        {
            _uIManager = uIManager;
        }

        public void Init()
        {
            _abilityManager.Init(PlayerTransform);

            _playerMovemnt.Init(_player);

            _playerHealth.Init(_player);

            _playerCollision
                .PlayerCollided
                .Subscribe(_ => Damage())
                .AddTo(_disposable);

            _playerHealth
                .PlayerDead
                .Subscribe(_ => PlayerDead())
                .AddTo(_disposable);

            _playerShoot.Init();

            _playerMoney.Init();
        }

        public void Damage()
        {
            _playerHealth.DoDamage();
        }

        private void PlayerDead()
        {
            OnPlayerDead?.OnNext(UniRx.Unit.Default);
            _disposable?.Clear();
            _disposable?.Dispose();
        }
    }

    public interface IPlayerManagerTransform
    {
        public Transform PlayerTransform { get; }
    }
    
    public interface IPlayerManagerActions
    {
        public Subject<UniRx.Unit> OnPlayerDead { get; set; }
    }

    public interface IPlayerManager
    {
        public void Init();
        public GameObject Player { get; }
    }
}