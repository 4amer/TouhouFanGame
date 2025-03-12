using Enemies.Bosses.Phase;
using Stages.Manager;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace Enemies.Bosses.Timer
{
    public class TimerCotroller : MonoBehaviour, ITimerControll
    {
        [SerializeField] private TextMeshProUGUI _timerText = null;

        private float _timeShift = 0;
        private int _currentTimerTime = 0;
        public Subject<Unit> TimeOut { get; set; } = new Subject<Unit>();

        private CompositeDisposable _disposable = new CompositeDisposable();

        private IStageManagerTimer _stageTimer;

        [Inject]
        private void Construct(IStageManagerTimer stageTimer, IBaseBossActions baseBossActions)
        {
            baseBossActions.OnAttackStart
                .Subscribe(_ => ChangeTimer(_))
                .AddTo(_disposable);

            _stageTimer = stageTimer;

            _stageTimer.TimeChanged
                .Subscribe(_ => TimeChanged(_))
                .AddTo(_disposable);
        }

        public void Init()
        {
            
        }

        private void TimeChanged(float timerTime)
        {
            float time = (_timeShift - timerTime) + _currentTimerTime;
            int roundTime = Mathf.RoundToInt(time);
            if(roundTime > -1)
                _timerText.text = $"{roundTime}";

            if (roundTime < 0)
            {
                TimeOut?.OnNext(Unit.Default);
            }
        }
        private void ChangeTimer(BossAttack attack)
        {
            _timeShift = _stageTimer.currentTime;
            _currentTimerTime = attack.TimeTobeat;

        }
    }

    internal interface ITimerControll
    {
        public void Init();
        public Subject<Unit> TimeOut { get; set; }
    }
}
