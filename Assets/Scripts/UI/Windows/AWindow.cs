using System.Collections;
using System.Collections.Generic;
using Testing;
using UI;
using UnityEngine;

namespace UI
{
    public abstract class AWindow<T> : ABaseWindow where T : UIData
    {
        public abstract void SetData(T data);
    }
}
