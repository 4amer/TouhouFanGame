using UnityEngine;

namespace Player.Shoot
{
    public abstract class AShooting : MonoBehaviour
    {
        public abstract void Init();

        public abstract void UpdateComponents();

        public abstract void DoNormalShoot();

        public abstract void DoShiftShoot();

        public abstract void StopShoot();

        public abstract void IncreasePower(int powerLevel);
        public abstract void DecreasePower(int powerLevel);
    }
}