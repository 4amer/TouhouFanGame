using UnityEngine;

namespace Game.Player.Manager
{
    public class PlayerManager : MonoBehaviour, IPlayerManager
    {
        [SerializeField] private GameObject _player = null;

        public GameObject Player { get { return _player; } }

        public void Init()
        {

        }
    }

    public interface IPlayerManager
    {
        public void Init();
        public GameObject Player { get; }
    }
}