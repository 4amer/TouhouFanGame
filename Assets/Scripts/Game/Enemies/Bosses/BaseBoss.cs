using System.Collections.Generic;
using Enemies.Bosses.Attack;
using Enemies.Bosses.HP;
using Enemies.Bosses.Phase;
using Enemies.Bosses.SpellCards;
using Enemies.Bosses.Timer;
using UniRx;
using UnityEngine;
using Zenject;

namespace Enemies.Bosses
{
    public class BaseBoss : MonoBehaviour, IInitBaseBoss, IBaseBossActions, IBaseBossInfo, IBaseBoss
    {
        [SerializeField] private BossAttack[] _attacks = new BossAttack[1];

        [SerializeField] private GameObject _bossObject = null;

        private int _currentPhaseIndex = 0;

        private DiContainer _diContainer = null;

        private BossPattern _currentPatternObject = null;
        public Subject<Unit> OnDeath { get; set; } = new Subject<Unit>();
        public Subject<Unit> OnHPChanged { get; set; } = new Subject<Unit>();
        public Subject<BossAttack> OnSpellCardStart { get; set; } = new Subject<BossAttack>();
        public Subject<BossAttack> OnSpellCardEnd { get; set; } = new Subject<BossAttack>();
        public Subject<BossAttack> OnAttackEnd { get; set; } = new Subject<BossAttack>();
        public Subject<BossAttack> OnAttackStart { get; set; } = new Subject<BossAttack>();

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

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
            BossAttack currentAttack = _attacks[attackIndex];

            if (currentAttack == null)
            {
                Debug.LogWarning("Current phase is null.");
                return;
            }

            SpawnAttackPattern(currentAttack.GetRandomAttack());
            OnAttackStart?.OnNext(currentAttack);
            if (currentAttack.IsSpellCard)
            {
                OnSpellCardStart?.OnNext(currentAttack);
            }
        }

        public void NextPhase()
        {
            if (_currentPhaseIndex + 1 < _attacks.Length)
            {
                BossAttack currentAttack = _attacks[_currentPhaseIndex];
                OnAttackEnd?.OnNext(currentAttack);
                if (currentAttack.IsSpellCard)
                {
                    OnSpellCardEnd?.OnNext(currentAttack);
                }
                DestroyPattern();
                StartAttack(_currentPhaseIndex + 1);
            }
            else
            {
                Debug.Log("Boss defeated or no more phases left.");
            }
        }

        private void SpawnAttackPattern(BossPattern attackPatternPrefab)
        {
            if (attackPatternPrefab == null)
            {
                Debug.LogWarning("Attack pattern prefab is null.");
                return;
            }

            BossPattern instantiatedObject = Instantiate(attackPatternPrefab, transform);
            _diContainer.Inject(instantiatedObject);
            instantiatedObject.Init(_bossObject);
            _currentPatternObject = instantiatedObject;
        }

        public float GetCurrentHPAmount()
        {
            return _attacks[_currentPhaseIndex].HP;
        }

        public BossAttack GetCurrentAttack()
        {
            return _attacks[_currentPhaseIndex];
        }

        public int GetSpellCardAmount()
        {
            int counter = 0;
            foreach (BossAttack attack in _attacks)
            {
                if (attack.IsSpellCard == false) continue;
                counter += 1;
            }

            return counter;
        }

        private void DestroyPattern()
        {
            _currentPatternObject.Clear();
            Destroy(_currentPatternObject.gameObject);
        }

        private void DamageBoss(int damage)
        {
            OnHPChanged.OnNext(Unit.Default);
        }
    }

    public interface IBaseBossActions
    {
        public Subject<Unit> OnDeath { get; set; }
        public Subject<Unit> OnHPChanged { get; set; }
        public Subject<BossAttack> OnSpellCardStart { get; set; }
        public Subject<BossAttack> OnSpellCardEnd { get; set; }
        public Subject<BossAttack> OnAttackEnd { get; set; }
        public Subject<BossAttack> OnAttackStart { get; set; }
    }

    public interface IBaseBossInfo
    {
        public float GetCurrentHPAmount();
        public int GetSpellCardAmount();
        public BossAttack GetCurrentAttack();
    }

    public interface IBaseBossNextPhase
    {
        public void NextPhase();
    }

    public interface IBaseBoss : IBaseBossActions, IBaseBossInfo, IBaseBossNextPhase { }

    public interface IInitBaseBoss
    {
        public Subject<Unit> OnDeath { get; set; }
        public void Init();
    }   
}