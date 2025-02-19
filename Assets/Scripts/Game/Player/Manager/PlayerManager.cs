using UnityEngine;

namespace Game.Player.Manager
{
    public class PlayerManager : MonoBehaviour, IPlayerManager, IPlayerManagerTransform
    {
        [SerializeField] private GameObject _player = null;

        public GameObject Player { get { return _player; } }
        public Transform PlayerTransform { get => _player.transform; }

        public void Init()
        {

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