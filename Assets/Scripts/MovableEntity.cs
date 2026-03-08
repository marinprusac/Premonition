using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Entity))]
public class MovableEntity : MonoBehaviour
{
    public MovementStats movementStats;
    public Entity entity;

    
    private void Start()
    {
        entity = GetComponent<Entity>();
    }
    
    public void MoveAnimated(Tile[] path, Action callback = null)
    {
        if (path.Length < 2)
        {
            callback?.Invoke();
            return;
        };
        StartCoroutine(MultipleTileMovementAnimation(path, 1, callback));
    }
    
    public void Move(Tile destination)
    {
        var heightToCompensate = (transform.position - entity.StandingOnTile.transform.position).y;
        var toPos = destination.transform.position + Vector3.up * heightToCompensate;
        transform.position = toPos;
        entity.UpdateHexCoordinates();

    }


    
    private IEnumerator MultipleTileMovementAnimation(Tile[] path, float time, Action callback)
    {
        
        var timePerTile = time / (path.Length - 1);
        for (var i = 0; i < path.Length - 1; i++)
        {
            Move(path[i+1]);
            yield return new WaitForSecondsRealtime(timePerTile);
        }
        
        callback?.Invoke();
    }
}