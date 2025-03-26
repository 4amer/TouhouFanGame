using System.Collections.Generic;
using UnityEngine;

namespace Game.BulletSystem.Pool
{
    public class BulletPool : MonoBehaviour, IBulletPool
    {
        private Dictionary<BulletTypes, Queue<Bullet>> _freeBullets = new Dictionary<BulletTypes, Queue<Bullet>>();

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
            bullet.transform.parent = transform;
            _freeBullets[bullet.BulletTypes].Enqueue(bullet);
        }

        private void Create(Bullet prefab)
        {
            Bullet bullet = Instantiate(prefab, transform);
            bullet.transform.localPosition = Vector3.zero;
            _freeBullets[bullet.BulletTypes].Enqueue(bullet);
        }
    }

    public interface IBulletPool
    {
        public void Prepare(int amount, Bullet prefab);
        public Bullet Spawn(Bullet prefab, Transform parent, Vector3 position);
        public void Release(Bullet bullet);
    }
}
