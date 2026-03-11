using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TinyWars/Action/ClearCorpse", fileName = "ClearCorpse_ActionData")]
public class Action_ClearCorpse : TWAction
{
    public override TWAction GenerateAction(GameObject initiator, List<GameObject> targets)
    {
        Action_ClearCorpse newAction = CreateInstance<Action_ClearCorpse>();
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

        //Play corpse animation.

        //Clear currently occupied slot.
        int currentSlot = _iCombatHandler.CurrentRow.GetFighterSlotIndex(_iCombatHandler);
        _iCombatHandler.CurrentRow.Slots[currentSlot] = null;

        //TODO: Remove this later.
        Destroy(Initiator);
    }
}
