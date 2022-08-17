using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class ChoiceDeckManager : MonoBehaviour
    {

        private Card[] _choiceDeck;

        //Метод выкладывания на стол карт из выбранного набора
        private void ShowPack()
        {
            //var deck = new Card[_packs];
        }

        private Card[] _cardsInHand;

        [SerializeField]
        private Transform[] _positions;

        private void Start()
        {
            _cardsInHand = new Card[_positions.Length];
        }

        public bool SetNewCard(Card newCard)
        {
            var result = GetLastPosition();

            if (result == -1)
            {
                Destroy(newCard.gameObject);
                return false;
            }
            _cardsInHand[result] = newCard;
            //StartCoroutine(MoveInHand(newCard, _positions[result]));
            return true;
        }

        private int GetLastPosition()
        {
            for (int i = 0; i < _cardsInHand.Length; i++)
            {
                if (_cardsInHand[i] == null) return i;
            }
            return -1;
        }
    }
}