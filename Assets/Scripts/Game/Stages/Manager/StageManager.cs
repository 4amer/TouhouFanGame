using Stages.Parts;
using Unity.VisualScripting;
using UnityEngine;

namespace Stages.Manager
{
    public class StageManager : MonoBehaviour, IStageManager
    {
        [SerializeField] private BaseStage[] _stages = new BaseStage[6];

        [SerializeField] private int _stageIndex = 0;

        [SerializeField] private Transform _partsParentTransform = null;

        private IBaseStage _currentStage = default;
        public void Init()
        {
            SetupTimer();
            InitStage(_stageIndex);
        }

        private void InitStage(int index)
        {
            _currentStage = _stages[index];
        }

        private void WhenStageClear(IBaseStage stage)
        {
            NextState();
        }

        private void NextState()
        {

        }

        private void SetupTimer()
        {

        }
    }

    public interface IStageManager 
    {
        public void Init();
    }
}
