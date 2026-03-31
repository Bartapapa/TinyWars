using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TinyWars/Action/ClearCorpse", fileName = "ClearCorpse_ActionData")]
public class Action_ClearCorpse : TWAction
{
    public override TWAction GenerateAction(GameObject initiator, List<GameObject> targets)
    {
        Action_ClearCorpse newAction = CreateInstance<Action_ClearCorpse>();
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

        //Clear currently occupied slot.
        _iCombatHandler.CurrentRow.ClearFighterCorpse(_iCombatHandler);
    }
}
