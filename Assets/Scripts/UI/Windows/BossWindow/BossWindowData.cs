using Enemies.Bosses;
using Stages.Manager;

namespace UI.Windows
{
    public class BossWindowData : UIData
    {
        public IBaseBoss boss = null;
        public IStageManagerTimer stageManagerTimer = null;
    }
}
