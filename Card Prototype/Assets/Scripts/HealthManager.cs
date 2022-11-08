using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cards
{
    public class HealthManager : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _healthIndicatorFirstPlayer;
        [SerializeField]
        private TextMeshProUGUI _healthIndicatorSecondPlayer;

        [Header("Здоровье первого игрока")]
        public int _healthFirstPlayer = 10;
        [Header("Здоровье второго игрока")]
        public int _healthSecondPlayer = 10;

        private void Awake()
        {
            UpdateHealthIndicators();
        }

        private void UpdateHealthIndicators()
        {
            _healthIndicatorFirstPlayer.text = _healthFirstPlayer.ToString();
            _healthIndicatorSecondPlayer.text = _healthSecondPlayer.ToString();
        }

        public void HeroDamage(ushort _attack, uint _player)
        {
            if (_player == 2)
            {
                _healthFirstPlayer -= _attack;
            }
            else
            {
                _healthSecondPlayer -= _attack;
            }
            UpdateHealthIndicators();
            if (_healthFirstPlayer <= 0)
            {
                Debug.Log("Победа второго игрока");
                UnityEditor.EditorApplication.isPaused = true;
            }
            if (_healthSecondPlayer <= 0)
            {
                Debug.Log("Победа первого игрока");
                UnityEditor.EditorApplication.isPaused = true;
            }
        }

        #region Тест индикаторов здоровья игроков
        /*
        //Тест индикаторов здоровья
        private void TestHealthIndicators()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                _healthFirstPlayer -= 1;
                _healthSecondPlayer += 1;
            }
            UpdateHealthIndicators();
        }
        private void Update()
        {
            TestHealthIndicators();
        }
        */
        #endregion
    }
}