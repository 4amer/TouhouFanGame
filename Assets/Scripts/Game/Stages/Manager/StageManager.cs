using Cysharp.Threading.Tasks;
using Stages.Parts;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Stages.Manager
{
    public class StageManager : MonoBehaviour, IStageManager, IStageManagerActions
    {
        [SerializeField] private BaseStage[] _stages = new BaseStage[6];

        [SerializeField] private int _stageIndex = 0;

        [SerializeField] private Transform _partsParentTransform = null;

        private IBaseStage _currentStage = default;

        public Subject<float> TimeChanged { get; set; } = new Subject<float>();

        private Timer _timer = null;

        private CompositeDisposable _stageDisposables = new CompositeDisposable();
        private DiContainer _diContainer = null;

        [Inject]
        public void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
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
            _timer?.Dispose();
        }
    }

    public interface IStageManagerActions
    {
        public Subject<float> TimeChanged { get; set; }
    }

    public interface IStageManager 
    {
        public void Init();
    }
}
