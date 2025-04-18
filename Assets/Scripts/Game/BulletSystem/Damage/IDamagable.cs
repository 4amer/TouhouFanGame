using System;
using UniRx;
using UnityEngine;

namespace Game.BulletSystem.Damage
{
    public interface IDamagable
    {
        public Subject<IDamagable> OnDead { get; set; }
        public Subject<float> OnDamaged { get; set; }
        public Transform Transform { get; }
        public float RangeToCollide { get; }
        public bool IsVulnerable { get; set; }
        public void Damage(float damage);
    }
}
