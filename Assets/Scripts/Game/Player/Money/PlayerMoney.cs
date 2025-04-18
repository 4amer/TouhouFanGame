using System.Net.Http.Headers;
using Enemies.Drop;
using Player.Collision;
using Services.Money;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Player.Money
{
    public class PlayerMoney : MonoBehaviour, IPlayerMoneyInit
    {
        [SerializeField] private PlayerCollision _playerCollision = null;

        private IMoneyService _moneyService = null;

        [Inject]
        private void Construct(IMoneyService moneyService)
        {
            _moneyService = moneyService;
        }

        public void Init()
        {
            _playerCollision
                .PlayerCollided
                .Subscribe(_ => Collided(_))
                .AddTo(_playerCollision);
        }

        private void Collided(GameObject gameObject)
        {
            DropItem item = gameObject.GetComponent<DropItem>();

            if (item == null) return;

            switch (item.Type)
            {
                case EDropItemType.MoneyHeap:
                    _moneyService.Add(5);
                    break;
                case EDropItemType.MoneyCoin:
                    _moneyService.Add(1);
                    break;
            }
        }
    }

    public interface IPlayerMoneyInit
    {
        public void Init();
    }
}