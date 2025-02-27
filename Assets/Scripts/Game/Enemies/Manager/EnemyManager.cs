using System.Collections.Generic;
using Enemies.Sequences;
using Game.Player.Manager;
using Stages.Manager;
using Stages.Parts;
using UniRx;
using UnityEngine;
using Zenject;

namespace Enemies.Manager
{
    public class EnemyManager : MonoBehaviour, IEnemyManager
    {
        [SerializeField] private EnemyController[] enemyControllers = new EnemyController[1];

        private IEnemyController[] _enemyControllers = null;

        private List<EnemyController> _initializedEnemies = new List<EnemyController>();

        private Dictionary<IEnemyController, float> EnemysTime = new Dictionary<IEnemyController, float>();

        private CompositeDisposable disposables = new CompositeDisposable();

        private Transform _playerTransform = null;

        [Inject]
        private void Construct(IPlayerManagerTransform playerManager)
        {
            _playerTransform = playerManager.PlayerTransform;
        }

        public void Init(IPartAction partAction)
        {
            _enemyControllers = enemyControllers;
            InitEnemies(_playerTransform);
            FillEnemysTime();

            partAction
                .TimerUpdatedLocal
                .Subscribe(_ => TimerUpdated(_))
                .AddTo(disposables);
        }

        private void InitEnemies(Transform player)
        {
            foreach (IEnemyController enemy in _enemyControllers)
            {
                enemy.Init(player);
            }
        }

        private void FillEnemysTime()
        {
            foreach (IEnemyController enemy in _enemyControllers)
            {
                EnemysTime.Add(enemy, 0f);
            }
        }

        private void TimerUpdated(float time)
        {
            foreach (EnemyController enemy in _enemyControllers)
            {
                if (EnemysTime.TryGetValue(enemy, out float lastEventTime))
                {
                    while (enemy.EventSequencesQueue.Count > 0)
                    {
                        EventSequence currentSequence = enemy.EventSequencesQueue.Peek();
                        float scheduledTime = lastEventTime + currentSequence.Delay;

                        if (scheduledTime <= time)
                        {
                            currentSequence.Event.Invoke();
                            lastEventTime = scheduledTime;
                            enemy.EventSequencesQueue.Dequeue();
                            EnemysTime[enemy] = lastEventTime;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    public interface IEnemyManager
    {
        void Init(IPartAction partAction);
    }
}