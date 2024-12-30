using Services.GSMC;
using Services.SceneLoaderC;
using Zenject;

namespace Installers
{
    public class ProjectContext : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindServices();
        }

        private void BindServices()
        {
            Container.BindInterfacesAndSelfTo<GSM>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SceneLoader>().AsSingle().NonLazy();
        }
    }
}
