using Game;
using Services.GSMC;
using Services.GSMC.States;
using UI;
using UnityEngine;
using Zenject;

namespace BootstrapService
{
    public class Bootstrap : MonoBehaviour
    {
        private IGSM _GSM = null;
        private IGameManager _GameManager = null;
        private IUIManagerInit _UIManager = null;

        [Inject]
        private void Construct(IGSM gSM, IGameManager gameManager, IUIManagerInit uIManager)
        {
            _GSM = gSM;
            _GameManager = gameManager;
            _UIManager = uIManager;
        }

        private void Awake()
        {
            _GameManager.Init();
            _UIManager.Init();

            _GSM.Init();
            _GSM.ChangeState<GameState>();
        }
    }
}
