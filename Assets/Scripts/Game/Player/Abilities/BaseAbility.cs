using UnityEngine;

namespace Game.Player.Ability
{
    public class BaseAbility : MonoBehaviour
    {
        [Space(10)]
        [Header("Shop Info")]
        [SerializeField] private int _price = 10;
        [SerializeField] private Sprite _spriteShopItem = null;

        public int GetPrice => _price;
        public Sprite GetSpriteShopItem => _spriteShopItem;

        public virtual void Init()
        {

        }

        public virtual void Attach()
        {

        }
    }
}
