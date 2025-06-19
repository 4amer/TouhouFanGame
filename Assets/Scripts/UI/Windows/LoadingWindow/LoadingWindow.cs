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
        [SerializeField] private float _timeToHide = 5f;

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
            float time = data.timeToHide;
            if (time > -1f)
            {
                _timeToHide = time;
            }
        }

        private void CloseLoading()
        {
            _cirnoAnimator.SetBool("Open", false);
            Utils.Timer timer = new Utils.Timer();
            timer.SetupDelayWithAction(_timeToHide, () =>
            {
                Hide();
            });
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}
