using TMPro;
using UniRx;
using UnityEngine;

namespace Enemies.Bosses.Timer
{
    public class TimerCotroller : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timerText = null;

        public Subject<Unit> TimeOut { get; set; } = new Subject<Unit>();

        private Utils.Timer _timer = null;

        public void Init()
        {
            
        }

    }
}
