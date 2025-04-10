using DG.Tweening;
using Game.BulletSystem;
using UnityEngine;

namespace Player.Shoot.Reimu
{
    public class ReimuShoot : AShooting
    {

        [SerializeField] private Bullet _commonBullet = null;
        [SerializeField] private Bullet _autoAimBullet = null;

        [SerializeField] private ReimuCommonBulletController[] _commonBulletControllers = new ReimuCommonBulletController[1];

        [Space(10)]
        [Header("Spining Objects")]
        [SerializeField] private Transform _rotationObjTransform = null;
        [SerializeField] private float _spiningSpeed = 1f;

        [Space(10)]
        [SerializeField] private ReimuAutoAimBulletController[] _autoAimBulletControllers = new ReimuAutoAimBulletController[1];

        private int _currentPowerLevel = 0;

        public override void Init()
        {
            foreach (ReimuCommonBulletController controller in _commonBulletControllers)
            {
                controller.Init(_commonBullet);
            }

            foreach (ReimuAutoAimBulletController controller in _autoAimBulletControllers)
            {
                controller.Init(_autoAimBullet);
            }

            SetupSpiningObject();
        }

        private void SetupSpiningObject()
        {
            Vector3 middlePosition = new Vector3(180, 0, 0);
            Vector3 finishPosition = new Vector3(180, 0, 0);

            Sequence sequence = DOTween.Sequence();

            sequence.Append(_rotationObjTransform
                .DOBlendableLocalRotateBy(middlePosition, _spiningSpeed)
                .SetEase(Ease.Linear))
                .Append(_rotationObjTransform
                .DOBlendableLocalRotateBy(finishPosition, _spiningSpeed)
                .SetEase(Ease.Linear))
                .SetLoops(-1);
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

            foreach (ReimuAutoAimBulletController controller in _autoAimBulletControllers)
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
            foreach (ReimuAutoAimBulletController controller in _autoAimBulletControllers)
            {
                controller.StopShooting();
            }
        }

        public override void DoShiftShoot()
        {

        }

        public override void DecreasePower(int powerLevel)
        {
            for (int i = 0; powerLevel >= i; i++)
            {
                _autoAimBulletControllers[i].Hide();
            }
        }

        public override void IncreasePower(int powerLevel)
        {
            for (int i = 0; powerLevel >= i; i++)
            {
                _autoAimBulletControllers[i].Show();
            }
        }
    }
}