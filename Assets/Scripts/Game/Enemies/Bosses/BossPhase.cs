using System;
using System.Runtime.CompilerServices;
using Enemies.Bosses.Attack;
using UnityEngine;

namespace Enemies.Bosses.Phase
{
    [Serializable]
    public class BossPhase
    {
        [SerializeField] private BossAttack[] _attacks = new BossAttack[1];
        [SerializeField] private bool _isSpellCard = false;
        [SerializeField] private float _hpOnPhase = 100f;
        [SerializeField] private float _timeToBeatInSeconds = 99f;
        [SerializeField] private bool _isInvulnerable = false;

        public BossAttack[] GetAttacks { get => _attacks; }
        public float HP { get => _hpOnPhase; }
        public bool IsInvulnerable { get => _isInvulnerable; }

        public BossAttack GetRandomAttack()
        {
            int attackLenght = _attacks.Length;
            if (attackLenght == 0) return null;
            int index = UnityEngine.Random.RandomRange(0, attackLenght);
            BossAttack attack = _attacks[index];
            return attack;
        }
    }
}
