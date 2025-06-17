using System;
using DG.Tweening;
using Services.Money;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace UI.Windows
{
    public class ResultWindow : AWindow<ResultWindowData>
    {
        [SerializeField] private TextMeshProUGUI _timerCounter = null;
        [SerializeField] private TextMeshProUGUI _coinCounter = null;
        [SerializeField] private TextMeshProUGUI _enemyCounter = null;

        [Space(10)]
        [Header("Skip button")]
        [SerializeField] private TextMeshProUGUI _proceedText = null;
        [SerializeField] private float _timeToShowAndHide = 1f;
        [SerializeField] private Color _colorToShow = Color.white;

        [Space(10)]
        [Header("Animation Event Listener")]
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimationEventListener _animationEventListener = null;

        [Space(10)]
        [Header("Hide")]
        [SerializeField] private float _hideTimer = 2f;

        private IMoneyService _moneyService = null;

        private Sequence _showAnimtaion;

        private float _completeTime = 0f;

        private CompositeDisposable _disposable = new CompositeDisposable();    
        private PlayerInput _playerInput = null;

        private ResultWindowData _data = null;

        private InputActionMap _map;
        private System.Action<InputAction.CallbackContext> _goToMenu;

        private bool _isSubscribed = false;

        private bool isToMainMenuCalled = false;

        [Inject]
        private void Construct(IMoneyService moneyService, PlayerInput playerInput)
        {
            _moneyService = moneyService;
            _playerInput = playerInput;
        }

        public override void Show()
        {
            base.Show();

            _showAnimtaion = DOTween.Sequence();

            isToMainMenuCalled = false;

            Color textColor = _proceedText.color;

            _showAnimtaion.Append(_proceedText.DOColor(_colorToShow, _timeToShowAndHide))
                .Append(_proceedText.DOColor(textColor, _timeToShowAndHide)).SetLoops(-1);

            _showAnimtaion.Pause();

            if(_disposable == null)
            {
                _disposable = new CompositeDisposable();
            }

            _animationEventListener
                .OnAnimationEvent
                .Subscribe(_ => OnAnimationEnd())
                .AddTo(_disposable);

            _timerCounter.text = $"{Mathf.FloorToInt(_completeTime / 60)}:{Mathf.FloorToInt(_completeTime % 60)}";
            _coinCounter.text = $"{_moneyService.GetMoneyAmount}";

            _animator.SetTrigger("StartAnimaiton");

            SubscribeOnActionEvents();
        }

        public override void SetData(ResultWindowData data)
        {
            _completeTime = data.timeInSeconds;
            _data = data;
        }

        private void OnAnimationEnd()
        {
            _showAnimtaion.Restart();
        }

        public override void Hide()
        {
            _animator.SetTrigger("GoToStart");
            Dispose();
            UnsubscribeOnActionEvents();
            base.Hide();
        }

        private void ToMainMenu()
        {
            if (isToMainMenuCalled) return;
            isToMainMenuCalled = true;

            _data.OnGoToMenu?.Invoke();

            Utils.Timer timer = new Utils.Timer();
            timer.SetupDelayWithAction(_hideTimer, () =>
            {
                Hide();
            });
        }

        private void SubscribeOnActionEvents()
        {

            if (_isSubscribed) return;
            _isSubscribed = true;
            _playerInput.SwitchCurrentActionMap("UI");

            InputActionMap map = _playerInput.currentActionMap;

            _map = map;
            _goToMenu = (ctx) => ToMainMenu();

            map["Confirm"].started += _goToMenu;
        }

        private void UnsubscribeOnActionEvents()
        {
            _isSubscribed = false;
            if (_goToMenu == null) return;

            _playerInput.SwitchCurrentActionMap("GamePlay");

            _map["Confirm"].started -= _goToMenu;
        }

        private void OnDestroy()
        {
            Dispose();
        }
        private void Dispose()
        {
            _disposable?.Clear();
        }
    }
}