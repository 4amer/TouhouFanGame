using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class UIManager : MonoBehaviour, IUIManagerInit, IUIManager
    {
        [SerializeField] private Canvas _canvasPrefab = null;

        [SerializeField] private ABaseWindow[] _windows = null;

        private Canvas _canvas = null;

        private Dictionary<string, ABaseWindow> _windowsDictionaty = new Dictionary<string, ABaseWindow>();
        private Dictionary<string, ABaseWindow> _shownWindows = new Dictionary<string, ABaseWindow>();

        private DiContainer _diContainer = null;

        public Subject<ABaseWindow> OnShowWindow { get; set; } = new Subject<ABaseWindow>();
        public Subject<ABaseWindow> OnHideWindow { get; set; } = new Subject<ABaseWindow>();

        private bool _isInited = false;

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        public void Init()
        {
            if(_isInited) return;
            _isInited = true;
            PrepareCanvas();
            InitWindows();
        }

        public T GetWindow<T>() where T : ABaseWindow
        {
            ABaseWindow window = null;
            _windowsDictionaty.TryGetValue(typeof(T).ToString(), out window);
            return (T)window;
        }

        public void Show(ABaseWindow window) 
        {
            window.Show();
            _shownWindows.Add(window.GetType().ToString(), window);
        }

        public void Hide<T>() where T : ABaseWindow
        {
            ABaseWindow window = null;
            _shownWindows.TryGetValue(typeof(T).ToString(), out window);
            if (window == null)
            {
                Debug.LogWarning($"{typeof(T)} window is no exist!");
                return;
            }
            _shownWindows.Remove(typeof(T).ToString());
            window?.Hide();
        }

        private void InitWindows()
        {
            foreach (ABaseWindow baseWindow in _windows)
            { 
                string key = baseWindow.GetType().ToString();

                ABaseWindow newWindow = SpawnWindow(baseWindow);

                _windowsDictionaty.Add(key, newWindow);
            }
        }

        private ABaseWindow SpawnWindow(ABaseWindow window)
        {
            ABaseWindow baseWindow = Instantiate(window, _canvas.transform);

            _diContainer.Inject(baseWindow);

            return baseWindow;
        }

        private void PrepareCanvas()
        {
            if (_canvas == null)
            {
                GameObject canvasGO = new GameObject("MainCanvas");
                _canvas = canvasGO.AddComponent<Canvas>();
                _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<CanvasScaler>();
                canvasGO.AddComponent<GraphicRaycaster>();
                canvasGO.transform.parent = this.transform;

                CanvasScaler canvasScaler = _canvas.GetComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(1920, 1080);
            }
        }
    }

    public interface IUIManagerInit
    {
        public void Init();
    }

    public interface IUIManager
    {
        public T GetWindow<T>() where T : ABaseWindow;
        public void Show(ABaseWindow window);
        public void Hide<T>() where T : ABaseWindow;

    }
}
