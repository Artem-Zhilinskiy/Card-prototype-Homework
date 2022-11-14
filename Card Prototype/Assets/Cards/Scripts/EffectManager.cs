using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class EffectManager : MonoBehaviour
    {
        public void PlayEffect(Card card)
        {
            Debug.Log("Эффект карты " + card._ID);
        }
    }
}
