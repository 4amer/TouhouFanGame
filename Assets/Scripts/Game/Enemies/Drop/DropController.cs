using System;
using Game.BulletSystem.Damage;
using Game.BulletSystem.Pool;
using PoolService.Pool;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

namespace Enemies.Drop
{
    public class DropController : MonoBehaviour
    {
        [SerializeField] private DropProperty[] _dropProperties = new DropProperty[1];

        private IPool<DropItem> _dropPool = null;

        private CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        private void Construct(IPool<DropItem> dropPool)
        {
            _dropPool = dropPool;
        }

        public void Init(IDamagable damagable)
        {
            damagable
                .OnDead
                .Subscribe(_ => Drop(_))
                .AddTo(_disposables);
        }

        private void Drop(IDamagable damagable)
        {
            Transform transform = damagable.Transform;

            foreach (DropProperty dropProperty in _dropProperties)
            {
                float diameter = dropProperty.rangeToDrop / 2f;

                for (int i = 0; i < dropProperty.amount; i++) { 
                
                    float posX = UnityEngine.Random.Range(diameter * -1, diameter);
                    float posY = UnityEngine.Random.Range(diameter * -1, diameter);

                    Vector3 position = new Vector3(posX, posY, 0);

                    _dropPool.Spawn(dropProperty.dropItem, transform, position);
                }
            }
        }
    }

    [Serializable]
    public class DropProperty
    {
        [SerializeField] public DropItem dropItem = null;
        [SerializeField] public float rangeToDrop = 0f;
        [SerializeField] public int amount = 1;
    }
}