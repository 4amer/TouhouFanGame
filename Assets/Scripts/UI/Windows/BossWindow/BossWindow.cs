using Enemies.Bosses.HP;
using Enemies.Bosses.SpellCards;
using Enemies.Bosses.Timer;
using Enemies.Bosses;
using UnityEngine;
using UniRx;
using Zenject;
using Stages.Manager;

namespace UI.Windows
{
    public class BossWindow : AWindow<BossWindowData>
    {
        [SerializeField] private TimerCotroller _timerController = null;
        [SerializeField] private HealthController _healthController = null;
        [SerializeField] private SpellCardManager _spellCardManager = null;

        private IBaseBoss _boss = null;
        private IStageManagerTimer _stageManagerTimer = null;

        private DiContainer _diContainer = null;

        private CompositeDisposable _disposable = new CompositeDisposable();

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        public override void Show()
        {
            base.Show();

            if (_boss == null) return;

            _diContainer.Inject(_timerController);

            _spellCardManager.Init(_boss);
            _healthController.Init(_boss);
            _timerController.Init(_stageManagerTimer, _boss);

            _timerController
                .TimeOut
                .Subscribe(_ => _boss.NextPhase())
                .AddTo(_disposable);
        }

        public override void SetData(BossWindowData data)
        {
            _boss = data.boss;
            _stageManagerTimer = data.stageManagerTimer;
        }

        public override void Hide()
        {
            base.Hide();

            _disposable?.Clear();
        }
    }
}
