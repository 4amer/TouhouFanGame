using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Enemies.Bosses.SpellCards
{
    public class SpellCardManager : MonoBehaviour
    {
        [SerializeField] private Image[] _spellCardImages = new Image[1];

        [SerializeField] private GameObject _spellCardEffect = null;

        public void Init(int spellCardAmount)
        {
            for(int i = 0; i < _spellCardImages.Length; i++)
            {
                if(i < spellCardAmount)
                {
                    _spellCardImages[i].enabled = true;
                }
                else
                {
                    _spellCardImages[i].enabled = false;
                }
            }
        }

        private void SpellCardEnd()
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

        private void ChangeEnviranvemnt()
        {

        }
    }
}