using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Game.BulletSystem
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float _damage = 1f;
        [SerializeField] private float _range = 1f;
        [SerializeField] private BulletTypes _bulletType = BulletTypes.None;
        [SerializeField] private ParticleSystem _particleSystem = null;

        public float Damage => _damage;
        public float Range => _range;
        public BulletTypes BulletTypes { get { return _bulletType; } }
        public void PlayDestroyParticle()
        {
            _particleSystem.Play();
        }
    }

    public enum BulletTypes
    {
        None = 0,
        Player = 1,
        Enemy = 2,
    }
}