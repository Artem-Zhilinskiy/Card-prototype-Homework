using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cards
{
    public class TurnManager : MonoBehaviour
    {
        //Переменная для поиска GameManager
        private GameObject _cp;

        [SerializeField, Space]
        private PlayerHand _playerHand1;
        [SerializeField, Space]
        private PlayerHand _playerHand2;

        public ushort _playerTurn = 1;

        //Флаг первого хода, в который второй игрок не берёт карту.
        private bool _isNotFirstRound = false;

        //Переменные для поиска PlayerPlayed
        private GameObject _pp1;
        private GameObject _pp2;

        //Счётчик увеличения силы миньонов
        public ushort _murlockBuff = 0;

        private void Awake()
        {
            _cp = GameObject.Find("CenterPoint");
            _pp1 = GameObject.Find("Player1Played");
            _pp2 = GameObject.Find("Player2Played");
        }

        public void EndTurn()
        {
            //Обновление маны
            _cp.GetComponent<GameManager>().ManaRenovation(_playerTurn);
            _cp.GetComponent<GameManager>().MakePlayableAfterTurn(_playerTurn); //Снятие блока с карт, пролежавших ход и сыгранных в прошлом ходу
            //Смена хода
            switch (_playerTurn)
            { 
                case 1:
                    if (_isNotFirstRound)
                    {
                        _cp.GetComponent<GameManager>().TurnNewCard(_cp.GetComponent<GameManager>()._player2Deck, _playerHand2, _playerTurn);
                        _playerTurn = 2;
                    }
                    else
                    {
                        _isNotFirstRound = true;
                        _playerTurn = 2;
                    }
                    break;   
                case 2:
                    _cp.GetComponent<GameManager>().TurnNewCard(_cp.GetComponent<GameManager>()._player1Deck, _playerHand1, _playerTurn);
                    _playerTurn = 1;
                    break;
            }
            Debug.Log("Ход игрока " + _playerTurn);
            CheckMurlockBuff(_playerTurn);
        }

        private ushort CheckMurlockBuff(ushort _player)
        {
            _murlockBuff = 0;
            switch (_player)
            {
                case 1:
                    foreach (var card in _pp1.GetComponent<PlayerPlayed>()._cardsInPlayed)
                    {
                        if ((card != null) && ((card._ID == 101) || (card._ID == 305) ||  (card._ID == 702))) _murlockBuff += 1;
                    }
                    break;
                case 2:
                    foreach (var card in _pp2.GetComponent<PlayerPlayed>()._cardsInPlayed)
                    {
                        if ((card != null) && ((card._ID == 101) || (card._ID == 305) || (card._ID == 702))) _murlockBuff += 1;
                    }
                    break;
            }
            return _murlockBuff;
        }
    }
}
