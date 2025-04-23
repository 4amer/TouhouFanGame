using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using Services.SceneLoaderC;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI.Windows
{
    public class LoadingWindow : AWindow<LoadingWindowData>
    {
        [SerializeField] private Animator _cirnoAnimator = null;

        private CompositeDisposable _disposable = new CompositeDisposable();    

        [Inject]
        private void Construct(ISceneLoaderActions sceneLoaderActions)
        {
            sceneLoaderActions
                 .SceneEndLoad
                 .Subscribe(_ => CloseLoading())
                 .AddTo(_disposable);
        }

        public override void Show()
        {
            base.Show();
            _cirnoAnimator.SetBool("Open", true);
        }

        public override void SetData(LoadingWindowData data)
        {

        }

        private void CloseLoading()
        {
            _cirnoAnimator.SetBool("Open", false);
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}
