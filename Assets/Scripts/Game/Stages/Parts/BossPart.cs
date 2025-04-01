using Cysharp.Threading.Tasks;
using Enemies;
using Enemies.Bosses;
using Stages.Manager;
using UI;
using UI.Windows;
using UniRx;
using UnityEngine;
using Zenject;

namespace Stages.Parts
{
    public class BossPart : APart
    {
        private float _timeOnStart = 0;

        private IBaseBoss _baseBoss;
        private IUIManager _UIManager;
        private IStageManagerTimer _stageManagerTimer;

        [SerializeField] BaseBoss baseBoss = null;

        [Inject]
        private void Construct(IStageManagerTimer stageManagerTimer, IUIManager uIManager)
        {
            _stageManagerTimer = stageManagerTimer;

            _UIManager = uIManager;

            _timeOnStart = stageManagerTimer.currentTime;

            stageManagerTimer.TimeChanged.Subscribe(_ => TimerUpdated(_)).AddTo(disposable);
        }

        public override void Init()
        {
            _baseBoss = baseBoss;

            baseBoss.OnDead
                .Subscribe(_ => Clear())
                .AddTo(disposable);

            baseBoss.Init();

            SetupBossWindow();
        }

        private void SetupBossWindow()
        {
            AWindow<BossWindowData> window = _UIManager.GetWindow<BossWindow>();
            window.SetData(new BossWindowData
            {
                boss = _baseBoss,
                stageManagerTimer = _stageManagerTimer
            });
            _UIManager.Show(window);
        }
    }
}

