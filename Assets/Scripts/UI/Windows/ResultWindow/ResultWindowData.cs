using System;
using UnityEngine;

namespace UI.Windows
{
    public class ResultWindowData : UIData
    {
        public float timeInSeconds = 0f;
        public Action OnGoToMenu;
    }
}