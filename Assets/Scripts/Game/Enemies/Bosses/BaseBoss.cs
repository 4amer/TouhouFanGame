using Enemies.Bosses.Phase;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Enemies.Bosses
{
    public class BaseBoss : MonoBehaviour
    {
        [SerializeField] private BossPhase[] _attacks = new BossPhase[1];

        private int _currentPhaseIndex = 0;

        public Subject<Unit> OnHPChanged { get; set; } = new Subject<Unit>();
        public Subject<Unit> OnSpellCardStart { get; set; } = new Subject<Unit>();
        public Subject<Unit> OnSpellCardEnd { get; set; } = new Subject<Unit>();
        public Subject<Unit> OnAttackEnd { get; set; } = new Subject<Unit>();
        public Subject<Unit> OnAttackStart { get; set; } = new Subject<Unit>();

        public void Init()
        {
            StartAttack(_currentPhaseIndex);
        }

        private void StartAttack(int attackIndex)
        {
            if (attackIndex < 0 || attackIndex >= _attacks.Length)
            {
                Debug.LogWarning($"Phase index {attackIndex} is out of range.");
                return;
            }

            _currentPhaseIndex = attackIndex;
            BossPhase currentAttack = _attacks[attackIndex];

            if (currentAttack == null)
            {
                Debug.LogWarning("Current phase is null.");
                return;
            }

            SpawnAttackPattern(currentAttack.GetRandomAttack().gameObject);
            OnAttackStart.OnNext(Unit.Default);
        }

        private void NextPhase()
        {
            if (_currentPhaseIndex + 1 < _attacks.Length)
            {
                OnAttackEnd.OnNext(Unit.Default);
                StartAttack(_currentPhaseIndex + 1);
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

            Instantiate(attackPatternPrefab, transform);
        }

        private void DamageBoss(int damage)
        {

            OnHPChanged.OnNext(Unit.Default);
        }
    }
}
