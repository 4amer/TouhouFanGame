using Enemies;
using Enemies.Drop;
using Game.Player.Ability;
using Game.Player.Manager;
using Services.Money;
using Stages.Parts.Selection;
using Stages.Parts.Shop;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace Stages.Parts
{
    public class ShopPart : PassivePart
    {
        [SerializeField] private BaseAbility[] baseAbilities = new BaseAbility[1];

        [SerializeField] private ShopItem[] items = new ShopItem[1];

        [SerializeField] private float _timeToPickUpItem = 2f;

        [SerializeField] private Vector2Int _amountOfItemsInShop = Vector2Int.zero;

        [SerializeField] private TextMeshProUGUI _salesmanText = null;
        [SerializeField] private EntityController _salesmanEntityController = null;

        [SerializeField] private SelectionArea _selectionArea = null;

        [SerializeField] private float _timeToExit = 0f;

        private IMoneyService _moneyService = null;
        private IAbilityManager _abilityManager = null;
        private Transform _playerTransform = null;

        private CompositeDisposable _disposable = new CompositeDisposable();

        [Inject]
        private void Construct(IMoneyService moneyService, IAbilityManager abilityManager
            , IPlayerManagerTransform playerManagerTransform)
        {
            _moneyService = moneyService;
            _abilityManager = abilityManager;
            _playerTransform = playerManagerTransform.PlayerTransform;
        }

        public override void Init()
        {
            base.Init();

            int itemsAmount = Random.Range(_amountOfItemsInShop.x, _amountOfItemsInShop.y);

            foreach (ShopItem shopItem in items)
            {
                if (itemsAmount <= 0) break;

                itemsAmount++;

                int randomAbilityIndex = Random.Range(0, baseAbilities.Length);

                BaseAbility baseAbility = baseAbilities[randomAbilityIndex];

                shopItem
                    .OnPicked
                    .Subscribe(_ => ItemPicked(_))
                    .AddTo(_disposable);

                shopItem.Init(baseAbility, _timeToPickUpItem, _playerTransform);
            }

            InitSalesman();
            SetupSelectionArea();
        }

        private void SetupSelectionArea()
        {
            _selectionArea
                .OnAreaSelected
                .Subscribe(_ => Exit())
                .AddTo(_disposable);

            _selectionArea.Init(_timeToExit, null);
        }

        private void ItemPicked(ShopItem shopItem)
        {
            int currentMoney = _moneyService.GetMoneyAmount;
            int itemPrice = shopItem.GetBaseAbility.GetPrice;

            if (currentMoney <= itemPrice)
            {
                NotEnoughMoney();
                return;
            }

            EnoughMoney(shopItem);
        }

        private void EnoughMoney(ShopItem shopItem)
        {
            BaseAbility baseAbility = shopItem.GetBaseAbility;
            int itemPrice = baseAbility.GetPrice;

            shopItem.gameObject.SetActive(false);
            _moneyService.Substract(itemPrice);
            _abilityManager.AddAbility(baseAbility);
        }

        private void InitSalesman()
        {
            _salesmanEntityController.Init(_playerTransform);
            ChangeSalesmanText("Oh... Hi, again...");
        }

        private void NotEnoughMoney()
        {
            ChangeSalesmanText($"Sorry, but it seems like you don't have enough money.");
        }

        private void Exit()
        {
            Clear();
        }

        private void ChangeSalesmanText(string text)
        {
            _salesmanText.text = text;
        }

        private void OnDestroy()
        {
            _disposable?.Clear();
        }
    }
}
