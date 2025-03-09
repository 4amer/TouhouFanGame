using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Enemies.Bosses.HP
{
    public class HealthController : MonoBehaviour, IHealthController
    {
        [SerializeField] private Image _HPBar = null;
        [SerializeField] private float _timeToChange = 0.1f;

        private float _hpAmount = 1.0f;

        [Inject]
        private void Contract()
        {

        }

        public void Init(float fullHP)
        {
            _hpAmount = fullHP;
        }

        private void HPChanged(float _hp)
        {
            float percent = _hp / _hpAmount;
            _HPBar.DOFillAmount(percent, _timeToChange);
        }
    }

    internal interface IHealthController
    {
        public void Init(float fullHP);
    }
}
