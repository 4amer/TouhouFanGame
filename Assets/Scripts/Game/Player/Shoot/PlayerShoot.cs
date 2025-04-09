using Enemies.Drop;
using Player.Collision;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Player.Shoot
{
    public class PlayerShoot : MonoBehaviour, IPlayerShootActions
    {
        [SerializeField] private AShooting AShooting;

        [SerializeField] private int PowerToNextLevel = 100;
        [SerializeField] private int MaxPowerStage = 4;

        [Space(10)]
        [Header("Power values")]
        [SerializeField] private int _smallPower = 3;
        [SerializeField] private int _bigPower = 10;


        [Space(10)]
        [Header("Item Collision")]
        [SerializeField] private PlayerCollision _playerCollision = null;

        private int _currentPowerAmount = 0;
        private int _currentPowerStage = 0;

        private bool _isShifting = false;

        private PlayerInput _playerInput = null;

        public Subject<int> PowerAmountChanged { get; set; } = new Subject<int>();
        public Subject<int> PowerStageChanged { get; set; } = new Subject<int>();

        private CompositeDisposable _compositeDisposable = new CompositeDisposable();   

        [Inject]
        private void Construct(PlayerInput playerInput)
        {
            _playerInput = playerInput;
        }

        public void Init()
        {
            AShooting.Init();

            _playerCollision
                .PlayerCollided
                .Subscribe(_ => CheckItem(_))
                .AddTo(_compositeDisposable);
        }

        private void Shoot()
        {
            if (_isShifting)
            {
                AShooting.DoShiftShoot();
            }
            else
            {
                AShooting.DoNormalShoot();
            }
        }

        private void StopShoot()
        {
            AShooting.StopShoot();
        }

        private void Shifting(bool shift)
        {
            _isShifting = shift;
        }

        public void AddPower(int amount)
        {
            _currentPowerAmount += amount;
            if(_currentPowerAmount >= PowerToNextLevel)
            {
                if (_currentPowerStage >= MaxPowerStage)
                {
                    _currentPowerAmount = (PowerToNextLevel - 1);
                    PowerAmountChanged?.OnNext(_currentPowerAmount);
                    return;
                }
                _currentPowerAmount -= PowerToNextLevel;
                _currentPowerStage += 1;
                PowerStageChanged?.OnNext(_currentPowerStage);
                AShooting.IncreasePower(_currentPowerStage);
            }
            PowerAmountChanged?.OnNext(_currentPowerAmount);
        }

        public void DecreasePower()
        {
            _currentPowerStage -= 1;
            PowerStageChanged?.OnNext(_currentPowerStage);
            AShooting.DecreasePower(_currentPowerStage);
        }

        private void CheckItem(GameObject item)
        {
            DropItem dropItem = item.GetComponent<DropItem>();

            if (dropItem == null) return;

            switch (dropItem.Type)
            {
                case EDropItemType.PowerSmall:
                    AddPower(_smallPower);
                    break;

                case EDropItemType.PowerBig:
                    AddPower(_bigPower);
                    break;
            }
        }

        private void OnEnable()
        {
            InputActionMap map = _playerInput.actions.FindActionMap("GamePlay");
            InputAction shoot = map["Shoot"];
            InputAction shift = map["SlowMovement"];

            shoot.started += ctx => Shoot();
            shoot.canceled += ctx => StopShoot();

            shift.started += ctx => Shifting(true);
            shift.canceled += ctx => Shifting(false);
        }

        private void OnDisable()
        {
            InputActionMap map = _playerInput.actions.FindActionMap("GamePlay");
            InputAction shoot = map["Shoot"];
            InputAction shift = map["SlowMovement"];

            shoot.started -= ctx => Shoot();
            shoot.canceled -= ctx => StopShoot();

            shift.started -= ctx => Shifting(true);
            shift.canceled -= ctx => Shifting(false);
        }

        private void OnDestroy()
        {
            _compositeDisposable?.Clear();
        }
    }

    public interface IPlayerShootActions
    {
        public Subject<int> PowerAmountChanged { get; set; }    
        public Subject<int> PowerStageChanged { get; set; }
    }
}