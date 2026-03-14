using System;
using UnityEngine;

[RequireComponent(typeof(Weapon))]
public class Sword : MonoBehaviour
{
    [HideInInspector] public Weapon weapon;

    private void Awake()
    {
        weapon = GetComponent<Weapon>();
    }
}