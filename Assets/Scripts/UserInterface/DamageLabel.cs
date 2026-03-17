using System;
using System.Collections;
using Actors;
using TMPro;
using UnityEngine;

namespace UserInterface
{
    public class DamageLabel : MonoBehaviour
    {
        private int _rememberedDamage;
        
        private TMP_Text _damageText;
        private static Actor PlayerActor => ActorManager.Instance.playerActor;
        
        private void Start()
        {
            _damageText = GetComponent<TMP_Text>();
            _rememberedDamage = 0;
            _damageText.text = "0";
        }

        private void Update()
        {
            if (PlayerActor.damageTaken == _rememberedDamage) return;
            StartCoroutine(RedAnimation());
            _rememberedDamage = PlayerActor.damageTaken;
            _damageText.text = $"{_rememberedDamage}";
        }


        private IEnumerator RedAnimation()
        {
            var time = 3f;
            while (time > 0)
            {
                _damageText.color = Color.Lerp(Color.red, Color.white, 1 - time/3.0f);
                time -= Time.deltaTime;
                yield return null;
            }
            
        }
        
    }
}