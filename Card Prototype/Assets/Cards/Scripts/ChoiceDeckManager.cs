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
            //_choiceDeck[i] = Instantiate(_cardPrefab, root);
        }

        [SerializeField]
        private Transform[] _positions;

        private void Awake()
        {
            _choiceDeck = new Card[_positions.Length];
        }

        public bool SetNewCard(Card newCard)
        {
            var result = GetLastPosition();

            if (result == -1)
            {
                Destroy(newCard.gameObject);
                return false;
            }
            _choiceDeck[result] = newCard;
            //Здесь нужен метод создания карты 
            //StartCoroutine(MoveInHand(newCard, _positions[result]));
            return true;
        }

        private int GetLastPosition()
        {
            for (int i = 0; i < _choiceDeck.Length; i++)
            {
                if (_choiceDeck[i] == null) return i;
            }
            return -1;
        }
    }
}