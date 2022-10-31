using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class PlayerPlayed : MonoBehaviour
    {
        [SerializeField]
        public Transform[] _positions;
        private Card[] _cardsInPlayed;

        private void Awake()
        {
            _cardsInPlayed = new Card[_positions.Length];
        }
        private int GetLastPosition()
        {
            for (int i = 0; i < _cardsInPlayed.Length; i++)
            {
                if (_cardsInPlayed[i] == null) return i;
            }
            return -1;
        }

        public void MoveInPlayed(Card card, Vector3 _initialPosition)
        {
            var result = GetLastPosition();

            if (result == -1)
            {
                StartCoroutine(MoveInPlayedCoroutine(card, _initialPosition));
            }
            else
            {
                _cardsInPlayed[result] = card;
                StartCoroutine(MoveInPlayedCoroutine(card, _positions[result]));
                card.State = CardStateType.OnTable;
            }
        }

        private IEnumerator MoveInPlayedCoroutine(Card card, Transform parent)
        {
            var startPos = card.transform.position;
            var endPos = parent.position;
            var time = 0f;
            while (time < 1f)
            {
                card.transform.position = Vector3.Lerp(startPos, endPos, time);
                time += Time.deltaTime;
                yield return null;
            }
            //Чтобы точно расположить карту на месте
            card.transform.position = endPos;
        }

        //Перегрузка метода MoveInHand с Vector3
        public IEnumerator MoveInPlayedCoroutine(Card card, Vector3 endPos)
        {
            var startPos = card.transform.position;
            var time = 0f;
            while (time < 1f)
            {
                card.transform.position = Vector3.Lerp(startPos, endPos, time);
                time += Time.deltaTime;
                yield return null;
            }
            //Чтобы точно расположить карту на месте
            card.transform.position = endPos;
        }
    }


}