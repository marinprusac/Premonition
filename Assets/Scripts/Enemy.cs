using System;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class Enemy : MonoBehaviour
{
    public Actor actor;
    public bool CanSeeThePlayer => GridNavigation.IsEntityVisible(actor.entity.Coordinates, EntityManager.Instance.player.actor.entity);
    
    private void Awake()
    {
        actor = GetComponent<Actor>();
    }
    
}