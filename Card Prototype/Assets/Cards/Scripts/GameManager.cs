﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards.ScriptableObjects;
using System;
using TMPro;

namespace Cards
{
    public class GameManager : MonoBehaviour
    {
        private const float c_stepCardInDeck = 1f;

        private Material _baseMat;
        private List<CardPropertiesData> _allCards;

        public Card[] _player1Deck;
        public Card[] _player2Deck;

        [SerializeField]
        private CardPackConfiguration[] _packs;
        [SerializeField]
        private Card _cardPrefab;

        [SerializeField, Space]
        private PlayerHand _playerHand1;
        [SerializeField, Space]
        private PlayerPlayed _playerPlayed1;
        [SerializeField]
        private Transform _player1DeckRoot;

        [SerializeField, Space]
        private PlayerHand _playerHand2;
        [SerializeField]
        private Transform _player2DeckRoot;
        [SerializeField, Space]
        private PlayerPlayed _playerPlayed2;

        [SerializeField, Space, Range(0f, 200f)]
        private int _cardDeckCount = 30;

        [SerializeField, Space]
        private HealthManager _healthManager;

        [SerializeField, Space]
        private EffectManager _effectManager;

        //Картинка игрока
        [SerializeField]
        private Transform _heroPicture;
        //Материалы для картинок игрока
        //Материал мага
        [SerializeField]
        private Material _mageMat;
        //Материал виона
        [SerializeField]
        private Material _warriorMat;
        //Материал священника
        [SerializeField]
        private Material  _priestMat;
        //Материал охотника
        [SerializeField]
        private Material _hunterMat;

        //Объявление списка ID карт для формирования колоды
        public static List<uint> _koloda = new List<uint>(30);

        //Флаг быстрого старта
        public static bool _fastStart = false;

        //Переменные для поиска PlayerPlayed
        private GameObject _pp1;
        private GameObject _pp2;

        //Отслежение маны игроков
        [Header("Мана первого игрока")]
        public int _manaFirstPlayer = 1;
        [Header("Мана второго игрока")]
        public int _manaSecondPlayer = 1;

        private int _maxMana = 1;

        [SerializeField]
        private TextMeshProUGUI _manaIndicatorFirstPlayer;
        [SerializeField]
        private TextMeshProUGUI _manaIndicatorSecondPlayer;

        //Счётчик взятых карт из пустой колоды для каждого игрока
        private ushort _emptyTaken1 = 0;
        private ushort _emptyTaken2 = 0;

        private void Awake()
        {
            _pp1 = GameObject.Find("Player1Played");
            _pp2 = GameObject.Find("Player2Played");

            IEnumerable<CardPropertiesData> arrayCards = new List<CardPropertiesData>(); //Создаётся массив со структурой карты
            foreach (var pack in _packs) arrayCards = pack.UnionProperties(arrayCards); //Массивам присваевается стоимость
            _allCards = new List<CardPropertiesData>(arrayCards); 
            _baseMat = new Material(Shader.Find("TextMeshPro/Sprite"));
            _baseMat.renderQueue = 2995;
            //Проверка типа героя и выставление соответствующей герою картинки
            HeroCheck();
        }

        private void Start()
        {
            DeckCreation();
            InitialCardSetUp(_player1Deck, _playerHand1);
            InitialCardSetUp(_player2Deck, _playerHand2);
        }

        private Card[] CreateDeck(Transform root, uint _player)
        {
            //Здесь случайный набор, надо переделать в соответствии с ТЗ
            var deck = new Card[_cardDeckCount]; //Содаётся массив вместимостью 30
            var vector = Vector3.zero; //Определяестся нулевой вектор
            for (int i = 0; i<_cardDeckCount;i++) //Цикл от 0 до 30
            {
                deck[i] = Instantiate(_cardPrefab, root); //Создаётся префаб карты в заданном через инспектор месте
                deck[i].transform.localPosition = vector; //Переопредление места карты
                //if (deck[i].IsFrontSide) deck[i].SwitchEnable(); 
                vector += new Vector3(0f,c_stepCardInDeck,0f); //Подъём места следующей карты

                var random = _allCards[UnityEngine.Random.Range(0, _allCards.Count)]; //Берётся случайная карта из заранее созданного массива всех карт

                var newMat = new Material(_baseMat)
                {
                    mainTexture = random.Texture
                };
                deck[i].Configuration(random, newMat, CardUtility.GetDescriptionById(random.Id));
                //Опредение, какому игроку принадлежит карта
                deck[i]._player = _player;
            }
            return deck;
        }

        private Card[] CreateChosenDeck(Transform root, uint _player)
        {
            //Переделанный набор колоды из выбранных в предыдущей сцене
            var deck = new Card[_cardDeckCount]; //Содаётся массив вместимостью 30
            var vector = Vector3.zero; //Определяестся нулевой вектор
            for (int i = 0; i < _cardDeckCount; i++) //Цикл от 0 до 30
            {
                deck[i] = Instantiate(_cardPrefab, root); //Создаётся префаб карты в заданном через инспектор месте
                deck[i].transform.localPosition = vector; //Переопредление места карты
                //if (deck[i].IsFrontSide) deck[i].SwitchEnable(); 
                vector += new Vector3(0f, c_stepCardInDeck, 0f); //Подъём места следующей карты

                foreach (var card in _allCards)
                {
                    if ((uint)_koloda[i] == card.Id)
                    {
                        var newMat = new Material(_baseMat)
                        {
                            mainTexture = card.Texture
                        };
                        deck[i].Configuration(card, newMat, CardUtility.GetDescriptionById(card.Id));
                        //Опредение, какому игроку принадлежит карта
                        deck[i]._player = _player;
                    }
                }
            }
            return deck;
        }

        private void HeroCheck()
        {
            int _heroType = PlayerPrefs.GetInt("hero", 1);
            switch (_heroType)
            {
                case 1:
                    //Debug.Log("Mage");
                    _heroPicture.transform.GetComponent<MeshRenderer>().material = _mageMat;
                    break;
                case 2:
                    //Debug.Log("Warrior");
                    _heroPicture.transform.GetComponent<MeshRenderer>().material = _warriorMat;
                    break;
                case 3:
                    //Debug.Log("Priest");
                    _heroPicture.transform.GetComponent<MeshRenderer>().material = _priestMat;
                    break;
                case 4:
                    //Debug.Log("Hunter");
                    _heroPicture.transform.GetComponent<MeshRenderer>().material = _hunterMat;
                    break;
            }
        }

        private void ShuffleDeck(List<uint> _koloda)
        {
            int _randomNumber;
            //Создаю массив случайных чисел от 1 до 30
            List<int> _randomArray = new List<int>(_cardDeckCount);
            for (int i = 0; i < _cardDeckCount;)
            {
                _randomNumber = UnityEngine.Random.Range(0, _cardDeckCount);
                if (!_randomArray.Contains(_randomNumber))
                {
                    _randomArray.Add(_randomNumber);
                    i++;
                }
            }
            List<uint> _temp = new List<uint>(_cardDeckCount);
            for (int i = 0; i < _cardDeckCount; i++)
            {
                _temp.Add(_koloda[_randomArray[i]]);
                Debug.Log(_temp[i]);
            }
            for (int i = 0; i < _cardDeckCount; i++)
            {
               _koloda[i]=_temp[i];
            }
        }

        //Опрделяет случайную карту в колоде первого игрока
        private Card FindRandomCardInDeck()
        {
            Card _resultRandomCard = null;
            while (_resultRandomCard == null)
            {
                Card _randomCard = _player1Deck[UnityEngine.Random.Range(0, _player1Deck.Length)];
                if ((_randomCard != null) && (_randomCard.State == CardStateType.InDeck))
                {
                    _resultRandomCard = _randomCard;
                    break;
                }
            }
            if (_resultRandomCard == null)
            {
                Debug.Log("_resultRandomCard == null");
            }
            return _resultRandomCard;
        }

        public void ChangeCards(Card _card1)
        {
            var _card2 = FindRandomCardInDeck();
            Transform _position1 = _card1.transform;
            Transform _position2 = _card2.transform;
            _playerHand1.StartCoroutine(_playerHand1.MoveInHand(_card2, _position1, true, true));
            _playerHand1.StartCoroutine(_playerHand1.MoveInHand(_card1, _position2, true, true));
        }

        public void MoveInPlayedCard (Card _card, Vector3 _initialPosition, uint _player)
        {
            if ((_player == 1) && (_manaFirstPlayer >= _card._costMana))
            {
                _playerHand1.CardIsPlayed(_card); //Обнуление позиции сыгранной карты в PlayerHand
                _manaFirstPlayer -= _card._costMana;
                _playerPlayed1.MoveInPlayed(_card, _initialPosition);
                //Вызов эффекта сыгранной карты
                _effectManager.PlayEffect(_card);
            }
            else if ((_player == 2) && (_manaSecondPlayer >= _card._costMana))
            {
                _playerHand2.CardIsPlayed(_card);
                _manaSecondPlayer -= _card._costMana;
                _playerPlayed2.MoveInPlayed(_card, _initialPosition);
                //Вызов эффекта сыгранной карты
                _effectManager.PlayEffect(_card);
            }
            UpdateMana();
        }

        public void ReturnPlayedCard(Card _card)
        {
            if (_card._player == 1)
            {
                foreach (var card in _pp1.GetComponent<PlayerPlayed>()._cardsInPlayed)
                {
                    if (_card == card)
                    {
                        int _index = Array.IndexOf(_pp1.GetComponent<PlayerPlayed>()._cardsInPlayed, card);
                        _playerHand1.StartCoroutine(_playerHand1.MoveInHand(_card, _pp1.GetComponent<PlayerPlayed>()._positions[_index], false, false));
                    }
                }
            }
            else
            {
                foreach (var card in _pp2.GetComponent<PlayerPlayed>()._cardsInPlayed)
                {
                    if (_card == card)
                    {
                        int _index = Array.IndexOf(_pp2.GetComponent<PlayerPlayed>()._cardsInPlayed, card);
                        _playerHand2.StartCoroutine(_playerHand2.MoveInHand(_card, _pp2.GetComponent<PlayerPlayed>()._positions[_index], false, false));
                    }
                }
            }
        }

        //Первоначальная раздача 10 начальных карт
        private void InitialCardSetUp(Card [] _deck, PlayerHand _playerHand)
        {
            for (int j = 0; j < 9; j++)
            {
                var index = _deck.Length - 1;
                for (int i = index; i >= 0; i--)
                {
                    if (_deck[i] != null)
                    {
                        index = i;
                        break;
                    }
                }
                _playerHand.SetNewCard(_deck[index]);
                _deck[index] = null;
            }
        }

        public void TurnNewCard(Card[] _deck, PlayerHand _playerHand, ushort _playerTurn)
        {
            var index = _deck.Length - 1;
            for (int i = index; i >= 0; i--)
                {
                if ((_deck[i] != null) && (_deck[i].State == CardStateType.InDeck))
                {
                    index = i;
                    break;
                }
            }
            if (_deck[index] != null)
            {
                _playerHand.SetNewCard(_deck[index]);
                _deck[index] = null;
            }
            else
            {
                switch (_playerTurn)
                {
                    case 1:
                        _emptyTaken1 += 1;
                        _healthManager.HeroDamage(_emptyTaken1, _playerTurn);
                        break;                    
                    case 2:
                        _emptyTaken2 += 1;
                        _healthManager.HeroDamage(_emptyTaken2, _playerTurn);
                        break;
                }
            }
        }

        private void DeckCreation()
        {
            if (_fastStart == true)
            {
                _player1Deck = CreateDeck(_player1DeckRoot, 1);
            }
            else
            {
                ShuffleDeck(_koloda);
                _player1Deck = CreateChosenDeck(_player1DeckRoot,1);
            }
            _player2Deck = CreateDeck(_player2DeckRoot, 2);
        }

        //Метод для обновления показателей маны
        private void UpdateMana()
        {
            _manaIndicatorFirstPlayer.text = _manaFirstPlayer.ToString();
            _manaIndicatorSecondPlayer.text = _manaSecondPlayer.ToString();
        }

        //Метод для обновления маны
        public void ManaRenovation(uint _player)
        {
            if ((_maxMana < 10) && (_player == 1)) _maxMana += 1;
            if (_player == 1)
            {
                _manaFirstPlayer = _maxMana;
            }            
            if (_player == 2)
            {
                _manaSecondPlayer = _maxMana;
            }
            UpdateMana();
        }

        //Метод для снятия блокировки карт, пролежавших на столе ход
        public void MakePlayableAfterTurn(uint _player)
        {
            if (_player == 1)
            {
                foreach (var card in _playerPlayed1._cardsInPlayed)
                {
                    if (card != null) card._playable = true;
                }
            }
            else if (_player == 2)
            {
                foreach (var card in _playerPlayed2._cardsInPlayed)
                {
                    if (card != null) card._playable = true;
                }
            }
            else
            {
                Debug.LogError("ошибка в методе MakePlayableAfterTurn класса GameManager");
            }
        }
    }
}