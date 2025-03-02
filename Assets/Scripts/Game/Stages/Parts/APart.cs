using System;
using UniRx;
using UnityEngine;

namespace Stages.Parts
{
    public abstract class APart : MonoBehaviour, IDisposable
    {
        public Subject<APart> PartClear = new Subject<APart>();
        public Subject<APart> PartStarted = new Subject<APart>();

        protected CompositeDisposable _disposable = new CompositeDisposable();
        public virtual void Init()
        {
            PartClear.OnNext(this);
        }

        public virtual void Clear()
        {
            PartClear.OnNext(this);
        }

        public virtual void TimerUpdated(float time)
        {

        }

        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}
