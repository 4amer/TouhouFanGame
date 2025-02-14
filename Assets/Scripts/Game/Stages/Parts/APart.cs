using UniRx;
using UnityEngine;

namespace Stages.Parts
{
    public abstract class APart : MonoBehaviour
    {
        public Subject<APart> PartClear = new Subject<APart>();
        public Subject<APart> PartStarted = new Subject<APart>();
        public virtual void Init()
        {
            PartStarted.OnNext(this);
        }
        public virtual void Clear()
        {
            PartClear.OnNext(this);
        }
    }
}
