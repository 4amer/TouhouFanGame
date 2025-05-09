using Services.SceneLoaderC;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Zenject;

namespace Services.GSMC.States
{
    public class MenuState : BaseGameState
    {
        [Inject] private SceneLoader _sceneLoader = null;
        [Inject] private PlayerInput _playerInput = null;
        public override void Enter()
        {
            base.Enter();
            Debug.Log("Menu Started");
            _sceneLoader.LoadScene("MenuScene");
            ChangeInputMap();
        }

        private void ChangeInputMap()
        {
            _playerInput.SwitchCurrentActionMap("UI");
        }
    }
}