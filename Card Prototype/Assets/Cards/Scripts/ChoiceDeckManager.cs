using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class ChoiceDeckManager : MonoBehaviour
    {
        private Material _baseMat;
        //private Card[][] _choiceDeck;

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
            IEnumerable<CardPropertiesData> arrayCards = new List<CardPropertiesData>(); //Создаётся массив со структурой карты
            //_choiceDeck[i] = Instantiate(_cardPrefab, root);
            //for (int i = 0; i < GameManager._packs[i]; i++)
            //for (int i = 0; i < GetComponent<GameManager>()._packs[i].); i++)
            foreach (var pack in GetComponent<GameManager>()._packs) //pack - тип CardPackConfiguration
            {
                arrayCards = pack.UnionProperties(arrayCards);
                foreach (var card in arrayCards)
                {
                    _choiceDeck[_pos,_cos] = Instantiate(GetComponent<GameManager>()._cardPrefab, _positions[_pos]);
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

        [SerializeField]
        private Transform[] _positions;

        private void Awake()
        {
            _baseMat = new Material(Shader.Find("TextMeshPro/Sprite"));
            _baseMat.renderQueue = 2995;
            ShowPack();
        }

        /*
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

        */
    }
}