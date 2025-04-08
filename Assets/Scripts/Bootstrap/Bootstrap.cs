using Audio;
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
        private IGameStateMachine _gameStateMachine = null;
        private IGameManagerInit _gameManager = null;
        private IUIManagerInit _uIManager = null;
        private IAudioManagerInit _audioManager = null;

        [Inject]
        private void Construct(IGameStateMachine gameStateMachine, IGameManagerInit gameManager, IUIManagerInit uIManager, IAudioManagerInit audioManager)
        {
            _gameStateMachine = gameStateMachine;
            _gameManager = gameManager;
            _uIManager = uIManager;
            _audioManager = audioManager;
        }

        private void Awake()
        {
            _gameManager.Init();
            _uIManager.Init();
            _audioManager.Init();

            _gameStateMachine.Init();
            _gameStateMachine.ChangeState<GameState>();
        }
    }
}
