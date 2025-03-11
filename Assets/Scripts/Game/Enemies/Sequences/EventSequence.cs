using System;
using UnityEngine;

namespace Enemies.Sequences
{
    [Serializable]
    public class EventSequence
    {
        [SerializeField] private float _actionDelay = 1f;
        [SerializeField] private EventType _type;
        private Action _event;
        public Action Event { get => _event; set => _event = value; }

        public EventType Type { get => _type; }
        public float Delay { get => _actionDelay; }
    }

    public enum EventType
    {
        None = 0,
        Shoot = 1,
        StopShoot = 2,
        LookAtPlayer = 3,
        StopLookAtPlayer = 4,
        Move = 5,
        StopMove = 6,
        Cycle = 7,
        EndCycle = 8,
    }
}
