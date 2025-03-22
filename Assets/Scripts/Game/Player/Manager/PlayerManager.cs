using Cysharp.Threading.Tasks;
using Player.Movement;
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
        public GameObject Player { get { return _player; } }
        public Transform PlayerTransform { get => _player.transform; }

        public void Init()
        {
            _playerMovemnt.Init(_player);
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