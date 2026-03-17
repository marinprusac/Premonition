using System;
using Actions;
using Actors;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class SkipButton : MonoBehaviour
    {
        private Button _button;
        private TMP_Text _text;
        private PlayerController _playerController;

        private bool PlayerPlaying => TurnManager.Instance.CurrentlyActing == ActorManager.Instance.playerActor;
        private bool _consideredPlaying = true;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _text = GetComponentInChildren<TMP_Text>();
            _playerController = FindAnyObjectByType<PlayerController>();
        }

        private void Start()
        {
            _button.onClick.AddListener(_playerController.SkipTurn);
        }

        private void Update()
        {
            if (PlayerPlaying && !_consideredPlaying)
            {
                _button.interactable = true;
                _text.text = "Skip";
            }
            if(!PlayerPlaying && _consideredPlaying)
            {
                _button.interactable = false;
                _text.text = "Waiting...";
            }
            _consideredPlaying = PlayerPlaying;
        }
    }
}