using System;
using Actions;
using Actors;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class AttackLabel : MonoBehaviour
    {
        private int _rememberedAttack;
        private PlayerController _controller;
        
        private TMP_Text _attackText;

        [SerializeField] private Image go;
        
        private void Start()
        {
            _attackText = GetComponent<TMP_Text>();
            _controller = FindAnyObjectByType<PlayerController>();
            
            _rememberedAttack = 0;
            _attackText.text = "0";
        }

        private void Update()
        {
            if (TurnManager.Instance.CurrentlyActing == ActorManager.Instance.playerActor &&
                TurnManager.Instance.combatPhase == TurnManager.CombatPhase.Attacking)
            {
                go.rectTransform.localScale = Vector2.one;
            }
            else
            {
                go.rectTransform.localScale = Vector2.zero;
            }
            
            if (_controller.attackingFor == _rememberedAttack) return;
            _rememberedAttack = _controller.attackingFor;
            _attackText.text = $"{_rememberedAttack}";
        }
        
    }
}