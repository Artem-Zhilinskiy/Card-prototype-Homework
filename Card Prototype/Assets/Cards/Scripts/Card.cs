﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

namespace Cards
{
    public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IEndDragHandler, IBeginDragHandler
    {
        private const float c_scaleMult = 2f;

        [SerializeField]
        private GameObject _frontCard;
        [SerializeField]
        private MeshRenderer _picture;
        [SerializeField]
        private TextMeshPro _name;
        [SerializeField]
        private TextMeshPro _description;
        [SerializeField]
        private TextMeshPro _cost;
        [SerializeField]
        private TextMeshPro _attack;
        [SerializeField]
        private TextMeshPro _health;
        [SerializeField]
        private TextMeshPro _type;

        private uint _ID; // Для сохранения id карты и передачи её из сцены выбора в сцену игры

        //Переменная для поиска GameManager
        private GameObject _cp;

        //Флаг "была ли заменена" для выполнения требования 3 из ТЗ "Выбор стартовой руки: "Каждую карту можно заменить только раз"
        private bool _wasChanged = false;
        public CardStateType State {get; set;}

        public bool IsFrontSide => _frontCard.activeSelf;

        Vector3 _handPosition;

        private void Awake()
        {
            _cp = GameObject.Find("CenterPoint");
        }

        public void Configuration (CardPropertiesData data, Material picture, string description)
        {
            _cost.text = data.Cost.ToString();
            _picture.material = picture;
            _name.text = data.Name;
            _description.text = description;
            _attack.text = data.Attack.ToString();
            _health.text = data.Health.ToString();
            _type.text = CardUnitType.None == data.Type ? "" : data.Type.ToString();
            //Сохранение id карты для формирования игровой колоды в сцене выбора
            _ID = data.Id;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _handPosition = transform.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            switch (State)
            {
                case CardStateType.InHand:
                    //Debug.Log("Dragging.");
                    //transform.position = eventData.pointerCurrentRaycast.worldPosition;
                    Vector3 _position = eventData.pointerCurrentRaycast.worldPosition;
                    _position.y = 0;
                    transform.position = _position;
                    //transform.position = new Vector3(_position.x, 0, _position.y);
                    break;
                case CardStateType.OnTable:
                    break;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if(transform.position.z > -100)
            {
                //transform.position = new Vector3(-340, 0, -52);
                _cp.GetComponent<GameManager>().ReturnCard(this, new Vector3(-340, 0, -52)); //Позднее заменить на поиск места в PlayerPlayed
            }
            else if( transform.position.z<= -100)
            {
                //transform.position = _handPosition;
                _cp.GetComponent<GameManager>().ReturnCard(this, _handPosition);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //Здесь вы проверяете, что карта находится на столе или в руке
            //И если в руке, то проверяете количество маны и ваш ли ход
            //Если ход не ваш, то ничего не происходит.
            // Если ход ваш, то она у вас начинает двигаться
            //А двигается она через OnDrag
            switch (State)
            {
                case CardStateType.OnChoiceDeck:
                    //Debug.Log("Щелчок по карте " + _ID);
                    if (GameManager._koloda.Count >=30 )
                    {
                        Debug.Log("Колода заполнена, добавление невозможно");
                    }
                    else if (GameManager._koloda.Contains(_ID))
                    {
                        GameManager._koloda.Remove(_ID);
                        Debug.Log(GameManager._koloda.Count);
                    }
                    else
                    {
                        GameManager._koloda.Add(_ID);
                        Debug.Log(GameManager._koloda.Count);
                    }
                    break;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            switch (State)
            {
                case CardStateType.InDeck:
                    break;
                case CardStateType.InHand:
                    transform.localScale *= c_scaleMult;
                    break;
                case CardStateType.OnTable:
                    break;
                case CardStateType.OnChoiceDeck:
                    transform.localScale *= c_scaleMult;
                    break;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            switch (State)
            {
                case CardStateType.InDeck:
                    break;
                case CardStateType.InHand:
                    transform.localScale /= c_scaleMult;
                    break;
                case CardStateType.OnTable:
                    break;
                case CardStateType.OnChoiceDeck:
                    transform.localScale /= c_scaleMult;
                    break;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //throw new System.NotImplementedException();
        }

        [ContextMenu ("Switch Enable")]

        public void SwitchEnable()
        {
            var boolean = !IsFrontSide;
            _frontCard.SetActive(boolean);
            _picture.enabled = boolean;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            switch (State)
            {
                case CardStateType.InDeck:
                    break;
                case CardStateType.InHand:
                    if (eventData.button == PointerEventData.InputButton.Right)
                    {
                        if (_wasChanged == false)
                        {
                            _wasChanged = true; //Помечаем, что карта уже была заменена и больше её заменить нельзя
                            _cp.GetComponent<GameManager>().ChangeCards(this);
                        }
                        else
                        {
                            Debug.Log("Карта уже была заменена, повторная замена запрещена.");
                        }
                    }
                    break;
                case CardStateType.OnTable:
                    break;
                case CardStateType.OnChoiceDeck:
                    break;
            }
        }
    }
}