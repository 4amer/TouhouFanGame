using UniRx;
using UnityEngine;

namespace UI.Windows
{
    public class AnimationEventListener : MonoBehaviour
    {
        public Subject<Unit> OnAnimationEvent { get; set; } = new Subject<Unit>();

        public void AnimationEvent()
        {
            OnAnimationEvent?.OnNext(Unit.Default);
        }
    }
}