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
        [SerializeField]
        private Transform[] _positions;

        [SerializeField]
        private CardPackConfiguration[] _packs;
        [SerializeField]
        private Card _cardPrefab;

        //Метод выкладывания на стол карт из выбранного набора
        //1. Создать через Instantiate(что и где)
        //2. Переопределить её место?
        //3. Вызвать Card.Configuration (Передать CardProprtiesData, картинку и описание)
        //На каждой карте висит скрипт Card!
        private void ShowPack()
        {
            int _pos = 0;
            int _cos = 0;
            var _choiceDeck = new Card[9,8];
            var vector = _positions[0].transform.localPosition;
            IEnumerable<CardPropertiesData> arrayCards = new List<CardPropertiesData>(); //Создаётся массив со структурой карты
            for (int i=0; i<7;i++) 
            {
                arrayCards = _packs[i].ReplaceProperties(arrayCards);
                _packCards = new List<CardPropertiesData>(arrayCards); // Почему-то массив переприсваивается?
                foreach (var card in arrayCards)
                {
                    vector = _positions[_pos].transform.localPosition;
                    _choiceDeck[_pos,_cos] = Instantiate(_cardPrefab, _deck);
                    _choiceDeck[_pos, _cos].transform.localPosition = vector;
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
                _pos = 0;
                _cos++;
            }
        }

        private void Awake()
        {
            _baseMat = new Material(Shader.Find("TextMeshPro/Sprite"));
            _baseMat.renderQueue = 2995;
            ShowPack();
        }
    }
}