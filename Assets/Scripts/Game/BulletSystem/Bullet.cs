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
        [SerializeField] private SpriteRenderer _spriteRenderer = null;

        public float Damage => _damage;
        public float Range => _range;
        public BulletTypes BulletTypes { get { return _bulletType; } }
        public bool IsHided { get => _spriteRenderer.enabled; }
        public void PlayDestroyParticle()
        {
            _particleSystem.Play();
        }
        public void HideSprite()
        {
            _spriteRenderer.enabled = false;
        }

        public void ShowSprite()
        {
            _spriteRenderer.enabled = true;
        }
    }

    public enum BulletTypes
    {
        None = 0,
        Player = 1,
        Enemy = 2,
    }
}