using System;
using System.Threading;
using System.Threading.Tasks;

namespace Utils
{
    public class Timer
    {
        public float duration = 5f;
        public float timeStep = 1f;

        public event Action OnTimerStart;
        public event Action<float> OnTimerUpdated;
        public event Action OnTimerFinish;
        public event Action OnTimerPause;

        public Action EventOnStart;
        public Action EventOnUpdate;
        public Action EventOnFinish;
        public Action EventOnPause;

        private const int ONE_SECOND = 1000;
        private float _currentTimerTime = 0f;

        private bool _isTimerStoped = true;

        private CancellationTokenSource _cancellationTokenSource = null;

        public Timer()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public float GetCurrentTime
        {
            get
            {
                return _currentTimerTime;
            }
        }
        public void Start()
        {
            if (_isTimerStoped == false) return;


            _cancellationTokenSource = new CancellationTokenSource();
            _isTimerStoped = false;
            EventOnStart?.Invoke();
            OnTimerStart?.Invoke();
            StartTimer(_cancellationTokenSource.Token);
        }

        public void StartInfinity()
        {
            if (_isTimerStoped == false) return;
            duration = int.MaxValue;

            Start();
        }

        public void Pause()
        {
            EventOnPause?.Invoke();
            OnTimerPause?.Invoke();
            PauseTimer();
        }

        public void Finish()
        {
            EventOnFinish?.Invoke();
            OnTimerFinish?.Invoke();
            ResetTimer();
        }

        public void Reset()
        {
            ResetTimer();
        }

        public bool IsPlaying => !_isTimerStoped;

        private void ResetTimer()
        {
            _currentTimerTime = 0;
            PauseTimer();
        }

        private void PauseTimer()
        {
            _isTimerStoped = true;
            _cancellationTokenSource.Cancel();
        }

        private async Task StartTimer(CancellationToken token)
        {
            _currentTimerTime = 0;
            try
            {
                while (!_isTimerStoped)
                {
                    int timeToUpdate = (int)(ONE_SECOND * timeStep);

                    if (_currentTimerTime >= duration)
                    {
                        Finish();
                        break;
                    }

                    await Task.Delay(timeToUpdate, token);
                    _currentTimerTime += timeStep;

                    _currentTimerTime = (float)Math.Round(_currentTimerTime, 1);

                    EventOnUpdate?.Invoke();
                    OnTimerUpdated?.Invoke(_currentTimerTime);
                }
            }
            catch (TaskCanceledException)
            {

            }
        }
    }
}
