using System;
using Actors;
using Animations;
using Grid;
using UnityEngine;

namespace Actions
{
    public static class CombatServer
    {


        private static bool EnergyCheck(Actor actor, int energy)
        {
            if (actor.currentEnergy < energy)
                throw new ArgumentOutOfRangeException($"An actor ({actor.name}) is trying to use more energy ({energy}) than it currently has ({actor.currentEnergy}).");
            if (energy < 0)
                throw new ArgumentOutOfRangeException(
                    $"An actor ({actor.name}) needs to use at least 0 energy to attack, but is trying to use {energy} instead.");
            return true;
        }

        private static bool IsLegalAttack(Actor attacker, Actor defender)
        {
            var weapon = attacker.equippedWeapon;
            return true;
        }
        
        public static void Attack(Actor attacker, Actor defender, int energy, Action onDone)
        {
            if (energy <= 0)
            {
                onDone();
                return;
            }
            if (!EnergyCheck(attacker, energy) || !IsLegalAttack(attacker, defender)) return;
            var weaponStats = attacker.equippedWeapon.stats;
            var heightDifference = (attacker.Coordinates - defender.Coordinates).h;
            var heightDamage = Mathf.Max(heightDifference * weaponStats.heightBenefit, 0);
            var baseDamage = weaponStats.baseDamage;
            var shieldDamageMitigation = defender.shield ? 1 : 0;
            var heightDamageMitigation = Mathf.Max(-heightDifference/2f, 0);
            var undefendedDamage = (int)(energy + heightDamage + baseDamage - shieldDamageMitigation - heightDamageMitigation);
            var energyUsedToDefend = Mathf.Min(undefendedDamage, defender.currentEnergy);
            var finalDamage = undefendedDamage - energyUsedToDefend;
            
            ActorAnimator.Attack(attacker, defender);
            AnimationManager.Instance.QueueAnimation(new InstantAnimation(() =>
            {
                attacker.currentEnergy -= energy;
                defender.currentEnergy -= energyUsedToDefend;
                defender.damageTaken += finalDamage;
                defender.alive = defender.damageTaken < defender.actorStats.damageThreshold;
            }));
            if(defender.damageTaken + finalDamage >= defender.actorStats.damageThreshold) ActorAnimator.Die(defender);
            ActorAnimator.Nothing(onDone);
            
            
        }

        public static void Move(Actor actor, Tile[] path, Action onDone)
        {
            if (path is null)
            {
                onDone();
                return;
            }
            var pathLength = path.Length - 1;
            if (pathLength < 1)
            {
                onDone();
                return;
            }
            var energy = pathLength * 2;

            if (!EnergyCheck(actor, energy)) return;
            
            actor.currentEnergy -= energy;
            actor.StandingOnTile = path[^1];
            ActorAnimator.Move(actor, path, onDone);
        }
    }
}