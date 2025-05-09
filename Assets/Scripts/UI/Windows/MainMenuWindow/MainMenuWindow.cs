using DG.Tweening;
using Services.GSMC;
using Services.GSMC.States;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

namespace UI.Windows
{
    public class MainMenuWindow : AWindow<MainMenuWindowData>
    {
        [SerializeField] private Button[] _buttons = new Button[1];

        [Space(10)]
        [Header("Button Selection Setup")]
        [SerializeField] private float _timeToSelectButton = 0.2f;
        [SerializeField] private float _selectedButtonShift = 50f;
        [SerializeField] private Ease _easeAnimation = Ease.OutCubic;
        [Header("Arrow")]
        [SerializeField] private Transform _arrowImageTransform = null;
        [SerializeField] private float _arrowPositionXShift = 350f;
        [SerializeField] private float _arrowSwingTweenShift = 5f;
        [SerializeField] private float _arrowYShift = 220f;
        [SerializeField] private float _arrowSwingTweenShiftTime = 1f;
        [SerializeField] private float _arrowMoveToButtonTime = 0.1f;
        [SerializeField] private Ease _easeArrow = Ease.OutCubic;

        private int _currentSelectedButtonIndex = 0;
        private Button _currentButton = null;

        private PlayerInput _playerInput = null;
        private IGameStateMachine _gameStateMachine = null;

        private const float _delayBeforeHide = 2f;

        private Sequence _arrowSwingAnimation = DOTween.Sequence();

        [Inject]
        private void Construct(PlayerInput playerInput, IGameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
            _playerInput = playerInput;
        }

        public override void Show()
        {
            base.Show();
            SubscribeOnInput();
            _currentButton = _buttons[_currentSelectedButtonIndex];
            DoStartButtonAnimation();
            StartArrowAnimation();
        }

        public override void SetData(MainMenuWindowData data)
        {

        }

        public override void Hide()
        {
            base.Hide();
            UnsubscribeOnInput();
            StopArrowAnimation();
        }

        private void SelectButtonAbove()
        {
            if ((_currentSelectedButtonIndex - 1) < 0) return;
            _currentSelectedButtonIndex -= 1;
            OnTweenButton();
        }

        private void SelectButtonBelow()
        {
            if ((_currentSelectedButtonIndex + 1) >= _buttons.Length) return;
            _currentSelectedButtonIndex += 1;
            OnTweenButton();
        }

        private void OnTweenButton()
        {
            TweenButton();
            _currentButton = _buttons[_currentSelectedButtonIndex];
        }

        private void TweenButton()
        {
            Transform currentButton = _currentButton.transform;
            Transform nextButton = _buttons[_currentSelectedButtonIndex].transform;

            Sequence sequence = DOTween.Sequence();

            sequence
                .Append(BackButton(currentButton))
                .Join(PopButton(nextButton))
                .Join(ChangeArrowPosition(nextButton));
        }

        private Tween BackButton(Transform buttonTransform)
        {
            Tween t = buttonTransform.DOLocalMoveZ(0, _timeToSelectButton);
            return t;
        }

        private Tween PopButton(Transform buttonTransform)
        {
            Tween t = buttonTransform.DOLocalMoveZ(-_selectedButtonShift, _timeToSelectButton);
            return t;
        }

        private Tween ChangeArrowPosition(Transform nextButtonTransform)
        {
            float buttonYPosition = nextButtonTransform.localPosition.y;
            Tween t = _arrowImageTransform.DOLocalMove(new Vector3(-_arrowPositionXShift, buttonYPosition + _arrowYShift, 0f), _arrowMoveToButtonTime);
            return t;
        }

        private void SelectButton()
        {
            _buttons[_currentSelectedButtonIndex].onClick.Invoke();
        }

        private void OnDecline()
        {
            

            //buttonTransform.
        }

        public void StartGame()
        {
            _gameStateMachine.ChangeState<GameState>();
            Utils.Timer timer = new Utils.Timer();
            timer.SetupDelayWithAction(_delayBeforeHide, () =>
            {
                Hide();
            });
        }

        private void DoStartButtonAnimation()
        {
            Transform buttonTransform = _currentButton.transform;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(PopButton(buttonTransform));
        }

        private void StartArrowAnimation()
        {
            _arrowSwingAnimation = DOTween.Sequence();

            _arrowSwingAnimation.Append(_arrowImageTransform.DOLocalMoveX((-_arrowPositionXShift + _arrowSwingTweenShift), _arrowSwingTweenShiftTime).SetEase(_easeArrow))
                .Append(_arrowImageTransform.DOLocalMoveX((-_arrowPositionXShift - _arrowSwingTweenShift), _arrowSwingTweenShiftTime).SetEase(_easeArrow))
                .SetLoops(-1);

            _arrowSwingAnimation.Restart();
        }

        private void StopArrowAnimation()
        {
            _arrowSwingAnimation.Pause();
        }

        private void SubscribeOnInput()
        {
            _playerInput.SwitchCurrentActionMap("UI");

            InputActionMap map = _playerInput.currentActionMap;

            map["Up"].started += (ctx) => SelectButtonAbove();
            map["Down"].started += (ctx) => SelectButtonBelow();
            map["Decline"].started += (ctx) => OnDecline();
            map["Confirm"].started += (ctx) => SelectButton();
        }
        private void UnsubscribeOnInput()
        {
            InputActionMap map = _playerInput.actions.FindActionMap("UI");

            map["Up"].started -= (ctx) => SelectButtonAbove();
            map["Down"].started -= (ctx) => SelectButtonBelow();
            map["Decline"].started -= (ctx) => OnDecline();
            map["Confirm"].started -= (ctx) => SelectButton();
        }
    }
}
