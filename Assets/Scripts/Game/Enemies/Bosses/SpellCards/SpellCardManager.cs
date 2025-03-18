using Cysharp.Threading.Tasks;
using Enemies.Bosses.Phase;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Enemies.Bosses.SpellCards
{
    public class SpellCardManager : MonoBehaviour, ISpellCardManager
    {
        [SerializeField] private Image[] _spellCardImages = new Image[1];

        [SerializeField] private GameObject _spellCardEffect = null;

        private CompositeDisposable _disposable = new CompositeDisposable();

        public void Init(IBaseBoss bossActions)
        {
            bossActions
                .OnSpellCardEnd
                .Subscribe(_ => SpellCardEnd(_))
                .AddTo(_disposable);

            PrepareSpellCardImages(bossActions.GetSpellCardAmount());
        }

        private void SpellCardEnd(BossAttack attack)
        {
            for (int i = (_spellCardImages.Length - 1); i >= 0; i--)
            {
                if (_spellCardImages[i].enabled == true)
                {
                    _spellCardImages[i].enabled = false;
                    break;
                }
            }
        }

        private void PrepareSpellCardImages(int spellCardAmount)
        {
            for (int i = 0; i < _spellCardImages.Length; i++)
            {
                if (i < spellCardAmount)
                {
                    _spellCardImages[i].enabled = true;
                }
                else
                {
                    _spellCardImages[i].enabled = false;
                }
            }
        }

        private void ChangeEnviranvemnt()
        {

        }
    }

    internal interface ISpellCardManager
    {
        public void Init(IBaseBoss bossActions);
    }
}