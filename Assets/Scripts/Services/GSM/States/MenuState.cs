using UnityEngine;
using UnityEngine.SceneManagement;

namespace Services.GSMC.States
{
    public class MenuState : BaseGameState
    {
        public override void Enter()
        {
            base.Enter();
            Debug.Log("Menu Started");

            SceneManager.LoadSceneAsync();
        }
    }
}