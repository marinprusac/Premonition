using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public List<Entity> entities;
    public List<Actor> actors;
    public Player player;

    public static EntityManager Instance;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        entities = FindObjectsByType<Entity>(FindObjectsSortMode.None).ToList();
        actors = FindObjectsByType<Actor>(FindObjectsSortMode.None).ToList();
        player = FindAnyObjectByType<Player>();
    }
    

}