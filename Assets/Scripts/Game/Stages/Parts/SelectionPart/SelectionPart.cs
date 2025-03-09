using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Stages.Parts.Selection
{
    public class SelectionPart : APart, ISelectionPart
    {
        [SerializeField] private SelectionArea[] _selecteAreas = new SelectionArea[1];
        [SerializeField] private float _timeForSelect = 5f;
        public Subject<APart> OnPartSelected { get; set; } = new Subject<APart>();

        private ISelectionArea[] _iSelecteAreas = new ISelectionArea[1];
        private Queue<APart> _passivePartsQueue = new Queue<APart>();

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
        }

        private void PartSelected(APart part)
        {
            OnPartSelected?.OnNext(part);
        }
    }

    public interface ISelectionPart
    {
        
    }
}
