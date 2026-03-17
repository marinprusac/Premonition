using System;
using Actors;
using UnityEngine;

namespace Actions
{
    public abstract class Controller : MonoBehaviour
    {
        public abstract void MoveAction(Actor actor, Action onDone);
        public abstract void AttackAction(Actor actor, Action onDone);

    }
}