using System;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Weapon))]
public class Bow : MonoBehaviour
{
    
    public Weapon weapon;
    [SerializeField] private GameObject bowPrefab;
    [SerializeField] private GameObject bowAndArrowPrefab;
    [SerializeField] private ParticleSystem arrowParticles;
    [SerializeField] private ParticleSystem blastParticles;
    
    private void Awake()
    {
        weapon = GetComponent<Weapon>();
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
    }
    
}