using UniRx;
using UnityEngine;

namespace Stages.Parts.Selection
{
    public class SelectionArea : MonoBehaviour, ISelectionArea
    {
        private float _timerToSelect = 0;

        private Timer _timer = null;

        public Subject<PassivePart> AreaSelected { get; set; } = new Subject<PassivePart>();
        public void Init(float timeToSelect, PassivePart passivePart)
        {
            _timerToSelect = timeToSelect;
            
            _timer = new Timer();
            _timer.EventOnFinish = () =>
            {
                AreaSelected?.OnNext(passivePart);
            };
        }

        private void OnCollisionEnter(Collision collision)
        {
            _timer.Start();
        }

        private void OnCollisionExit(Collision collision)
        {
            _timer.Reset();
        }
    }

    public interface ISelectionArea
    {
        public void Init(float timeToSelect, PassivePart passivePart);
        public Subject<PassivePart> AreaSelected { get; set; }
    }
}
