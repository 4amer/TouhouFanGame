using System;
using UnityEngine;

namespace Enemies.Sequences
{
    [Serializable]
    public class ShootSequence
    {
        [SerializeField] private float _actionDelay = 1f;
        [SerializeField] private ShootEventType _type;
        private Action _event;
        public Action Event { get => _event; set => _event = value; }

        public ShootEventType Type { get => _type; }
        public float Delay { get => _actionDelay; }
    }

    public enum ShootEventType
    {
        None = 0,
        Shoot = 1,
        StopShoot = 2,
        LookAtPlayer = 3,
        StopLookAtPlayer = 4,
    }
}
