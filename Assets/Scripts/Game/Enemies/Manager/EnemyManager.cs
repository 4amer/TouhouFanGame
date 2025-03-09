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
        [SerializeField] private EntityController[] enemyControllers = new EntityController[1];

        private IEntityController[] _enemyControllers = null;

        private List<EntityController> _initializedEnemies = new List<EntityController>();

        private Dictionary<IEntityController, float> EnemysTime = new Dictionary<IEntityController, float>();

        private CompositeDisposable disposables = new CompositeDisposable();

        private Transform _playerTransform = null;

        private float _timeOnStart = 0;

        [Inject]
        private void Construct(IPlayerManagerTransform playerManager, IStageManagerTimer stageManagerTimer)
        {
            _playerTransform = playerManager.PlayerTransform;
            _timeOnStart = stageManagerTimer.currentTime;

            stageManagerTimer
                .TimeChanged
                .Subscribe(_ => TimerUpdated(_))
                .AddTo(disposables);
        }

        public void Init(GameObject entity = null)
        {
            _enemyControllers = enemyControllers;
            InitEnemies(_playerTransform, entity);
            FillEnemysTime();
        }

        private void InitEnemies(Transform player, GameObject entity)
        {
            foreach (IEntityController enemy in _enemyControllers)
            {
                enemy.Init(player, entity);
            }
        }

        private void FillEnemysTime()
        {
            foreach (IEntityController enemy in _enemyControllers)
            {
                EnemysTime.Add(enemy, 0f);
            }
        }

        private void TimerUpdated(float time)
        {
            float localTime = time - _timeOnStart;
            foreach (EntityController enemy in _enemyControllers)
            {
                if (EnemysTime.TryGetValue(enemy, out float lastEventTime))
                {
                    if (enemy.EventSequencesQueue.Count == 0 && enemy.IsSequenceCycled)
                    {
                        enemy.RestoreSequence();
                    }

                    while (enemy.EventSequencesQueue.Count > 0)
                    {
                        EventSequence currentSequence = enemy.EventSequencesQueue.Peek();
                        float scheduledTime = lastEventTime + currentSequence.Delay;

                        if (scheduledTime <= localTime)
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
        void Init(GameObject entity = null);
    }
}