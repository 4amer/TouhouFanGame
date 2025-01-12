using Game;
using Services.GSMC;
using Services.GSMC.States;
using UnityEngine;
using Zenject;

namespace BootstrapService
{
    public class Bootstrap : MonoBehaviour
    {
        private IGSM _GSM = null;
        private IGameManager _GameManager = null;

        [Inject]
        private void Construct(IGSM gSM, IGameManager gameManager)
        {
            _GSM = gSM;
            _GameManager = gameManager;
        }

        private void Awake()
        {
            _GameManager.Init();

            _GSM.Init();
            _GSM.ChangeState<GameState>();
        }
    }
}
