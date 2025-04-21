using UnityEngine;

namespace Game.BackGround.Manager
{
    public class BGManager : MonoBehaviour, IBGManager
    {
        [SerializeField] private ParalaxManager[] _paralaxManagers = new ParalaxManager[1];

        public ParalaxManager[] GetParalaxManagers { get => _paralaxManagers; }
    }

    internal interface IBGManager
    {
        public ParalaxManager[] GetParalaxManagers { get; }
    }
}
