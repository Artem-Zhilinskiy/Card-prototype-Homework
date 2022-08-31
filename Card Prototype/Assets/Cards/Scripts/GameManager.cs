using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards.ScriptableObjects;

namespace Cards
{
    public class GameManager : MonoBehaviour
    {
        private const float c_stepCardInDeck = 1f;

        private Material _baseMat;
        private List<CardPropertiesData> _allCards;

        private Card[] _player1Deck;
        private Card[] _player2Deck;

        [SerializeField]
        //private CardPackConfiguration[] _packs;
        public CardPackConfiguration[] _packs;
        [SerializeField]
        //public Card _cardPrefab { get; private set; }
        public Card _cardPrefab;

        [SerializeField, Space]
        private PlayerHand _playerHand1;
        [SerializeField]
        private Transform _player1DeckRoot;

        [SerializeField, Space]
        private PlayerHand _playerHand2;
        [SerializeField]
        private Transform _player2DeckRoot;

        [SerializeField, Space, Range(0f, 200f)]
        private int _cardDeckCount = 30;

        private void Awake()
        {
            IEnumerable<CardPropertiesData> arrayCards = new List<CardPropertiesData>(); //Создаётся массив со структурой карты
            foreach (var pack in _packs) arrayCards = pack.UnionProperties(arrayCards); //Массивам присваевается стоимость
            _allCards = new List<CardPropertiesData>(arrayCards); // Почему-то массив переприсваивается?
            _baseMat = new Material(Shader.Find("TextMeshPro/Sprite"));
            _baseMat.renderQueue = 2995;
        }

        private void Start()
        {
            _player1Deck = CreateDeck(_player1DeckRoot);
            _player2Deck = CreateDeck(_player2DeckRoot);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                var index = _player1Deck.Length - 1;
                for (int i = index; i >= 0; i--)
                {
                    if (_player1Deck[i] != null)
                    {
                        index = i;
                        break;
                    }
                }
                _playerHand1.SetNewCard(_player1Deck[index]);
                _player1Deck[index] = null;
            }
        }

        private Card[] CreateDeck(Transform root)
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

                var random = _allCards[Random.Range(0, _allCards.Count)]; //Берётся случайная карта из заранее созданного массива всех карт

                var newMat = new Material(_baseMat)
                {
                    mainTexture = random.Texture
                };
                deck[i].Configuration(random, newMat, CardUtility.GetDescriptionById(random.Id));
            }
            return deck;
        }
    }
}