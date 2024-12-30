using Services.GSMC;
using Zenject;

namespace Installers
{
    public class ProjectContext : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindGSM();
        }

        private void BindGSM()
        {
            Container.BindInterfacesAndSelfTo<GSM>().AsSingle().NonLazy();
        }
    }
}
