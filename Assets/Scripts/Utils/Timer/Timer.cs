using System;
using System.Threading;
using System.Threading.Tasks;
public class Timer
{
    public float duration = 5f;
    public float timeStep = 1f;

    public event Action OnTimerStop;
    public event Action OnTimerStart;
    public event Action<float> OnTimerUpdated;

    public Action EventOnStart;
    public Action EventOnUpdate;
    public Action EventOnStop;

    private const int ONE_SECOND = 1000;
    private float _currentTimerTime = 0f;

    private bool _isTimerEnd = true;

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
        if (EventOnStart != null) EventOnStart();
        _isTimerEnd = false;
        OnTimerStart?.Invoke();
        StartTimer(_cancellationTokenSource.Token);
    }

    public void StartInfinity()
    {
        if (EventOnStart != null) EventOnStart();
        _isTimerEnd = false;
        duration = int.MaxValue;
        OnTimerStart?.Invoke();
        StartTimer(_cancellationTokenSource.Token);
    }

    public void Stop()
    {
        if (EventOnStop != null) EventOnStop();
        OnTimerStop?.Invoke();
        ResetTimer();
    }

    public void Dispose()
    {
        ResetTimer();
    }

    public bool IsPlaying => !_isTimerEnd;

    private void ResetTimer()
    {
        _currentTimerTime = 0;
        _isTimerEnd = true;

        _cancellationTokenSource.Cancel();
    }

    private async Task StartTimer(CancellationToken token)
    {
        _currentTimerTime = 0;
        try
        {
            while (!_isTimerEnd)
            {
                int timeToUpdate = (int)(ONE_SECOND * timeStep);

                if (_currentTimerTime >= duration)
                {
                    Stop();
                    break;
                }

                await Task.Delay(timeToUpdate, token);
                _currentTimerTime += timeStep;

                EventOnUpdate?.Invoke();
                OnTimerUpdated?.Invoke(_currentTimerTime);
            }
        }
        catch (TaskCanceledException)
        {

        }
    }
}
