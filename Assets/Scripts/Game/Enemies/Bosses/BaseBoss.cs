using System;
using Audio;
using Audio.Types.Music;
using Enemies.Bosses.Attack;
using Enemies.Bosses.HP;
using Enemies.Bosses.Phase;
using Game.BulletSystem.Damage;
using UniRx;
using UnityEngine;
using Zenject;

namespace Enemies.Bosses
{
    public class BaseBoss : MonoBehaviour, IInitBaseBoss, IBaseBossActions, IBaseBossInfo, IBaseBoss, IDamagable
    {
        [SerializeField] private BossAttack[] _attacks = new BossAttack[1];

        [SerializeField] private GameObject _bossObject = null;

        [SerializeField] private HealthController _healthController = null;

        private int _currentPhaseIndex = 0;

        private DiContainer _diContainer = null;

        private IAudioManager _audioManager = null;

        private BossPattern _currentPatternObject = null;
        public Subject<Unit> OnDeath { get; set; } = new Subject<Unit>();
        public Subject<float> OnDamaged { get; set; } = new Subject<float>();
        public Subject<BossAttack> OnSpellCardStart { get; set; } = new Subject<BossAttack>();
        public Subject<BossAttack> OnSpellCardEnd { get; set; } = new Subject<BossAttack>();
        public Subject<BossAttack> OnAttackEnd { get; set; } = new Subject<BossAttack>();
        public Subject<BossAttack> OnAttackStart { get; set; } = new Subject<BossAttack>();

        public bool IsVulnerable { get; set; } = false;

        public Transform Transform { get => transform; }

        public HealthController HealthController { get => _healthController; }

        public Subject<IDamagable> OnDead { get; set; } = new Subject<IDamagable>();

        public float RangeToCollide { get; set; } = 3f;

        [Inject]
        private void Construct(DiContainer diContainer, IAudioManager audioManager, IDamagableManager damagableManager)
        {
            _diContainer = diContainer;
            _audioManager = audioManager;

            damagableManager.AddDamagable(this);
        }

        public void Init()
        {
            StartAttack(_currentPhaseIndex);
            _audioManager.Play(EMusicTypes.TestMusic);

            HealthControllerSetup();
        }

        private void HealthControllerSetup()
        {
            _healthController.Init(this, GetCurrentHPAmount());
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

        public void Damage(float damage)
        {
            OnDamaged?.OnNext(damage);
        }
    }

    public interface IBaseBossActions
    {
        public Subject<Unit> OnDeath { get; set; }
        public Subject<float> OnDamaged { get; set; }
        public Subject<BossAttack> OnSpellCardStart { get; set; }
        public Subject<BossAttack> OnSpellCardEnd { get; set; }
        public Subject<BossAttack> OnAttackEnd { get; set; }
        public Subject<BossAttack> OnAttackStart { get; set; }
    }

    public interface IBaseBossInfo
    {
        public HealthController HealthController { get; }
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