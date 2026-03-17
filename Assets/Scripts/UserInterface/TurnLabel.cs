using System;
using Actions;
using Actors;
using TMPro;
using UnityEngine;

namespace UserInterface
{
    public class TurnLabel : MonoBehaviour
    {
        private int _rememberedTurn;
        
        private TMP_Text _turnText;
        
        private void Start()
        {
            _turnText = GetComponent<TMP_Text>();
            _rememberedTurn = 0;
            _turnText.text = "0";
        }

        private void Update()
        {
            if (TurnManager.Instance.turnCount == _rememberedTurn) return;
            _rememberedTurn = TurnManager.Instance.turnCount;
            _turnText.text = $"{_rememberedTurn}";
        }
        
    }
}