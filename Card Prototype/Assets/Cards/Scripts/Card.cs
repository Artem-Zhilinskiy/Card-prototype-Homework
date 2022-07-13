using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Cards
{
    public class Card : MonoBehaviour
    {
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

        public void Configuration (CardPropertiesData data, Material picture, string description)
        {
            _cost.text = data.Cost.ToString();
            _picture.material = picture;
            _name.text = data.Name;
            _description.text = description;
            _attack.text = data.Attack.ToString();
            _health.text = data.Health.ToString();
            _type.text = CardUnitType.None == data.Type ? "" : data.Type.ToString();

        }
    }
}