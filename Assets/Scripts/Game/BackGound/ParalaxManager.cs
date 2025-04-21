using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Game.BackGround.Manager
{
    public class ParalaxManager : MonoBehaviour, IParalaxManager
    {
        [SerializeField] private float _speedToMove = 0f;
        [SerializeField] private float _length = 0f;
        [SerializeField] private SpriteRenderer _spriteRenderer = null;

        private float _startXPosition = 0f;
        private Camera _camera = null;

        private GameManager _gameManager = null;

        private CompositeDisposable _disposable = new CompositeDisposable();

        [Inject]
        private void Construct(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void Init()
        {
            _startXPosition = transform.position.x;
            _camera = Camera.main;

            if (_length == 0f)
            {
                _length = _spriteRenderer.bounds.size.x;
            }

            _gameManager
                .FixedUpdated
                .Subscribe(_ => ComponentFixedUpdate())
                .AddTo(_disposable);
        }

        private void ComponentFixedUpdate()
        {
            Vector3 position = transform.position;
            transform.position = new Vector3(position.x - _speedToMove * Time.deltaTime, position.y, position.z);

            if (transform.position.x <= _startXPosition - _length)
            {
                transform.position = new Vector3(transform.position.x + _length, position.y, position.z);
            }
        }

        private void OnDestroy()
        {
            _disposable?.Clear();
        }
    }

    public interface IParalaxManager
    {
        public void Init();
    }
}
