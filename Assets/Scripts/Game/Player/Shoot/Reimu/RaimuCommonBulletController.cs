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

namespace Player.Shoot.Reimu
{
    [BurstCompile]
    public class RaimuCommonBulletController : MonoBehaviour
    {
        [SerializeField] private float _speed = 10f;

        private CompositeDisposable _disposable = new CompositeDisposable();

        [Inject]
        private void Construct(IGameManager gameManager)
        {
            gameManager
                .Updated
                .Subscribe(_ => UpdateComponent(_))
                .AddTo(_disposable);
        }

        public void Init()
        {

        }

        private void UpdateComponent(float delay)
        {

        }
    }

    [BurstCompile]
    public struct CommonBullet : IJobParallelForTransform
    {
        [ReadOnly] private float Speed;
        [ReadOnly] private float Delay;

        public void Execute(int index, TransformAccess transform)
        {
            float3 direction = math.normalize((float3)Vector3.right - (float3)transform.position);
            float3 newPosition = (float3)transform.position - direction * Delay * Speed;
            transform.position = newPosition;
        }
    }
}