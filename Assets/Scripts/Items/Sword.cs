using UnityEngine;

namespace Items
{
    [RequireComponent(typeof(Weapon))]
    public class Sword : MonoBehaviour
    {
        [HideInInspector] public Weapon weapon;
        private AudioSource _sound;

        private void Awake()
        {
            weapon = GetComponent<Weapon>();
            _sound = GetComponent<AudioSource>();
        }

        public void Slash()
        {
            _sound.Play();
        }
    }
}