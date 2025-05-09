using UniRx;
using UnityEngine;

namespace UI
{
    public abstract class ABaseWindow : MonoBehaviour
    {

        private bool _isShowen = true;

        public Subject<ABaseWindow> OnShow = new Subject<ABaseWindow>();
        public Subject<ABaseWindow> OnHide = new Subject<ABaseWindow>();

        public virtual void Show()
        {
            //if (_isShowen == true) return;
            gameObject.SetActive(true);
            _isShowen = true;
            OnShow?.OnNext(this);
        }

        public virtual void Hide()
        {
            //if (_isShowen == false) return;
            gameObject.SetActive(false);
            _isShowen = false;
            OnHide?.OnNext(this);
        }
    }
}
