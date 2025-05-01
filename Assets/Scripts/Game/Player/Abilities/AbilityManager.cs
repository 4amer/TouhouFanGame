using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Game.Player.Ability
{
    public class AbilityManager : MonoBehaviour, IAbilityManager, IAbilityManagerInit, IAbilityManagerInfo
    {
        [SerializeField] private BaseAbility BaseAbility;

        private List<BaseAbility> _abilities = new List<BaseAbility>();

        private Transform _playerTransform = null;

        private DiContainer _diContainer = null;

        public List<BaseAbility> abilities { get => _abilities; }

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        public void Init(Transform player)
        {
            _playerTransform = player;
        }

        public void AddAbility(BaseAbility baseAbility)
        {
            BaseAbility ability = CreateAbility(baseAbility);

            _abilities.Add(ability);
            ability.Init();
        }

        private BaseAbility CreateAbility(BaseAbility baseAbility)
        {
            BaseAbility ability = Instantiate(baseAbility);
            ability.enabled = false;
            _diContainer.Inject(ability);
            ability.enabled = true;
            ability.transform.parent = _playerTransform;
            return ability;
        }
    }

    public interface IAbilityManager
    {
        public void AddAbility(BaseAbility baseAbility);
    }

    public interface IAbilityManagerInfo
    {
        public List<BaseAbility> abilities { get; }
    }

    public interface IAbilityManagerInit
    {
        void Init(Transform player);
    }
}
