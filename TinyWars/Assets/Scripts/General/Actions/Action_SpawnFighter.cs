using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TinyWars/Action/SpawnFighter", fileName = "SpawnFighter_ActionData")]
public class Action_SpawnFighter : TWAction
{
    [Header("FIGHTER TO SPAWN")]
    [SerializeField] private CombatHandler _fighterToSpawn;

    public override TWAction GenerateAction(GameObject initiator, List<GameObject> targets)
    {
        Action_SpawnFighter newAction = CreateInstance<Action_SpawnFighter>();
        newAction._initiator = initiator;
        newAction._targets = targets;

        newAction._iCombatHandler = newAction._initiator.GetComponent<CombatHandler>();
        foreach (GameObject target in newAction._targets)
        {
            CombatHandler tCombatHandler = target.GetComponent<CombatHandler>();
            newAction._tCombatHandlers.Add(target, tCombatHandler);
        }

        newAction._fighterToSpawn = _fighterToSpawn;

        newAction._generated = true;

        return newAction;
    }

    public override void EnactAction()
    {
        base.EnactAction();

        List<CombatHandler> target = new List<CombatHandler>();
        CombatRow allyRow = _iCombatHandler.CurrentRow;
        int backmostOpenSlot = allyRow.GetBackmostOpenSlot();
        if (backmostOpenSlot < 0) return;

        allyRow.SpawnFighter(_fighterToSpawn, backmostOpenSlot);
    }
}
