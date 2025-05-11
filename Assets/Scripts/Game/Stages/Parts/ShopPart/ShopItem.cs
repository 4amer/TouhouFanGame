using DG.Tweening;
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

        [SerializeField] private float _rotateinDegree = 10f;
        [SerializeField] private float _rotationTime = 1f;

        private int _price = 0;
        private BaseAbility _baseAbility = null;

        private Utils.Timer _timer = new Utils.Timer();

        public BaseAbility GetBaseAbility => _baseAbility;

        public Subject<ShopItem> OnPicked { get; set; } = new Subject<ShopItem>();

        private Sequence _wiggleTween = null;

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
            SetupTween();
        }

        private void SetupTween()
        {
            Vector3 rot1 = new Vector3(0, 0, _rotateinDegree);
            Vector3 rot2 = new Vector3(0, 0, -_rotateinDegree);

            _wiggleTween = DOTween.Sequence();

            _wiggleTween.Append(transform.DOLocalRotate(rot1, _rotationTime).SetEase(Ease.InOutCubic))
                .Append(transform.DOLocalRotate(rot2, _rotationTime).SetEase(Ease.InOutCubic))
                .SetLoops(-1);

            _wiggleTween.Restart();
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

        private void Dispose()
        {
            _wiggleTween.Pause();
        }

        public void OnDisable()
        {
            Dispose();
        }

        public void OnDestroy()
        {
            Dispose();
        }
    }
}
