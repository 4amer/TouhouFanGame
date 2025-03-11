using System.Collections.Generic;
using Enemies.Bosses.Attack;
using Enemies.Bosses.HP;
using Enemies.Bosses.Phase;
using Enemies.Bosses.SpellCards;
using Enemies.Bosses.Timer;
using UniRx;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;
using Zenject;

namespace Enemies.Bosses
{
    public class BaseBoss : MonoBehaviour, IInitBaseBoss, IBaseBossActions
    {
        [SerializeField] private BossAttack[] _attacks = new BossAttack[1];

        [SerializeField] private GameObject _bossObject = null;

        private int _currentPhaseIndex = 0;

        private DiContainer _diContainer = null;

        private IHealthController _healthController = null;
        private ISpellCardManager _spellCardManager = null;
        private ITimerControll _timerControll = null;

        public Subject<Unit> OnDeath { get; set; } = new Subject<Unit>();
        public Subject<Unit> OnHPChanged { get; set; } = new Subject<Unit>();
        public Subject<BossAttack> OnSpellCardStart { get; set; } = new Subject<BossAttack>();
        public Subject<BossAttack> OnSpellCardEnd { get; set; } = new Subject<BossAttack>();
        public Subject<BossAttack> OnAttackEnd { get; set; } = new Subject<BossAttack>();
        public Subject<BossAttack> OnAttackStart { get; set; } = new Subject<BossAttack>();

        [Inject]
        private void Construct(DiContainer diContainer, IHealthController healthController,
            ISpellCardManager spellCardManager, ITimerControll timerControll)
        {
            _diContainer = diContainer;

            _healthController = healthController;
            _spellCardManager = spellCardManager;
            _timerControll = timerControll;
        }

        public void Init()
        {
            StartAttack(_currentPhaseIndex);

            _healthController.Init(CurrentHPAmount());
            _spellCardManager.Init(GetSpellCardAmount());
            _timerControll.Init();
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
            OnAttackStart.OnNext(currentAttack);
        }

        private void NextPhase()
        {
            if (_currentPhaseIndex + 1 < _attacks.Length)
            {
                OnAttackEnd.OnNext(_attacks[_currentPhaseIndex]);
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
        }

        private float CurrentHPAmount()
        {
            return _attacks[_currentPhaseIndex].HP;
        }

        private int GetSpellCardAmount()
        {
            int counter = 0;
            foreach (BossAttack attack in _attacks)
            {
                if (attack.IsSpellCard == false) continue;
                counter += 1;
            }

            return counter;
        }

        private void DamageBoss(int damage)
        {
            OnHPChanged.OnNext(Unit.Default);
        }
    }

    internal interface IBaseBossActions
    {
        public Subject<Unit> OnDeath { get; set; }
        public Subject<Unit> OnHPChanged { get; set; }
        public Subject<BossAttack> OnSpellCardStart { get; set; }
        public Subject<BossAttack> OnSpellCardEnd { get; set; }
        public Subject<BossAttack> OnAttackEnd { get; set; }
        public Subject<BossAttack> OnAttackStart { get; set; }
    }

    internal interface IInitBaseBoss
    {
        public Subject<Unit> OnDeath { get; set; }
        public void Init();
    }
    
}
