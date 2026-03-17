using UnityEngine;

namespace Items
{
    [RequireComponent(typeof(Weapon))]
    public class Bow : MonoBehaviour
    {
    
        [HideInInspector] public Weapon weapon;
        [SerializeField] private GameObject bowPrefab;
        [SerializeField] private GameObject bowAndArrowPrefab;
        [SerializeField] private ParticleSystem arrowParticles;
        [SerializeField] private ParticleSystem blastParticles;
        private AudioSource _sound;
    
        private void Awake()
        {
            weapon = GetComponent<Weapon>();
            _sound = GetComponent<AudioSource>();
        }
    
        public void Draw()
        {
            bowPrefab.SetActive(false);
            bowAndArrowPrefab.SetActive(true);
        }

        public void Loose()
        {
            bowPrefab.SetActive(true);
            bowAndArrowPrefab.SetActive(false);
            arrowParticles.Play();
            blastParticles.Play();
            _sound.Play();
        }
    
    }
}