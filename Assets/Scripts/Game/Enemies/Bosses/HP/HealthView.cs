using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Enemies.Bosses.HP
{
    public class HealthView : MonoBehaviour, IHealthView
    {
        [SerializeField] private Image _HPBar = null;
        [SerializeField] private float _timeToChange = 0.1f;

        private float _staticHP = 1.0f;

        private CompositeDisposable _disposable = new CompositeDisposable();    

        [Inject]
        private void Contract()
        {

        }

        public void Init(IBaseBoss boss)
        {
            _staticHP = boss.GetCurrentHPAmount();

            IHealthController healthController = boss.HealthController;

            healthController
                .HPChanged
                .Subscribe(_ => HPChanged(_))
                .AddTo(_disposable);

            healthController
                .StaticHPChanged
                .Subscribe(_ => StaticHPChange(_))
                .AddTo(_disposable);
        }

        private void HPChanged(float _hp)
        {
            float percent = _hp / _staticHP;
            _HPBar.DOFillAmount(percent, _timeToChange);
        }

        private void StaticHPChange(float _hp)
        {
            _staticHP = _hp;
            RestartBar();
        }

        private void RestartBar()
        {
            _HPBar.fillAmount = 1f;
        }
    }

    internal interface IHealthView
    {
        public void Init(IBaseBoss boss);
    }
}
