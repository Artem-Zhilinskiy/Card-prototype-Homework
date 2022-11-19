using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Cards
{
    public class PlayerHand : MonoBehaviour
    {
        public Card[] _cardsInHand;

        [SerializeField]
        public Transform[] _positions;

        private void Awake()
        {
            _cardsInHand = new Card[_positions.Length];
        }

        public bool SetNewCard(Card newCard)
        {
            var result = GetLastPosition();

            if (result == -1)
            {
                Destroy(newCard.gameObject);
                return false;
            }
            _cardsInHand[result] = newCard;
            StartCoroutine(MoveInHand(newCard, _positions[result], true, true));
            return true;
        }

        private int GetLastPosition()
        {
            for (int i = 0; i < _cardsInHand.Length; i++)
            {
                if (_cardsInHand[i] == null) return i;
            }
            return -1;
        }

        //Обнуление места в массиве, когда карта покидает руку
        public void CardIsPlayed(Card _card)
        {
            foreach (var _cardInHand in _cardsInHand)
            {
                if (_card == _cardInHand)
                {
                    var _index = Array.IndexOf(_cardsInHand, _cardInHand);
                    _cardsInHand[_index] = null;
                }
            }
        }

        public IEnumerator MoveInHand(Card card, Transform parent, bool _up, bool _rotate)
        {
            var startPos = card.transform.position;
            var endPos = parent.position;
            if (_up)
            {
                yield return UpCard(card);
            }
            if (_rotate)
            {
                yield return RotateCard(card);
            }
            var time = 0f;
            while (time <1f)
            {
                card.transform.position = Vector3.Lerp(startPos, endPos, time);
                time += Time.deltaTime;
                yield return null;
            }
            //Чтобы точно расположить карту на месте
            card.transform.position = endPos;

            switch (card.State)
            {
                case CardStateType.InDeck:
                    card.State = CardStateType.InHand;
                    break;
                case CardStateType.InHand:
                    card.State = CardStateType.InDeck;
                    break;
            }
        }

        //Перегрузка метода MoveInHand с Vector3
        public IEnumerator MoveInHand(Card card, Vector3 endPos, bool _up, bool _rotate)
        {
            var startPos = card.transform.position;
            //var endPos = parent.position;
            if (_up)
            {
                yield return UpCard(card);
            }
            if (_rotate)
            {
                yield return RotateCard(card);
            }
            var time = 0f;
            while (time < 1f)
            {
                card.transform.position = Vector3.Lerp(startPos, endPos, time);
                time += Time.deltaTime;
                yield return null;
            }
            //Чтобы точно расположить карту на месте
            card.transform.position = endPos;

            switch (card.State)
            {
                case CardStateType.InDeck:
                    card.State = CardStateType.InHand;
                    break;
                case CardStateType.InHand:
                    card.State = CardStateType.InDeck;
                    break;
            }
        }

        private IEnumerator UpCard(Card card)
        {
            var time = 0f;
            var startPos = card.transform.position;
            var endPos = new Vector3(card.transform.position.x, 40f, card.transform.position.z);
            while (time <= 1f)
            {
                card.transform.position = Vector3.Lerp(startPos, endPos, time);
                time += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator RotateCard(Card card)
        {
            var time = 0f;
            var startRot = card.transform.rotation;
            //var endRot = new Quaternion(0, 0, 0.1f, 0);
            var endRot = new Quaternion();
            if (startRot.eulerAngles == new Vector3(0,0,0))
            {
                endRot.eulerAngles = new Vector3(0, 0, 180);
            }
            else
            {
                endRot.eulerAngles = new Vector3(0, 0, 0);
            }
            while (time <= 1f)
            {
                card.transform.rotation = Quaternion.Slerp(startRot, endRot, time);
                time += Time.deltaTime;
                yield return null;
            }
            //endRot.eulerAngles = new Vector3(0, 0, 180);
            if (startRot.eulerAngles == new Vector3(0, 0, 0))
            {
                endRot.eulerAngles = new Vector3(0, 0, 180);
            }
            else
            {
                endRot.eulerAngles = new Vector3(0, 0, 0);
            }
            card.transform.rotation = endRot;
            yield return null;
        }
    }
}