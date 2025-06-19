using Services.SceneLoaderC;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Services.GSMC.States
{
    public class GameState : BaseGameState
    {
        [Inject] private SceneLoader _sceneLoader = null;
        [Inject] private PlayerInput _playerInput = null;
        public override void Enter()
        {
            base.Enter();
            Debug.Log("Game Started");
            _sceneLoader.LoadScene("GameScene", 5f);
            ChangeInputMap();
        }

        private void ChangeInputMap()
        {
            _playerInput.SwitchCurrentActionMap("GamePlay");
        }
    }
}