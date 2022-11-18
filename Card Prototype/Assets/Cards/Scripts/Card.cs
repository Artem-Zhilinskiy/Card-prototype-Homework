using System.Collections;
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

        public uint _ID; // Для сохранения id карты и передачи её из сцены выбора в сцену игры
        public uint _player; // Для определения игрока и управлением картой
        private ushort _dynamicHealth; //Для отслеживания здоровья карты
        private ushort _ushortAttack; //Для отслеживания здоровья карты
        public int _costMana; // Для отслеживания маны у игроков

        //Переменная для поиска GameManager
        private GameObject _cp;

        //Переменные для поиска PlayerPlayed
        private GameObject _pp1;
        private GameObject _pp2;

        //Переменные для поиска изображений героев
        private GameObject _hero1;
        private GameObject _hero2;


        //Атакованная карта
        private Card _attackedCard;

        //Флаг "была ли заменена" для выполнения требования 3 из ТЗ "Выбор стартовой руки: "Каждую карту можно заменить только раз"
        private bool _wasChanged = false;

        //Флаг, прошёл ли ход с момента выкладывания карты на стол
        public bool _playable = false;
        public CardStateType State {get; set;}

        public bool IsFrontSide => _frontCard.activeSelf;

        Vector3 _handPosition;

        //Для отключения карт во время хода другого игрка
        public bool _isPlayerTurn = false;

        private void Awake()
        {
            _cp = GameObject.Find("CenterPoint");
            _pp1 = GameObject.Find("Player1Played");
            _pp2 = GameObject.Find("Player2Played");
            _hero1 = GameObject.Find("Player1Head");
            _hero2 = GameObject.Find("Player2Head");
        }

        public void Configuration (CardPropertiesData data, Material picture, string description)
        {
            _cost.text = data.Cost.ToString();
            _picture.material = picture;
            _name.text = data.Name;
            _description.text = description;
            _attack.text = data.Attack.ToString();
            _health.text = data.Health.ToString();
            _dynamicHealth = data.Health;
            _type.text = CardUnitType.None == data.Type ? "" : data.Type.ToString();
            //Сохранение id карты для формирования игровой колоды в сцене выбора
            _ID = data.Id;
            //Определение героя для игрового прцесса
            _player = data._player;
            _ushortAttack = data.Attack;
            _costMana = data.Cost;
            _playable = Charge(); //Можно ли разыгрывать эту карту сразу после выкладывания на стол.
        }

        private bool CheckTurnAndMana()
        {
            if (GameObject.Find("TurnCanvas").GetComponent<TurnManager>()._playerTurn == _player) //Если сейчас ход того игрока, чья карта
            {
                switch (GameObject.Find("TurnCanvas").GetComponent<TurnManager>()._playerTurn) //У этого игрока
                {
                    case 1:
                        if (_cp.GetComponent<GameManager>()._manaFirstPlayer >= _costMana)
                        {
                            return true;
                        }
                        else return false;
                    case 2:
                        if (_cp.GetComponent<GameManager>()._manaSecondPlayer >= _costMana)
                        {
                            return true;
                        }
                        else return false;
                    default:
                        return false;
                }
            }
            else return false;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if ((GameObject.Find("TurnCanvas").GetComponent<TurnManager>()._playerTurn == _player) && (_playable))
            {
                _handPosition = transform.position;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 _position;
                switch (State)
                {
                    case CardStateType.InHand:
                    if (CheckTurnAndMana())
                    {
                        _position = eventData.pointerCurrentRaycast.worldPosition;
                        _position.y = 0;
                        transform.position = _position;
                        //transform.position = new Vector3(_position.x, 0, _position.y);
                    }
                    break;
                    case CardStateType.OnTable:
                    if ((GameObject.Find("TurnCanvas").GetComponent<TurnManager>()._playerTurn == _player) && (_playable))
                    {
                        _handPosition = transform.position;
                        _position = eventData.pointerCurrentRaycast.worldPosition;
                        _position.y = 0;
                        transform.position = _position;
                    }
                    break;
                }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (GameObject.Find("TurnCanvas").GetComponent<TurnManager>()._playerTurn == _player)
            {
                switch (State)
                {
                    case CardStateType.InHand:
                        _cp.GetComponent<GameManager>().MoveInPlayedCard(this, _handPosition, _player);
                        break;
                    case CardStateType.OnTable:
                        //Определение дистанции до одного из positions PlayerPlayed
                        if ((_player == 1) && (_playable))
                        {
                            float _distance = Vector3.Distance(transform.position, _hero2.transform.position);
                            foreach (var card in _pp2.GetComponent<PlayerPlayed>()._cardsInPlayed)
                            {
                                if (card != null)
                                {
                                    //Проверка, действует ли у противника провокация
                                    if (Taunted(card))
                                    {
                                        _attackedCard = card;
                                        break;
                                    }
                                    else
                                    {
                                        var _tempDistance = Vector3.Distance(transform.position, card.transform.position);
                                        if (_tempDistance < _distance)
                                        {
                                            _distance = _tempDistance;
                                            _attackedCard = card;
                                        }
                                    }
                                }
                            }
                        }
                        else if (_playable) //Модуль игрока 2
                        {
                            float _distance = Vector3.Distance(transform.position, _hero1.transform.position);
                            foreach (var card in _pp1.GetComponent<PlayerPlayed>()._cardsInPlayed)
                            {
                                if (card != null)
                                {
                                    //Проверка, действует ли у противника провокация
                                    if (Taunted(card))
                                    {
                                        _attackedCard = card;
                                        break;
                                    }
                                    else
                                    {
                                        var _tempDistance = Vector3.Distance(transform.position, card.transform.position);
                                        if (_tempDistance < _distance)
                                        {
                                            _distance = _tempDistance;
                                            _attackedCard = card;
                                        }
                                    }
                                }
                            }
                        }
                        if ((_attackedCard != null) && (_attackedCard._dynamicHealth > _ushortAttack))
                        {
                            _attackedCard._dynamicHealth -= _ushortAttack;
                            _attackedCard._health.text = _attackedCard._dynamicHealth.ToString();
                        }
                        else if (_attackedCard != null)
                        {
                            _attackedCard._dynamicHealth = 0;
                        }
                        else if ((_attackedCard == null) && (_playable))
                        {
                            _cp.GetComponent<HealthManager>().HeroDamage(_ushortAttack, _player);
                        }
                        _cp.GetComponent<GameManager>().ReturnPlayedCard(this);
                        if ((_attackedCard != null) && (_attackedCard._dynamicHealth == 0) && (_playable))
                        {
                            Destroy(_attackedCard.gameObject);
                        }
                        _playable = false; //Блокировка карты, чтобы её можно было сыграть только раз за ход
                        break;
                }
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
                        Debug.Log(GameManager._koloda.Count + " ID: " + _ID);
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
                    transform.localScale *= c_scaleMult;
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
                    transform.localScale /= c_scaleMult;
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

        private bool Taunted (Card card)
        {
            if ((card._ID == 103) || (card._ID == 204) || (card._ID == 303) || (card._ID == 308) || (card._ID == 406) || (card._ID == 501) || (card._ID == 603))
                return true;
            else return false;
        }

        private bool Charge()
        {
            if ((_ID == 104) || (_ID == 203) || (_ID == 407) || (_ID == 604))
                return true;
            else return false;
        }
    }
}