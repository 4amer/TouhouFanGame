using System.Collections.Generic;
using UnityEngine;

namespace Game.BulletSystem.Pool
{
    public class BulletPool : MonoBehaviour, IPool<Bullet>
    {
        private Dictionary<BulletTypes, Queue<Bullet>> _freeBullets = new Dictionary<BulletTypes, Queue<Bullet>>();
        public Transform Transform { get => transform; }
        public void Prepare(int amount, Bullet prefab)
        {
            for (int i = 0; amount >= i; i++)
            {
                Create(prefab);
            }
        }

        public Bullet Spawn(Bullet prefab, Transform parent, Vector3 position)
        {
            if(_freeBullets.ContainsKey(prefab.BulletTypes) == false)
            {
                _freeBullets[prefab.BulletTypes] = new Queue<Bullet>();
            }

            if (_freeBullets[prefab.BulletTypes].Count <= 0)
            {
                Create(prefab);
            }
            Bullet bullet = _freeBullets[prefab.BulletTypes].Dequeue();
            bullet.transform.parent = parent;
            bullet.transform.localPosition = position;
            bullet.gameObject.active = true;
            return bullet;
        }

        public void Release(Bullet bullet)
        {
            bullet.gameObject.active = false;
            bullet.transform.parent = Transform;
            _freeBullets[bullet.BulletTypes].Enqueue(bullet);
        }

        private void Create(Bullet prefab)
        {
            Bullet bullet = Instantiate(prefab, Transform);
            bullet.transform.localPosition = Vector3.zero;
            _freeBullets[bullet.BulletTypes].Enqueue(bullet);
        }
    }

    public interface IPool<T> where T : MonoBehaviour
    {
        public void Prepare(int amount, T prefab);
        public T Spawn(T prefab, Transform parent, Vector3 position);
        public Transform Transform { get; }
        public void Release(T obj);
    }
}
