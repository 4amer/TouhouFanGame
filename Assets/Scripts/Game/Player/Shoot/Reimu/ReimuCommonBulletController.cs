using UnityEngine;
using UnityEngine.Jobs;
using Unity.Burst;
using Zenject;
using Game;
using Cysharp.Threading.Tasks;
using UniRx;
using Game.BulletSystem;
using Unity.Collections;
using Unity.Mathematics;
using Game.BulletSystem.Pool;
using System.Collections.Generic;

namespace Player.Shoot.Reimu
{
    [BurstCompile]
    public class ReimuCommonBulletController : MonoBehaviour
    {
        [SerializeField] private float _speed = 10f;
        [SerializeField] private float _reloadDelay = 0.2f;
        [SerializeField] private int _maxBulletAmount = 20;

        private Bullet _bulletPrefab = null;

        private IBulletPool _bulletPool = null;

        private Utils.Timer timer = null;
        
        private CompositeDisposable _disposable = new CompositeDisposable();

        private Queue<Transform> bulletTransforms = null;
            
        private TransformAccessArray _transformAccessArray = default;

        private bool _isShootingAllowed = false;

        [Inject]
        private void Construct(IGameManager gameManager, IBulletPool bulletPool)
        {
            gameManager
                .Updated
                .Subscribe(_ => UpdateComponent(_))
                .AddTo(_disposable);

            _bulletPool = bulletPool;
        }

        public void Init(Bullet prefab)
        {
            _bulletPrefab = prefab;

            timer = new Utils.Timer();
            timer.duration = _reloadDelay;
            timer.timeStep = 0.1f;
            timer.EventOnFinish = Shoot;
            timer.Start();

            bulletTransforms = new Queue<Transform>();

            for (int i = 0; _maxBulletAmount >= i; i++)
            {
                CreateBullet();
            }

            _transformAccessArray = new TransformAccessArray(bulletTransforms.ToArray());
        }

        private void UpdateComponent(float delay)
        {
            var job = new CommonBullet
            {
                Speed = _speed,
                Delay = delay,
            };
            var handle = job.Schedule(_transformAccessArray);
            handle.Complete();
        }

        private void Shoot()
        {
            if (_isShootingAllowed == false) return;
            ReInitBullet();
            timer.Start();
        }

        public void AllowShooting()
        {
            Debug.Log("Allowed");
            _isShootingAllowed = true;
            Shoot();
        }

        public void StopShooting()
        {
            Debug.Log("Stop");
            _isShootingAllowed = false;
        }

        private void ReInitBullet()
        {
            Transform bulletTransform = bulletTransforms.Dequeue();
            bulletTransform.localPosition = Vector3.zero;
            bulletTransform.gameObject.SetActive(true);
            bulletTransforms.Enqueue(bulletTransform);
        }

        private void CreateBullet()
        {
            Bullet bullet = _bulletPool.Spawn(_bulletPrefab, this.transform, Vector3.zero);
            bullet.gameObject.SetActive(false);
            bulletTransforms.Enqueue(bullet.transform);
        }
    }

    [BurstCompile]
    public struct CommonBullet : IJobParallelForTransform
    {
        [ReadOnly] public float Speed;
        [ReadOnly] public float Delay;

        public void Execute(int index, TransformAccess transform)
        {
            float3 direction = math.normalize(new float3(-1,0,0));
            float3 newPosition = (float3)transform.localPosition - direction * Delay * Speed;
            transform.localPosition = newPosition;
        }
    }
}