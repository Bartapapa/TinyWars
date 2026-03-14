using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TinyWars/Action/HealSelf", fileName = "HealSelf_ActionData")]
public class Action_HealSelf : TWAction
{
    public override TWAction GenerateAction(GameObject initiator, List<GameObject> targets)
    {
        Action_HealSelf newAction = CreateInstance<Action_HealSelf>();
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

        _iCombatHandler.AnimationHandler.PlayAnimationWithBlend("Cast1");

        StatisticModifier newMod = new StatisticModifier(1f, StatisticModifierType.Flat, ModifierApplicationType.Permanent, _iCombatHandler.Character);
        _iCombatHandler.Health.AddModifier(newMod);

        Debug.Log(_iCombatHandler.gameObject.name + " has healed themselves!");
    }
}
