using UniRx;
using Utils;
using UnityEngine;
using Zenject;
using Player.Health;
using Game.Player.Manager;
using UI;
using UI.Windows;
using Services.GSMC;
using Services.GSMC.States;
using JetBrains.Annotations;

namespace Stages.Manager
{
    public class StageManager : MonoBehaviour, IStageManager, IStageManagerTimer, IStageManagerActions
    {
        [SerializeField] private BaseStage[] _stages = new BaseStage[6];

        [SerializeField] private int _stageIndex = 0;

        [SerializeField] private Transform _partsParentTransform = null;

        private IPlayerManagerActions _playerManager = null;

        private IBaseStage _currentStage = default;
        private IUIManager _uIManager = null;
        private IGameStateMachine _gameStateMachine = null;

        public Subject<Unit> OnSceneChanged { get; set; } = new Subject<Unit>();

        public Subject<float> TimeChanged { get; set; } = new Subject<float>();

        private Timer _timer = null;

        private CompositeDisposable _stageDisposables = new CompositeDisposable();
        private DiContainer _diContainer = null;
        public float currentTime { get => (_timer != null) ? _timer.GetCurrentTime : 0; }

        [Inject]
        public void Construct(DiContainer diContainer, IPlayerManagerActions playerManager, IUIManager uIManager,
            IGameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
            _diContainer = diContainer;
            _playerManager = playerManager;
            _uIManager = uIManager;
        }

        public void Init()
        {
            InitStage(_stageIndex);
            SetupTimer();
        }

        private void InitStage(int index)
        {
            _currentStage = _stages[index];

            _currentStage
                .StageClear
                .Subscribe(_ => NextState())
                .AddTo(_stageDisposables);

            _playerManager
                .OnPlayerDead
                .Subscribe(_ => StopGame())
                .AddTo(_stageDisposables);

            _diContainer.Inject(_currentStage);

            _currentStage.Init(_partsParentTransform, this);
        }

        private void WhenStageClear(IBaseStage stage)
        {
            NextState();
        }

        private void NextState()
        {
            _stageDisposables.Clear();
            OpenResultWindow();
        }

        private void HideAllGameWindows()
        {
            _uIManager.Hide<BossWindow>();
            _uIManager.Hide<GameWindow>();
        }

        private void OpenResultWindow()
        {
            AWindow<ResultWindowData> resultWindow = _uIManager.GetWindow<ResultWindow>();
            resultWindow.SetData(new ResultWindowData
            {
                timeInSeconds = _timer.GetCurrentTime,
                OnGoToMenu = GoToMenu,
            });
            _uIManager.Show(resultWindow);
        }

        private void StopGame()
        {
            _timer.Pause();
            OpenResultWindow();
        }

        private void SetupTimer()
        {
            _timer = new Timer();
            _timer.timeStep = 0.1f;
            _timer.EventOnUpdate = () =>
            {
                TimeChanged.OnNext(_timer.GetCurrentTime);
            };
            _timer.StartInfinity();
        }

        private void OnDestroy()
        {
            _stageDisposables.Dispose();
            _timer?.Reset();
        }

        private void GoToMenu()
        {
            HideAllGameWindows();
            OnSceneChanged?.OnNext(Unit.Default);
            _gameStateMachine.ChangeState<MenuState>();
        }
    }

    public interface IStageManagerTimer
    {
        public Subject<float> TimeChanged { get; set; }
        public float currentTime { get; }
    }

    public interface IStageManagerActions
    {
        public Subject<float> TimeChanged { get; set; }
        public Subject<Unit> OnSceneChanged { get; set; }
    }

    public interface IStageManager 
    {
        public void Init();
    }
}
