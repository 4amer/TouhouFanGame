using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Player.Shoot
{
    public class PlayerShoot : MonoBehaviour
    {
        [SerializeField] private AShooting AShooting;

        [SerializeField] private int PowerToNextLevel = 100;
        [SerializeField] private int MaxPowerStage = 4;

        private int _currentPowerAmount = 0;
        private int _currentPowerStage = 0;

        private bool _isShifting = false;

        private PlayerInput _playerInput = null;

        public Subject<int> PowerAmountChanged = new Subject<int>();
        public Subject<int> PowerStageChanged = new Subject<int>();

        [Inject]
        private void Construct(PlayerInput playerInput)
        {
            _playerInput = playerInput;
        }

        public void Init()
        {
            AShooting.Init();
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
                AShooting.IncreaseDamage();
            }
            PowerAmountChanged?.OnNext(_currentPowerAmount);
        }

        public void DicreasePower()
        {
            _currentPowerStage -= 1;
            PowerStageChanged?.OnNext(_currentPowerStage);
            AShooting.DecreaseDamage();
        }

        private void OnEnable()
        {
            InputActionMap map = _playerInput.actions.FindActionMap("GamePlay");
            InputAction shoot = map["Shoot"];
            InputAction shift = map["SlowMovement"];

            shoot.started += ctx => Shoot();
            shoot.performed += ctx => Shoot();

            shift.started += ctx => Shifting(true);
            shift.canceled += ctx => Shifting(false);
        }

        private void OnDisable()
        {
            InputActionMap map = _playerInput.actions.FindActionMap("GamePlay");
            InputAction shoot = map["Shoot"];
            InputAction shift = map["SlowMovement"];

            shoot.started -= ctx => Shoot();
            shoot.performed -= ctx => Shoot();

            shift.started -= ctx => Shifting(true);
            shift.canceled -= ctx => Shifting(false);
        }
    }
}