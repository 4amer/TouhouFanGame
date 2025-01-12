using Services.SceneLoaderC;
using UnityEngine;
using Zenject;

namespace Services.GSMC.States
{
    public class GameState : BaseGameState
    {
        [Inject] private SceneLoader _sceneLoader = null;
        public override void Enter()
        {
            base.Enter();
            Debug.Log("Game Started");
            _sceneLoader.LoadScene("GameScene");
        }
    }
}