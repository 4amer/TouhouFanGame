using Services.SceneLoaderC;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Services.GSMC.States
{
    public class MenuState : BaseGameState
    {
        [Inject] private SceneLoader _sceneLoader = null;
        public override void Enter()
        {
            base.Enter();
            Debug.Log("Menu Started");
            _sceneLoader.LoadScene("MenuScene");
        }
    }
}