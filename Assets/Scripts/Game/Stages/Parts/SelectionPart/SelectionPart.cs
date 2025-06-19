using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Stages.Parts.Selection
{
    public class SelectionPart : APart, ISelectionPart
    {
        [SerializeField] private SelectionArea[] _selecteAreas = new SelectionArea[1];
        [SerializeField] private float _timeForSelect = 3f;
        [SerializeField] private float _partTime = 10f;

        [SerializeField] private Image _clockImage = null;

        public Subject<APart> OnPartSelected { get; set; } = new Subject<APart>();

        private ISelectionArea[] _iSelecteAreas = new ISelectionArea[1];
        private Queue<APart> _passivePartsQueue = new Queue<APart>();

        private Utils.Timer _timer = null;

        public void PrepareParts(APart[] PassiveParts)
        {
            _passivePartsQueue = new Queue<APart>(PassiveParts);
        }

        public override void Init()
        {
            base.Init();
            _iSelecteAreas = _selecteAreas;
            foreach (ISelectionArea area in _iSelecteAreas)
            {
                if (_passivePartsQueue.Count == 0) break;
                APart part = _passivePartsQueue.Dequeue();

                area.OnAreaSelected
                    .Subscribe(_ => PartSelected(_))
                    .AddTo(disposable);

                area.Init(_timeForSelect, part);
            }

            _timer = new Utils.Timer(); 

            _timer.duration = _partTime;
            _timer.OnTimerFinish += () => {
                Clear();
            };
            _timer.Start();
            DoClockAnimation();
        }

        public void DoClockAnimation()
        {
            _clockImage.DOFillAmount(0f, _partTime).SetEase(Ease.Linear);
        }

        private void PartSelected(APart part)
        {
            _timer.Reset();
            OnPartSelected?.OnNext(part);
        }
    }

    public interface ISelectionPart
    {
        
    }
}
