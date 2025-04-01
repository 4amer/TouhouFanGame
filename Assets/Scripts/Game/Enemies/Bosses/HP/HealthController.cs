using Game.BulletSystem.Damage;
using UniRx;
using UnityEngine;

namespace Enemies.Bosses.HP
{
    public class HealthController : MonoBehaviour, IInitHealthController, IHealthController
    {
        private float _staticHP = 0f;
        private float _changableHP = 0f;

        public Subject<float> HPChanged { get; set; } = new Subject<float>();
        public Subject<float> StaticHPChanged { get; set; } = new Subject<float>();
        public Subject<Unit> OnDead { get; set; } = new Subject<Unit>();

        private CompositeDisposable disposables = new CompositeDisposable();

        public void Init(IDamagable damagable, float hp)
        {
            ChangeAndRestoreHP(hp);

            damagable
                .OnDamaged
                .Subscribe(_ => ChangeHP(_))
                .AddTo(disposables);
        }

        public void ChangeAndRestoreHP(float hp)
        {
            _staticHP = hp;
            _changableHP = hp;
            StaticHPChanged?.OnNext(_staticHP);
        }

        private void ChangeHP(float damage)
        {
            _changableHP -= damage;
            if(_changableHP <= 0)
            {
                _changableHP = 0;
                Dead();
            }
            HPChanged?.OnNext(_changableHP);
        }

        private void Dead()
        {
            OnDead?.OnNext(Unit.Default);
        }
    }

    public interface IInitHealthController
    {
        public void Init(IDamagable damagable, float hp);
    }

    public interface IHealthController
    {
        public Subject<float> HPChanged { get; set; }
        public Subject<float> StaticHPChanged { get; set; }
        public Subject<Unit> OnDead { get; set; }
        public void ChangeAndRestoreHP(float hp);
    }
}

