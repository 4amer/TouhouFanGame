using UniRx;
using UnityEngine;

namespace Services.Money
{
    public class MoneyService : IMoneyService, IMoneyServiceActions, IMoneyServiceInit
    {
        private int _currentAmount = 0;

        public Subject<int> AmountChanged { get; set; } = new Subject<int>();
        public Subject<Unit> OnNotEnoughMoney { get; set; } = new Subject<Unit>();

        public int GetMoneyAmount { get => _currentAmount; }

        public void Init()
        {

        }

        public void Add(int amount)
        {
            _currentAmount += amount;
            AmountChanged?.OnNext(_currentAmount);
        }

        public void Substract(int amount)
        {
            int newAmount = _currentAmount -= amount;
            if(newAmount < 0)
            {
                OnNotEnoughMoney?.OnNext(Unit.Default);
                return;
            }
            _currentAmount = newAmount;
            AmountChanged?.OnNext(_currentAmount);
        }
    }

    public interface IMoneyServiceInit
    {
        public void Init();
    }

    public interface IMoneyServiceActions
    {
        public Subject<int> AmountChanged { get; set; }
        public Subject<Unit> OnNotEnoughMoney { get; set; }
    }

    public interface IMoneyService
    {
        public int GetMoneyAmount { get; }
        public void Add(int amount);
        public void Substract(int amount);
    }
}