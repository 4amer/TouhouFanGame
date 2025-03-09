using System;
using System.Runtime.CompilerServices;
using Enemies.Bosses.Attack;
using UnityEngine;

namespace Enemies.Bosses.Phase
{
    [Serializable]
    public class BossAttack
    {
        [SerializeField] private BossPattern[] _attacks = new BossPattern[1];
        [SerializeField] private bool _isSpellCard = false;
        [SerializeField] private float _hpOnPhase = 100f;
        [SerializeField] private int _timeToBeatInSeconds = 99;
        [SerializeField] private bool _isInvulnerable = false;

        public BossPattern[] GetAttacks { get => _attacks; }
        public bool IsSpellCard { get => _isSpellCard; }
        public float HP { get => _hpOnPhase; }
        public int TimeTobeat { get => _timeToBeatInSeconds; }
        public bool IsInvulnerable { get => _isInvulnerable; }

        public BossPattern GetRandomAttack()
        {
            int attackLenght = _attacks.Length;
            if (attackLenght == 0) return null;
            int index = UnityEngine.Random.RandomRange(0, attackLenght);
            BossPattern attack = _attacks[index];
            return attack;
        }
    }
}
