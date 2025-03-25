using UnityEngine;
using Unity.Mathematics;
using Unity.Jobs;
using UnityEngine.Jobs;
using Unity.Burst;
using Zenject;
using Game;
using Cysharp.Threading.Tasks;
using UniRx;

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
        public void Execute(int index, TransformAccess transform)
        {

        }
    }
}