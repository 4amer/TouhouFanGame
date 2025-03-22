using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Player.Health
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private int _hpAmount = 3;
        [SerializeField] private Image _HPImage = null;

        [SerializeField] private int _maxHPAmount = 5;

        [SerializeField] private Ease _fillEase = Ease.Linear;
        [SerializeField] private float _fillDuration = 0.5f;

        private GameObject _player = null;

        public Subject<Unit> PlayerDead = new Subject<Unit>();

        public void Init(GameObject player)
        {
            _player = player;
        }

        public void DoDamage()
        {
            if(_hpAmount <= 0)
            {
                PlayerDead?.OnNext(Unit.Default);
                return;
            }
            _hpAmount -= 1;
            ChangeImageFill();
        }

        private void ChangeImageFill()
        {
            float fill = (((float)_hpAmount / (float)_maxHPAmount) * 100f) / 50f;
            _HPImage.DOFillAmount(fill, _fillDuration).SetEase(_fillEase);
        }
    }
}