using UnityEngine;

namespace Enemies.Bosses.Attack
{
    public class BossAttack : MonoBehaviour
    {
        [SerializeField] private GameObject _attackObject = null;

        public GameObject GetAttackObject { get => _attackObject; }
    }
}
