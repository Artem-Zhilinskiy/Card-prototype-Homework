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

        private void Awake()
        {
            _cp = GameObject.Find("CenterPoint");
        }

        public void EndTurn()
        {
            //Обновление маны
            _cp.GetComponent<GameManager>().ManaRenovation(_playerTurn);
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
        }
    }
}
