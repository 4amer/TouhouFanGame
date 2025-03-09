using Cysharp.Threading.Tasks;
using Enemies.Bosses;
using Stages.Manager;
using UniRx;
using Zenject;

namespace Stages.Parts
{
    public class BossPart : APart
    {
        private float _timeOnStart = 0;

        private IInitBaseBoss _boss;

        [Inject]
        private void Construct(IStageManagerTimer stageManagerTimer, IInitBaseBoss baseBoss)
        {
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
        }
    }
}

