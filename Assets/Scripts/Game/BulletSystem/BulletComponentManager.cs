using UnityEngine;


namespace Game.BulletSystem.Manager
{
    public class BulletComponentManager : MonoBehaviour, IBulletComponentManager
    {
        [SerializeField] private GameObject _player = null;
    
        public void Init()
        {

        }
    }
    public interface IBulletComponentManager
    {
        void Init();
    }
}
