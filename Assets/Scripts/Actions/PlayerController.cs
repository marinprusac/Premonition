using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Actors;
using Animations;
using Grid;
using Items;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace Actions
{
    public class PlayerController : Controller
    {
        private Camera _camera;

        public int attackingFor;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }
        

        private T GetSelectedObject<T>() where T : MonoBehaviour
        {
            if (!Input.GetMouseButtonDown(0)) return null;
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            return !Physics.Raycast(ray, out RaycastHit hit) ? null : hit.collider.GetComponent<T>();
        }

        private bool _skipping;

        public void SkipTurn()
        {
            _skipping = true;
        }
    

        public override void MoveAction(Actor actor, Action onDone)
        {
            var reachableTiles = NavigationServer.GetAllReachableTilesWithinRange(actor);
            Highlighter.Instance.HighlightTiles(reachableTiles);
            StartCoroutine(WaitForPlayerMoveAction(actor, reachableTiles, onDone));
        }
    
        public override void AttackAction(Actor actor, Action onDone)
        {
            var hittableActors = NavigationServer.GetAllEntitiesInWeaponsRange(actor, actor.equippedWeapon.stats);
            if (hittableActors.Count == 0)
            {
                onDone();
                return;
            }
            Highlighter.Instance.HighlightActors(hittableActors);
            StartCoroutine(WaitForPlayerAttackAction(actor, hittableActors, onDone));
        }


        private IEnumerator WaitForPlayerMoveAction(Actor actor, List<Tile> reachableTiles, Action onDone)
        {
            Tile selectedTile;
            do
            {
                yield return null;
                if (_skipping)
                {
                    Highlighter.Instance.ClearHighlights();
                    onDone();
                    _skipping = false;
                    StopAllCoroutines();
                }
                selectedTile = GetSelectedObject<Tile>();
            } while (!selectedTile || !reachableTiles.Contains(selectedTile));

            var path = NavigationServer.GetPath(actor.StandingOnTile, selectedTile, actor.actorStats);
            
            Highlighter.Instance.ClearHighlights();
            
            if (path is not null && path.Length > 0)
                actor.StandingOnTile = path[^1];
            
            CombatServer.Move(actor, path, onDone);
        }

        private IEnumerator WaitForPlayerAttackAction(Actor actor, List<Actor> hittableActors, Action onDone)
        {
            Actor selectedActor;
            attackingFor = Mathf.Min(2, actor.currentEnergy);
            do
            {
                if (_skipping)
                {
                    Highlighter.Instance.ClearHighlights();
                    onDone();
                    _skipping = false;
                    StopAllCoroutines();
                }
                yield return null;
                attackingFor = Mathf.Clamp(attackingFor + (Input.GetKeyDown(KeyCode.W) ? 1 : Input.GetKeyDown(KeyCode.S) ? -1 : 0), 0, actor.currentEnergy);
                selectedActor = GetSelectedObject<Actor>();
            } while (selectedActor is null || !hittableActors.Contains(selectedActor));
            
            Highlighter.Instance.ClearHighlights();
            
            
            CombatServer.Attack(actor, selectedActor, attackingFor, onDone);
            
        }
    }
}