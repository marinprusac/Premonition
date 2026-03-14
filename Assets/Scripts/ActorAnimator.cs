using System;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public static class ActorAnimator
{
    
    public static void MoveAnimation(Actor actor, Tile[] path, Action callback = null)
    {
        if (path.Length < 2) return;
        var currentTile = path[0];
        var heightCompensation = actor.transform.position - path[0].transform.position;
        var startingRotation = actor.transform.rotation;
        foreach (var tile in path[1..])
        {
            var lastTile = tile == path[^1];
            var startPos = currentTile.transform.position + heightCompensation;
            var endPos = tile.transform.position + heightCompensation;
            var targetRotation = Quaternion.Euler(0, Quaternion.LookRotation(endPos - startPos).eulerAngles.y, 0);
            var rotation = startingRotation;
            var anim = new Animation(0.5f, t =>
            {
                actor.transform.position = Vector3.Lerp(startPos, endPos, t);
                actor.transform.rotation = Quaternion.Lerp(rotation, targetRotation, t);

            }, lastTile ? callback : null);
            currentTile = tile;
            startingRotation = targetRotation;
            AnimationManager.Instance.QueueAnimation(anim);
        }
        
    }

    public static void TurnTowards(Actor actor, Vector3 target, Action callback = null)
    {
        var rotation = actor.transform.rotation;
        var targetRotation = Quaternion.Euler(0, Quaternion.LookRotation(target - actor.transform.position).eulerAngles.y, 0);
        
        var anim = new Animation(0.5f, (t) =>
            actor.transform.rotation = Quaternion.Lerp(rotation, targetRotation, t)
        , callback);
        AnimationManager.Instance.QueueAnimation(anim);
    }


    public static void SwingSword(Actor actor, Vector3 targetPosition, Sword sword, Action callback = null)
    {
        AnimationManager.Instance.QueueAnimation(new Animation(1,
            t =>
            {
                sword.transform.localRotation =
                    Quaternion.Euler(t < 0.5f ? Mathf.Lerp(0, 45, 2 * t) : Mathf.Lerp(45, 0, 2 * t - 1), 0, 0);
                actor.transform.position = t < 0.5f ? Vector3.Lerp(actor.entity.Coordinates.ToPixelCoordinates(HexGrid.Instance.settings), targetPosition, t) : Vector3.Lerp(targetPosition, actor.entity.Coordinates.ToPixelCoordinates(HexGrid.Instance.settings), t);
            }
            , callback));
    }
    
    public static void DrawBow(Bow bow, Vector3 at, Action callback = null)
    {
        var dir = at - bow.transform.position;
        var angle = -Mathf.Asin(dir.y / dir.magnitude) * Mathf.Rad2Deg;
        
        AnimationManager.Instance.QueueAnimation(new InstantAnimation(bow.Draw));
        AnimationManager.Instance.QueueAnimation(new Animation(1, t =>
            bow.transform.localRotation = Quaternion.Euler(Mathf.Lerp(0, angle, t), 0, 0)
        , callback));
    }
    
    public static void LooseArrow(Bow bow, Action callback = null)
    {
        AnimationManager.Instance.QueueAnimation(new InstantAnimation(bow.Loose));
        AnimationManager.Instance.QueueAnimation(new PauseAnimation(0.5f));
        AnimationManager.Instance.QueueAnimation(new Animation(1, _ =>
            bow.transform.localRotation = Quaternion.identity
        , callback));
    }
    
    public static void BraceShield(Shield shield, Action callback = null)
    {
        AnimationManager.Instance.QueueAnimation(new InstantAnimation(shield.Brace));
        AnimationManager.Instance.QueueAnimation(new Animation(0.5f, t =>
            shield.transform.localPosition = Vector3.Lerp(Vector3.zero, new Vector3(0.2f, 0, 0.2f), t)
        , callback));
    }

    public static void UnbraceShield(Shield shield, Action callback = null)
    {
        AnimationManager.Instance.QueueAnimation(new InstantAnimation(shield.Unbrace));
        AnimationManager.Instance.QueueAnimation(new Animation(0.5f, t =>
            shield.transform.localPosition = Vector3.Lerp(new Vector3(0.2f, 0, 0.2f), Vector3.zero, t)
        , callback));
    }
}