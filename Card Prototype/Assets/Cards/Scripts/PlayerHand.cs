using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cards
{
    public class PlayerHand : MonoBehaviour
    {
        private Card[] _cardsInHand;

        [SerializeField]
        private Transform[] _positions;

        private void Start()
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
            StartCoroutine(MoveInHand(newCard, _positions[result]));
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

        public IEnumerator MoveInHand(Card card, Transform parent)
        {
            yield return UpCard(card);
            yield return RotateCard(card);
            var time = 0f;
            var startPos = card.transform.position;
            var endPos = parent.position;
            while (time <1f)
            {
                card.transform.position = Vector3.Lerp(startPos, endPos, time);
                time += Time.deltaTime;
                yield return null;
            }
            //Чтобы точно расположить карту на месте
            card.transform.position = endPos;

            card.State = CardStateType.InHand;
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
            var endRot = new Quaternion(0, 0, 0.1f, 0);
            //var endRot = new Quaternion();
            //endRot.eulerAngles = new Vector3(0, 0, 180);
            while (time <= 1f)
            {
                card.transform.rotation = Quaternion.Slerp(startRot, endRot, time);
                time += Time.deltaTime;
                yield return null;
            }
            //endRot.eulerAngles = new Vector3(0, 0, 180);
            card.transform.rotation = endRot;
            yield return null;
        }
    }
}