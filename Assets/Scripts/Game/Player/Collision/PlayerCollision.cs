using System.Collections;
using System.Collections.Generic;
using Game.BulletSystem.Damage;
using UniRx;
using UnityEngine;

namespace Player.Collision
{
    public class PlayerCollision : MonoBehaviour
    {
        public Subject<GameObject> PlayerCollided = new Subject<GameObject>();
        [SerializeField] private string _tagName = string.Empty;

        private void OnCollisionEnter(UnityEngine.Collision collision)
        {
            GameObject gameObject = collision.gameObject;
            if (gameObject.CompareTag(_tagName))
            {
                PlayerCollided?.OnNext(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            GameObject gameObject = other.gameObject;
            if (gameObject.CompareTag(_tagName))
            {
                PlayerCollided?.OnNext(gameObject);
            }
        }

        private void OnParticleCollision(GameObject other)
        {
            GameObject gameObject = other.gameObject;
            if (gameObject.CompareTag(_tagName))
            {
                PlayerCollided?.OnNext(gameObject);
            }
        }
    }
}