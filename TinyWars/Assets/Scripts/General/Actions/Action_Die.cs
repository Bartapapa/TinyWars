using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "TinyWars/Action/Die", fileName = "Die_ActionData")]
public class Action_Die : TWAction
{
    public override TWAction GenerateAction(GameObject initiator, List<GameObject> targets)
    {
        Action_Die newAction = CreateInstance<Action_Die>();
        newAction._actionType = _actionType;
        newAction._simultaneousAction = _simultaneousAction;
        newAction._initiator = initiator;
        newAction._targets = targets;

        newAction._iCombatHandler = newAction._initiator.GetComponent<CombatHandler>();
        foreach (GameObject target in newAction._targets)
        {
            CombatHandler tCombatHandler = target.GetComponent<CombatHandler>();
            newAction._tCombatHandlers.Add(target, tCombatHandler);
        }
        newAction._generated = true;

        return newAction;
    }

    public override void EnactAction()
    {
        base.EnactAction();

        Debug.Log(_iCombatHandler.gameObject.name + " died!");
    }
}
