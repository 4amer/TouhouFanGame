using System.Collections;
using System.Collections.Generic;
using Services.Money;
using TMPro;
using UniRx;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace UI.Windows
{
    public class GameWindow : AWindow<GameWindowData>
    {
        [SerializeField] private TextMeshProUGUI _moneyAmount = null;

        private CompositeDisposable _disposable = new CompositeDisposable();

        private IMoneyServiceActions _moneyActions = null;

        [Inject]
        private void Construct(IMoneyServiceActions moneyService)
        {
            _moneyActions = moneyService;
        }

        public override void Show()
        {
            base.Show();

            _moneyActions
                .AmountChanged
                .Subscribe(_ => ChangeMoneyAmount(_))
                .AddTo(_disposable);
        }

        public override void SetData(GameWindowData data)
        {
            
        }

        public override void Hide()
        {
            base.Hide();

            _disposable?.Clear();
        }

        private void ChangeMoneyAmount(int amount)
        {
            _moneyAmount.text = amount.ToString();
        }
    }
}