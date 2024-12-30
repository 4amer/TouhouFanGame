using System.Collections.Generic;
using Services.GSMC.States;

namespace Services.GSMC
{ 
    public class GSM : IGSM, IGSMStates
    {
        private Dictionary<string, BaseGameState> _gameStates = new Dictionary<string, BaseGameState>();

        private BaseGameState _currentState = null;

        public void Init()
        {
            AddState(new GameState());
            AddState(new MenuState());
        }

        private void AddState(BaseGameState state)
        {
            state.Init(this);

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

    public interface IGSM
    {
        public void Init();
        public void ChangeState<T>() where T : BaseGameState;
    }
    public interface IGSMStates
    {
        public void ChangeState<T>() where T : BaseGameState;
    }
}
