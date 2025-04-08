using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game;
using UniRx;
using UnityEngine;
using Zenject;

namespace Enemies.Drop
{
    public class DropItem : MonoBehaviour
    {
        [SerializeField] private float _standartMoveSpeed = 1.0f;
        private float _changableSpeed = 0f;
        
        [SerializeField] private float _timerToPickUp = 1f;
        private float _cangableTimer = 0;

        [SerializeField] private Rigidbody _rigidbody = null;

        private Vector3 _moveToPosition = Vector3.left;

        private CompositeDisposable _disposable = new CompositeDisposable();

        public Subject<DropItem> PickedUp = new Subject<DropItem>();

        private bool _doPickUp = false;

        private Transform _playerTransform = null;
        private Vector3 _pickedUpPosition = Vector3.zero;

        private IGameManager _gameManager = null;

        [Inject]
        private void Construct(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void Init()
        {
            _gameManager
                .Updated
                .Subscribe(_ => ComponentUpdate(_))
                .AddTo(_disposable);

            _changableSpeed = _standartMoveSpeed;
            _cangableTimer = 0;

            if (_rigidbody == null) 
            {
                _rigidbody = GetComponent<Rigidbody>();
            }
        }

        private void ComponentUpdate(float delta)
        {
            if (_doPickUp == true)
            {
                PickUpEaseInBack(delta);
            } else
            {
                _rigidbody.MovePosition(transform.position + _moveToPosition * _changableSpeed * delta);
            }
        }

        private void PickUp(Transform playerTransform)
        {
            _playerTransform = playerTransform;
            _pickedUpPosition = transform.position;
            _doPickUp = true;
        }

        private void PickUpEaseInBack(float delta)
        {
            _cangableTimer += delta;

            float t = Mathf.Clamp01(_cangableTimer / _timerToPickUp);

            t = EaseInBack(t);

            transform.position = Vector3.LerpUnclamped(_pickedUpPosition, _playerTransform.position, t);
            
            if(t > 0.99)
            {
                Disable();
            }
        }

        private float EaseInBack(float t)
        {
            const float c1 = 3.0f;
            const float c3 = c1 + 1;

            return c3 * t * t * t - c1 * t * t;
        }

        private void Disable()
        {
            _disposable?.Clear();
            PickedUp?.OnNext(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("PickUpRange"))
            {
                PickUp(other.transform);
            }
        }
    }
}