using Cysharp.Threading.Tasks;
using Stages.Manager;
using UniRx;
using Zenject;

namespace Stages.Parts
{
    public class BossPart : APart
    {
        private float _timeOnStart = 0;

        [Inject]
        private void Construct(IStageManagerTimer stageManagerTimer)
        {
            _timeOnStart = stageManagerTimer.currentTime;

            stageManagerTimer.TimeChanged.Subscribe(_ => TimerUpdated(_)).AddTo(disposable);
        }

        public override void Init()
        {

        }

        public override void TimerUpdated(float time)
        {
            
        }
    }
}

