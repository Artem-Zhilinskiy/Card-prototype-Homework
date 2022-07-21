using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards.ScriptableObjects;

namespace Cards
{
    public class GameManager : MonoBehaviour
    {
        private Material _baseMat;
        private List<CardPropertiesData> _allCards;

        [SerializeField]
        private CardPackConfiguration[] _packs;
        [SerializeField]
        private Card _cradPrefab;

        [SerializeField, Space, Range(0f, 200f)]
        private int _cardDeckCount = 30;

        private void Awake()
        {
            IEnumerable<CardPropertiesData> arrayCards = new List<CardPropertiesData>();
            foreach (var pack in _packs) arrayCards = pack.UnionProperties(arrayCards);
            _allCards = new List<CardPropertiesData>(arrayCards);
            _baseMat = new Material(Shader.Find("TextMeshPro/Sprite"));
            _baseMat.renderQueue = 2995;
        }
    }
}