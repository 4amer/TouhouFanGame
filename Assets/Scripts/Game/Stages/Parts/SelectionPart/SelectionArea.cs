using TMPro;
using UniRx;
using Utils;
using UnityEngine;

namespace Stages.Parts.Selection
{
    public class SelectionArea : MonoBehaviour, ISelectionArea
    {
        [SerializeField] private SpriteRenderer _spriteRenderer = null;
        [SerializeField] private TextMeshProUGUI _partLabel = null;

        private float _timerToSelect = 0;

        private Timer _timer = null;

        public Subject<APart> OnAreaSelected { get; set; } = new Subject<APart>();
        public void Init(float timeToSelect, APart passivePart)
        {
            _timerToSelect = timeToSelect;

            _spriteRenderer.color = passivePart.PartColor;
            _partLabel.text = passivePart.PartName;

            _timer = new Timer();
            _timer.EventOnFinish = () =>
            {
                OnAreaSelected?.OnNext(passivePart);
            };
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _timer.Start();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _timer.Reset();
            }
        }
    }

    public interface ISelectionArea
    {
        public void Init(float timeToSelect, APart passivePart);
        public Subject<APart> OnAreaSelected { get; set; }
    }
}
