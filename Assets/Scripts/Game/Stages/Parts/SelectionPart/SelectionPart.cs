using UniRx;
using UnityEngine;

namespace Stages.Parts.Selection
{
    public class SelectionPart : APart, ISelectionPart
    {
        Subject<PassivePart> ClauseSelected { get; set; } = new Subject<PassivePart>();

        [SerializeField] private GameObject[] SelecteAreas = new GameObject[1];

        private PassivePart[] _passiveParts = new PassivePart[1];
        public PassivePart[] PassiveParts { get => _passiveParts; set => _passiveParts = value; }
        public override void Init()
        {
            base.Init();

        }

        private void PartSelected(PassivePart part)
        {


            ClauseSelected?.OnNext(part);
        }
    }

    public interface ISelectionPart
    {
        
    }
}
