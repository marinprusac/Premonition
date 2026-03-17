using System;
using System.Numerics;
using Actors;
using Grid;
using Items;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Animations
{
    public static class ActorAnimator
    {
    
        public static void Move(Actor actor, Tile[] path, Action callback = null)
        {
            if (path is null || path.Length < 2)
            {
                callback?.Invoke();
                return;
            };
            var currentTile = path[0];
            var heightCompensation = actor.transform.position - path[0].transform.position;
            var startingRotation = actor.transform.rotation;
            
            AnimationManager.Instance.QueueAnimation(new InstantAnimation(actor.StartPlayingWalkingSound));
            foreach (var tile in path[1..])
            {
                var lastTile = tile == path[^1];
                var startPos = currentTile.transform.position + heightCompensation;
                var endPos = tile.transform.position + heightCompensation;
                var targetRotation = Quaternion.Euler(0, Quaternion.LookRotation(endPos - startPos).eulerAngles.y, 0);
                var rotation = startingRotation;
                currentTile = tile;
                startingRotation = targetRotation;
                AnimationManager.Instance.QueueAnimation(new Animation(0.5f, t =>
                {
                    actor.transform.position = Vector3.Lerp(startPos, endPos, t);
                    actor.transform.rotation = Quaternion.Lerp(rotation, targetRotation, t);

                }, lastTile ? callback : null));
            }
            AnimationManager.Instance.QueueAnimation(new InstantAnimation(actor.StopPlayingWalkingSound));
        }

        public static void TurnTowards(Actor actor, Vector3 target, Action callback = null)
        {
            var rotation = actor.transform.rotation;
            var targetRotation = Quaternion.Euler(0, Quaternion.LookRotation(target - actor.transform.position).eulerAngles.y, 0);
        
            var anim = new Animation(0.25f, (t) =>
                    actor.transform.rotation = Quaternion.Lerp(rotation, targetRotation, t)
                , callback);
            AnimationManager.Instance.QueueAnimation(anim);
        }


        public static void SwingSword(Actor actor, Vector3 targetPosition, Sword sword, Action callback = null)
        {
            AnimationManager.Instance.QueueAnimation(new InstantAnimation(sword.Slash));
            AnimationManager.Instance.QueueAnimation(new PauseAnimation(0.2f));
            AnimationManager.Instance.QueueAnimation(new Animation(0.5f,
                t =>
                {
                    sword.transform.localRotation =
                        Quaternion.Euler(t < 0.5f ? Mathf.Lerp(0, 45, 2 * t) : Mathf.Lerp(45, 0, 2 * t - 1), 0, 0);
                    actor.transform.position = t < 0.5f ? Vector3.Lerp(actor.Coordinates.ToPixelCoordinates(GridManager.Instance.settings), targetPosition, t) : Vector3.Lerp(targetPosition, actor.Coordinates.ToPixelCoordinates(GridManager.Instance.settings), t);
                }
                , callback));
        }
    
        public static void DrawBow(Bow bow, Vector3 at, Action callback = null)
        {
            var dir = at - bow.transform.position;
            var angle = -Mathf.Asin(dir.y / dir.magnitude) * Mathf.Rad2Deg;
        
            AnimationManager.Instance.QueueAnimation(new InstantAnimation(bow.Draw));
            AnimationManager.Instance.QueueAnimation(new Animation(0.5f, t =>
                    bow.transform.localRotation = Quaternion.Euler(Mathf.Lerp(0, angle, t), 0, -45)
                , callback));
        }
    
        public static void LooseArrow(Bow bow, Action callback = null)
        {
            AnimationManager.Instance.QueueAnimation(new InstantAnimation(bow.Loose));
            AnimationManager.Instance.QueueAnimation(new PauseAnimation(0.5f));
            AnimationManager.Instance.QueueAnimation(new Animation(0.5f, _ =>
                    bow.transform.localRotation = Quaternion.Euler(0, 0, -45)
                , callback));
        }
    
        public static void BraceShield(Shield shield, Action callback = null)
        {
            AnimationManager.Instance.QueueAnimation(new InstantAnimation(shield.Brace));
            AnimationManager.Instance.QueueAnimation(new Animation(0.25f, t =>
                    shield.transform.localPosition = Vector3.Lerp(Vector3.zero, new Vector3(0.2f, 0, 0.2f), t)
                , callback));
        }

        public static void UnbraceShield(Shield shield, Action callback = null)
        {
            AnimationManager.Instance.QueueAnimation(new InstantAnimation(shield.Unbrace));
            AnimationManager.Instance.QueueAnimation(new Animation(0.25f, t =>
                    shield.transform.localPosition = Vector3.Lerp(new Vector3(0.2f, 0, 0.2f), Vector3.zero, t)
                , callback));
        }

        public static void Attack(Actor attacker, Actor defender, Action onDone = null)
        {
            var sword = attacker.equippedWeapon.GetComponent<Sword>();
            var bow = attacker.equippedWeapon.GetComponent<Bow>();
            var shield = defender.shield;

            TurnTowards(attacker, defender.transform.position);
            TurnTowards(defender, attacker.transform.position);
            if (shield) BraceShield(shield);
            if (sword) SwingSword(attacker, defender.transform.position, sword);
            if (bow) DrawBow(bow, defender.transform.position);
            if (bow) LooseArrow(bow);
            if (shield) UnbraceShield(shield);

            AnimationManager.Instance.QueueAnimation(new EmptyAnimation(onDone));
        }

        public static void Die(Actor actor, Action onDone = null)
        {
            var forwardDir = actor.transform.forward;
            var rightDir = actor.transform.right;
            
            var startingPosition = actor.transform.position;
            var newPosition = startingPosition + (actor.actorStats.height * GridManager.Instance.settings.tileHeight) / 2 * Vector3.down;
            var oldRot = actor.transform.rotation;
            var newRot = Quaternion.AngleAxis(-90, rightDir);
            
            
            AnimationManager.Instance.QueueAnimation(new InstantAnimation(actor.PlayDeathSound));
            AnimationManager.Instance.QueueAnimation(new Animation(1, t =>
            {
                actor.transform.rotation = Quaternion.Slerp(oldRot, newRot, t);
                actor.transform.position = Vector3.Lerp(startingPosition, newPosition, t);

            }, onDone));
        }

        public static void Nothing(Action onDone)
        {
            AnimationManager.Instance.QueueAnimation(new EmptyAnimation(onDone));
        }
    }
}