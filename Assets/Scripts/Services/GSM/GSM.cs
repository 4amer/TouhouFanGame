using System.Collections.Generic;
using Services.GSMC.States;
using Zenject;

namespace Services.GSMC
{ 
    public class GSM : IGameStateMachine, IGSMStates
    {
        private Dictionary<string, BaseGameState> _gameStates = new Dictionary<string, BaseGameState>();

        private BaseGameState _currentState = null;
        private DiContainer _diContainer = null;

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        public void Init()
        {
            AddState(new GameState());
            AddState(new MenuState());
        }

        private void AddState(BaseGameState state)
        {
            state.Init(this);
            _diContainer.Inject(state);


            string key = state.GetType().ToString();
            if (!_gameStates.ContainsKey(key)) {
                _gameStates.Add(key, state);
            }
        }

        public void ChangeState<T>() where T : BaseGameState
        {
            if (!_gameStates.ContainsKey(typeof(T).ToString())) return;

            _currentState?.Exit();

            _currentState = _gameStates[typeof(T).ToString()];

            _currentState?.Enter();
        }
    }

    public interface IGameStateMachine
    {
        public void Init();
        public void ChangeState<T>() where T : BaseGameState;
    }
    public interface IGSMStates
    {
        public void ChangeState<T>() where T : BaseGameState;
    }
}
