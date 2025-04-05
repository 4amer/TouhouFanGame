using Cysharp.Threading.Tasks;
using Game;
using Stages.Manager;
using UniRx;
using UnityEngine;
using Zenject;

namespace Enemies.Drop
{
    public class DropItem : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 1.0f;

        [SerializeField] private Rigidbody _rigidbody = null;

        private CompositeDisposable _disposable = new CompositeDisposable();

        [Inject]
        private void Construct(IGameManager gameManager)
        {
            gameManager
                .Updated
                .Subscribe(_ => ComponentUpdate(_))
                .AddTo(_disposable);
        }

        public void Init()
        {
            if (_rigidbody == null) 
            {
                _rigidbody = GetComponent<Rigidbody>();
            }
        }

        private void ComponentUpdate(float delta)
        {
            _rigidbody.MovePosition(transform.position + Vector3.right * _moveSpeed * delta);
        }
    }
}