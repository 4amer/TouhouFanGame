using Enemies.Bosses.Phase;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Enemies.Bosses
{
    public class BaseBoss : MonoBehaviour
    {
        [SerializeField] private BossPhase[] _phases = new BossPhase[1];
        [SerializeField] private Transform _spawnPoint;

        private int _currentPhaseIndex = 0;

        public Subject<Unit> OnHPChanged { get; set; } = new Subject<Unit>();
        public Subject<Unit> OnSpellCardStart { get; set; } = new Subject<Unit>();
        public Subject<Unit> OnSpellCardEnd { get; set; } = new Subject<Unit>();

        public void Init()
        {
            StartPhase(_currentPhaseIndex);
        }

        private void StartPhase(int phaseIndex)
        {
            if (phaseIndex < 0 || phaseIndex >= _phases.Length)
            {
                Debug.LogWarning($"Phase index {phaseIndex} is out of range.");
                return;
            }

            _currentPhaseIndex = phaseIndex;
            BossPhase currentPhase = _phases[phaseIndex];

            if (currentPhase == null)
            {
                Debug.LogWarning("Current phase is null.");
                return;
            }

            SpawnAttackPattern(currentPhase.AttackPatternPrefab);
            OnSpellCardStart.OnNext(Unit.Default);
        }

        private void NextPhase()
        {
            if (_currentPhaseIndex + 1 < _phases.Length)
            {
                OnSpellCardEnd.OnNext(Unit.Default);
                StartPhase(_currentPhaseIndex + 1);
            }
            else
            {
                Debug.Log("Boss defeated or no more phases left.");
            }
        }

        private void SpawnAttackPattern(GameObject attackPatternPrefab)
        {
            if (attackPatternPrefab == null)
            {
                Debug.LogWarning("Attack pattern prefab is null.");
                return;
            }

            Instantiate(attackPatternPrefab, _spawnPoint.position, Quaternion.identity, transform);
        }

        private void DamageBoss(int damage)
        {
            // Здесь можно реализовать логику понижения HP, вызов события об изменении HP и проверку на смену фазы
            OnHPChanged.OnNext(Unit.Default);
        }
    }
}
