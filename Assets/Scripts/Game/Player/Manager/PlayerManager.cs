using Cysharp.Threading.Tasks;
using Game.Player.Money;
using Player.Collision;
using Player.Health;
using Player.Movement;
using Player.Shoot;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game.Player.Manager
{
    public class PlayerManager : MonoBehaviour, IPlayerManager, IPlayerManagerTransform
    {
        [SerializeField] private GameObject _player = null;

        [SerializeField] private PlayerMovement _playerMovemnt = null;
        [SerializeField] private PlayerHealth _playerHealth = null;
        [SerializeField] private PlayerCollision _playerCollision = null;
        [SerializeField] private PlayerShoot _playerShoot = null;
        [SerializeField] private PlayerMoney _playerMoney = null;
        public GameObject Player { get { return _player; } }
        public Transform PlayerTransform { get => _player.transform; }

        public CompositeDisposable _disposable = new CompositeDisposable();
        public void Init()
        {
            _playerMovemnt.Init(_player);

            _playerHealth.Init(_player);

            _playerCollision
                .PlayerCollided
                .Subscribe(_ => Damage())
                .AddTo(_disposable);

            _playerShoot.Init();

            _playerMoney.Init();
        }

        public void Damage()
        {
            _playerHealth.DoDamage();
        }
    }

    public interface IPlayerManagerTransform
    {
        public Transform PlayerTransform { get; }
    }

    public interface IPlayerManager
    {
        public void Init();
        public GameObject Player { get; }
    }
}