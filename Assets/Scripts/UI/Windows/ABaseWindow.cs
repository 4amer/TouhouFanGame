using UnityEngine;

namespace UI
{
    public abstract class ABaseWindow : MonoBehaviour
    {

        private bool _isShowen = true;

        public virtual void Show()
        {
            if (_isShowen == true) return;
            gameObject.SetActive(true);
            _isShowen = true;
        }

        public virtual void Hide()
        {
            if (_isShowen == false) return;
            gameObject.SetActive(false);
            _isShowen = false;
        }
    }
}
