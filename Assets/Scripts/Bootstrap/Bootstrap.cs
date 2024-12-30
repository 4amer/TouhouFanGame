using Services.GSMC;
using Services.GSMC.States;
using UnityEngine;
using Zenject;

namespace BootstrapService
{
    public class Bootstrap : MonoBehaviour
    {
        private IGSM _GSM = null;

        [Inject]
        private void Construct(IGSM gSM)
        {
            _GSM = gSM;
        }

        private void Awake()
        {
            _GSM.Init();
            _GSM.ChangeState<MenuState>();
        }
    }
}
