using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TinyWars/Character/CombatData", fileName = "CharacterName_CombatData")]
public class SOCombatData : ScriptableObject
{
    [Header("BASE HEALTH")]
    public float BaseHealth = 5f;

    [Header("BASE ATTACK")]
    public float BaseAttack = 2f;

    [Header("LEVEL UP")]
    public float HealthIncrease = 3f;
    public float AttackIncrease = 3f;
}
