using Enemies;
using Game.Player.Ability;
using TMPro;
using UniRx;
using UnityEngine;

namespace Stages.Parts.Shop
{
    public class ShopItem : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer = null;
        [SerializeField] private TextMeshProUGUI _textMeshProUGUI = null;
        [SerializeField] private EntityController _entityController = null;

        private int _price = 0;
        private BaseAbility _baseAbility = null;

        private Utils.Timer _timer = new Utils.Timer();

        public BaseAbility GetBaseAbility => _baseAbility;

        public Subject<ShopItem> OnPicked { get; set; } = new Subject<ShopItem>();

        public void Init(BaseAbility ability, float timeToPickUpItem, Transform playerTransform)
        {
            _entityController.Init(playerTransform);

            _baseAbility = ability;

            _spriteRenderer.sprite = _baseAbility.GetSpriteShopItem;
            _price = _baseAbility.GetPrice;
            _textMeshProUGUI.text = $"{_price}";

            _timer = new Utils.Timer();
            _timer.duration = timeToPickUpItem;
            _timer.EventOnFinish += () =>
            {
                OnPicked?.OnNext(this);
            };
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _timer.Start();
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _timer.Reset();
            }
        }
    }
}
