using System;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class Actor : MonoBehaviour
{
    public MovementStats movementStats;
    [HideInInspector] public Entity entity;
    
    private void Awake()
    {
        entity = GetComponent<Entity>();
    }

    public void Move(Tile[] path, Action callback=null)
    {
        if (path.Length == 1)
        {
            callback?.Invoke();
            return;
        }
        ActorAnimator.MoveAnimation(this, path, callback);
        entity.StandingOnTile = path[^1];
    }




}