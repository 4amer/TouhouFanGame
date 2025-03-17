using Cysharp.Threading.Tasks;
using Enemies;
using Enemies.Bosses;
using Stages.Manager;
using UI;
using UI.Windows;
using UniRx;
using Zenject;

namespace Stages.Parts
{
    public class BossPart : APart
    {
        private float _timeOnStart = 0;

        private IInitBaseBoss _boss;
        private IUIManager _UIManager;

        [Inject]
        private void Construct(IStageManagerTimer stageManagerTimer, IInitBaseBoss baseBoss, IUIManager uIManager)
        {
            _UIManager = uIManager;

            _timeOnStart = stageManagerTimer.currentTime;
            _boss = baseBoss;

            stageManagerTimer.TimeChanged.Subscribe(_ => TimerUpdated(_)).AddTo(disposable);
        }

        public override void Init()
        {
            _boss.OnDeath
                .Subscribe(_ => Clear())
                .AddTo(disposable);

            _boss.Init();

            SetupBossWindow();
        }

        private void SetupBossWindow()
        {
            AWindow<BossWindowData> window = _UIManager.GetWindow<BossWindow>();
            window.SetData(new BossWindowData 
            { 
                            
            });
            _UIManager.Show(window);
        }
    }
}

