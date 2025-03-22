using Game;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Player.Movement
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float _defaultSpeed = 5.0f;
        [SerializeField] private float _slowSpeed = 2.5f;

        private GameObject _playerObject = null;
        private Rigidbody _playerRigidBody = null;

        private PlayerInput _playerInput = null;

        private Vector2 _playerMovementDirection = Vector2.zero;
        private bool _isSlowed = false;

        private CompositeDisposable _disposable = new CompositeDisposable();

        [Inject]
        private void Construct(PlayerInput playerInput, IGameManager gameManager)
        {
            _playerInput = playerInput;

            gameManager
                .Updated
                .Subscribe(_ => UpdatePlayer(_))
                .AddTo(_disposable);
        }

        public void Init(GameObject player)
        {
            _playerObject = player;
            _playerRigidBody = player?.GetComponent<Rigidbody>();
        }

        private void UpdatePlayer(float delta)
        {
            float speed = _defaultSpeed;
            if (_isSlowed)
            {
                speed = _slowSpeed;
            }

            Vector3 playerPosition = _playerObject.transform.position;
            Vector3 direction = new Vector3(_playerMovementDirection.x, _playerMovementDirection.y, 0).normalized;
            _playerRigidBody.MovePosition(playerPosition + direction * speed * delta);
        }

        private void ChageMovementDirection(InputAction.CallbackContext context)
        {
            Vector2 direction = context.ReadValue<Vector2>();

            _playerMovementDirection = direction;
        }

        private void SlowMovement(bool isSlowed)
        {
            _isSlowed = isSlowed;
        }

        private void OnEnable()
        {
            InputActionMap map = _playerInput.actions.FindActionMap("GamePlay");
            InputAction movementAction = map["PlayerMovement"];
            InputAction slowMovementAction = map["SlowMovement"];

            movementAction.started += ctx => ChageMovementDirection(ctx);
            movementAction.performed += ctx => ChageMovementDirection(ctx);
            movementAction.canceled += ctx => ChageMovementDirection(ctx);

            slowMovementAction.started += ctx => SlowMovement(true);
            slowMovementAction.canceled += ctx => SlowMovement(false);
        }

        private void OnDisable()
        {
            InputActionMap map = _playerInput.actions.FindActionMap("GamePlay");
            InputAction movementAction = map["PlayerMovement"];
            InputAction slowMovementAction = map["SlowMovement"];

            movementAction.started -= ctx => ChageMovementDirection(ctx);
            movementAction.performed -= ctx => ChageMovementDirection(ctx);
            movementAction.canceled -= ctx => ChageMovementDirection(ctx);

            slowMovementAction.started -= ctx => SlowMovement(true);
            slowMovementAction.canceled -= ctx => SlowMovement(false);
        }

        private void OnDestroy()
        {
            _disposable?.Clear();
        }
    }
}
