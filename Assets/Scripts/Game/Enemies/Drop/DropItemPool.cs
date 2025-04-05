using System.Collections.Generic;
using Game.BulletSystem.Pool;
using PoolService.Pool;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Enemies.Drop
{
    public class DropItemPool : MonoBehaviour, IPool<DropItem>
    {
        private Dictionary<string, Queue<DropItem>> _freeItems = new Dictionary<string, Queue<DropItem>>();

        public Transform Transform { get => transform; }

        [Inject]
        private void Construct()
        {

        }

        public void Prepare(int amount, DropItem prefab)
        {
            for (int i = 0; amount >= i; i++)
            {
                Create(prefab);
            }
        }

        public DropItem Spawn(DropItem prefab, Transform parent, Vector3 position)
        {
            if (_freeItems.ContainsKey(prefab.name) == false)
            {
                _freeItems[prefab.name] = new Queue<DropItem>();
            }

            if (_freeItems[prefab.name].Count <= 0)
            {
                Create(prefab);
            }
            DropItem item = _freeItems[prefab.name].Dequeue();
            item.transform.parent = parent;
            item.transform.localPosition = position;
            item.gameObject.active = true;
            return item;
        }

        public void Release(DropItem item)
        {
            item.gameObject.active = false;
            item.transform.parent = transform;
            _freeItems[item.name].Enqueue(item);
        }

        private void Create(DropItem prefab)
        {
            DropItem item = Instantiate(prefab, transform);
            item.transform.localPosition = Vector3.zero;
            _freeItems[item.name].Enqueue(item);
        }
    }
}