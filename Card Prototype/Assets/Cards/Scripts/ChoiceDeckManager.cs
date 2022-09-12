using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards.ScriptableObjects;

namespace Cards
{
    public class ChoiceDeckManager : MonoBehaviour
    {
        private Material _baseMat;
        [SerializeField]
        private Transform _deck;
        private const float c_stepCardInDeck = 1f;
        private List<CardPropertiesData> _packCards;
        private List<CardPropertiesData> _classCards;
        [SerializeField]
        private Transform[] _positions;

        [SerializeField]
        private CardPackConfiguration[] _packs;
        [SerializeField]
        private Card _cardPrefab;

        private Card[,] _choiceDeck = new Card[9, 8];
        int _pos = 0;
        int _cos = 0;

        private SideType _hero; //Выбор героя

        [SerializeField]
        private GameObject _panelHero;
        [SerializeField]
        private GameObject _panelCards;

        //Метод выкладывания на стол карт из выбранного набора
        //1. Создать через Instantiate(что и где)
        //2. Переопределить её место?
        //3. Вызвать Card.Configuration (Передать CardProprtiesData, картинку и описание)
        //На каждой карте висит скрипт Card!
        private void ShowPack()
        {
            var vector = _positions[0].transform.localPosition;
            IEnumerable<CardPropertiesData> arrayCards = new List<CardPropertiesData>(); //Создаётся массив со структурой карты
            for (int i = 0; i < 7; i++)
            {
                arrayCards = _packs[i].ReplaceProperties(arrayCards);
                _packCards = new List<CardPropertiesData>(arrayCards); // Почему-то массив переприсваивается?
                foreach (var card in arrayCards)
                {
                    vector = _positions[_pos].transform.localPosition;
                    vector += Vector3.up;
                    _choiceDeck[_pos, _cos] = Instantiate(_cardPrefab, _deck);
                    _choiceDeck[_pos, _cos].transform.localPosition = vector;
                    //_choiceDeck[_pos, _cos].transform.Rotate(180.0f, 180.0f, 0.0f, Space.Self); //разворачиваю лицом к игроку
                    _choiceDeck[_pos, _cos].State = CardStateType.InHand;//Для возможности скалирования
                    //Debug.Log(_choiceDeck[_pos, _cos].State);
                    _choiceDeck[_pos, _cos].transform.gameObject.SetActive(false); //отключаю видимость
                    var newMat = new Material(_baseMat)
                    {
                        mainTexture = card.Texture
                    };
                    _choiceDeck[_pos, _cos].Configuration(card, newMat, CardUtility.GetDescriptionById(card.Id));
                    _pos++;
                    //Debug.Log(card.Name);
                }
                _pos = 0;
                _cos++;
            }
        }

        private void Awake()
        {
            _baseMat = new Material(Shader.Find("TextMeshPro/Sprite"));
            _baseMat.renderQueue = 2995;
            //Выкладка общих карт
            ShowPack();
        }

        //метод выкладки классовых карт
        //public void ShowClassPack(SideType _hero)
        public void SetClassPack(int h)
        {
            _hero = TranslateIntToSideType(h);
            IEnumerable<CardPropertiesData> arrayCards = new List<CardPropertiesData>();
            foreach (var pack in _packs)
            {
                if (pack._sideType == _hero)
                {
                    arrayCards = pack.UnionProperties(arrayCards);
                }
            }
            _classCards = new List<CardPropertiesData>(arrayCards); // Почему-то массив переприсваивается?
            _baseMat = new Material(Shader.Find("TextMeshPro/Sprite"));
            _baseMat.renderQueue = 2995;
            var vector = _positions[0].transform.localPosition;
            _pos = 0;
            _cos = 7;
            foreach (var card in _classCards)
            {
                vector = _positions[_pos].transform.localPosition;
                _choiceDeck[_pos, _cos] = Instantiate(_cardPrefab, _deck);
                _choiceDeck[_pos, _cos].transform.localPosition = vector;
                _choiceDeck[_pos, _cos].State = CardStateType.InHand;//Для возможности скалирования
                //_choiceDeck[_pos, _cos].transform.Rotate(180.0f, 180.0f, 0.0f, Space.Self); //разворачиваю лицом к игроку
                _choiceDeck[_pos, _cos].transform.gameObject.SetActive(false); //отключаю видимость
                var newMat = new Material(_baseMat)
                {
                    mainTexture = card.Texture
                };
                _choiceDeck[_pos, _cos].Configuration(card, newMat, CardUtility.GetDescriptionById(card.Id));
                _pos++;
                //Debug.Log(card.Name);
            }
            PanelTrigger();
        }

        #region Блок показа и скрытия карт

        public void ShowCostPack(int _cost)
        {
            //Отключить все карты
            DisableAllCards();
            //Включить карты одной стоимости
            for (int pos = 0; pos < 8; pos++)
            {
                if (_choiceDeck[pos, _cost - 1] != null)
                {
                    _choiceDeck[pos, _cost - 1].transform.gameObject.SetActive(true);
                }
            }
        }

        public void ShowClassPack()
        {
            //Отключить все карты
            DisableAllCards();
            //Включить карты одной стоимости
            for (int pos = 0; pos < 8; pos++)
            {
                if (_choiceDeck[pos,7] != null)
                {
                    _choiceDeck[pos, 7].transform.gameObject.SetActive(true);
                }
            }
        }

        private void DisableAllCards()
        {
            foreach (var card in _choiceDeck)
            {
                if (card != null)
                {
                    card.transform.gameObject.SetActive(false);
                }
            }
        }

        #endregion

        //Метод переключения панелей
        public void PanelTrigger()
        {
            _panelHero.SetActive(false);
            _panelCards.SetActive(true);
        }

        private SideType TranslateIntToSideType(int h)
        {
            switch (h)
            {
                case 1:
                    return SideType.Mage;
                    break;
                case 2:
                    return SideType.Warrior;
                    break;
                case 3:
                    return SideType.Priest;
                    break;
                case 4:
                    return SideType.Hunter;
                    break;
                default:
                    return SideType.Common;
                    break;
            }
        }
    }
}