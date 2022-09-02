using OneLine;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Cards.ScriptableObjects
{
	[CreateAssetMenu(fileName = "NewCardPackConfiguration", menuName = "CardConfigs/Card Pack Configuration")]
	public class CardPackConfiguration : ScriptableObject
	{
		[SerializeField]
		public SideType _sideType;
		[SerializeField]
		private ushort _cost;
		[SerializeField, OneLine(Header = LineHeader.Short)]
		public CardPropertiesData[] _cards;
		//public CardPropertiesData[] _cards { get; private set; }

		public IEnumerable<CardPropertiesData> UnionProperties(IEnumerable<CardPropertiesData> array)
		{
			TryToContruct();

			return array.Union(_cards);
		}

		public IEnumerable<CardPropertiesData> ReplaceProperties(IEnumerable<CardPropertiesData> array)
		{
			TryToContruct();

			return _cards;
		}

		private void TryToContruct()
		{
			for(int i = 0; i < _cards.Length; i++)
			{
				_cards[i].Cost = _cost;
			}
		}
	}
}