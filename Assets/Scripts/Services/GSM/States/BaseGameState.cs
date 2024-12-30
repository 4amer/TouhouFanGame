using Zenject;

namespace Services.GSMC.States
{
    public abstract class BaseGameState
    {
        protected IGSMStates GSM = null;
        public void Init(IGSMStates gSMStates)
        {
            GSM = gSMStates;
        }
        public virtual void Enter()
        {

        }

        public virtual void Update() 
        { 
    
        }

        public virtual void Exit()
        {

        }
    }
}