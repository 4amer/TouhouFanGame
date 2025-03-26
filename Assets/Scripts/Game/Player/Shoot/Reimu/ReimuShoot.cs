using Game.BulletSystem;
using UnityEngine;

namespace Player.Shoot.Reimu
{
    public class ReimuShoot : AShooting
    {

        [SerializeField] private Bullet _commonBullet = null;

        [SerializeField] private ReimuCommonBulletController[] _commonBulletControllers = new ReimuCommonBulletController[1];
        public override void Init()
        {
            foreach (ReimuCommonBulletController controller in _commonBulletControllers)
            {
                controller.Init(_commonBullet);
            }
        }

        public override void UpdateComponents()
        {

        }

        public override void DoNormalShoot()
        {
            foreach (ReimuCommonBulletController controller in _commonBulletControllers)
            {
                controller.AllowShooting();
            }
        }
        public override void StopShoot()
        {
            foreach (ReimuCommonBulletController controller in _commonBulletControllers)
            {
                controller.StopShooting();
            }
        }

        public override void DoShiftShoot()
        {

        }

        public override void DecreaseDamage()
        {

        }

        public override void IncreaseDamage()
        {

        }
    }
}