using System;
using Actors;
using TMPro;
using UnityEngine;

namespace UserInterface
{
    public class EnergyLabel : MonoBehaviour
    {
        private int _rememberedEnergy;
        private int _maxEnergy;
        
        private TMP_Text _energyText;
        private Actor PlayerActor => ActorManager.Instance.playerActor;


        private void Start()
        {
            _energyText = GetComponent<TMP_Text>();
            _rememberedEnergy = PlayerActor.currentEnergy;
            _maxEnergy = PlayerActor.actorStats.maxEnergy;
            _energyText.text = $"{_rememberedEnergy}/{_maxEnergy}";
        }

        private void Update()
        {
            if (PlayerActor.currentEnergy == _rememberedEnergy) return;
            _rememberedEnergy = PlayerActor.currentEnergy;
            _energyText.text = $"{_rememberedEnergy}/{_maxEnergy}";
        }
        
    }
}