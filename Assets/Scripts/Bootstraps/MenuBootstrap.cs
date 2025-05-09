using UI;
using UI.Windows;
using UnityEngine;
using Zenject;

namespace BootstrapService
{
    public class MenuBootstrap : MonoBehaviour
    {
        private IUIManager _uIManager = null;

        [Inject]
        private void Construct(IUIManager uIManager)
        {
            _uIManager = uIManager;
        }

        private void Awake()
        {
            OpenMenuWindow();
        }

        private void OpenMenuWindow()
        {
            AWindow<MainMenuWindowData> menuWindow = _uIManager.GetWindow<MainMenuWindow>();
            _uIManager.Show(menuWindow);
        }
    }
}