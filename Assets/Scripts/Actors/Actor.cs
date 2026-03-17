using System;
using Actions;
using Animations;
using Grid;
using Items;
using UnityEngine;

namespace Actors
{
    public class Actor : MonoBehaviour
    {
    
        public ActorStats actorStats;
        public HexCoordinates Coordinates { get; private set; }
        private GridSettings _gridSettings;
        public Shield shield;
        public Weapon equippedWeapon;
        public Controller controller;
        public bool alive = true;
        public int damageTaken;
        public int currentEnergy;
        private AudioSource soundPlayer;
        [SerializeField] private AudioClip deathSound;
        [SerializeField] private AudioClip walkingSound;
        public Light light;
        
        
        
        public Tile StandingOnTile
        {
            get => GridManager.Instance.GetTile(Coordinates + actorStats.height * HexCoordinates.Down);
        
            set
            {
                var tileCoords = value.Coordinates;
                Coordinates = tileCoords + actorStats.height * HexCoordinates.Up;
            }
        }
    
        private void Awake()
        {
            _gridSettings = GridManager.Instance.settings;
            soundPlayer = GetComponent<AudioSource>();
        }

        private void Start()
        {
            Coordinates = HexCoordinates.FromPixelCoordinates(transform.position, _gridSettings);
            currentEnergy = actorStats.maxEnergy;
        }

        public void PlayDeathSound()
        {
            soundPlayer.clip = deathSound;
            soundPlayer.loop = false;
            soundPlayer.Play();
            light.enabled = false;
        }

        public void StartPlayingWalkingSound()
        {
            soundPlayer.clip = walkingSound;
            soundPlayer.loop = true;
            soundPlayer.Play();
        }

        public void StopPlayingWalkingSound()
        {
            soundPlayer.Stop();
        }

    }
}